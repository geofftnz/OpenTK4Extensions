using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using NLog;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Image;
using OpenTKExtensions.Resources;
using OpenTK.Mathematics;

namespace OpenTKExtensions.Text
{
    /*
     * Signed Distance Field Font Rendering
     * 
     * Based on work by:
     * 
     * Chris Green / Valve: Improved Alpha-Tested Magniﬁcation for Vector Textures and Special Effects
     * http://www.valvesoftware.com/publications/2007/SIGGRAPH2007_AlphaTestedMagnification.pdf
     * 
     * Lonesock (Font SDF generator)
     * http://www.gamedev.net/topic/491938-signed-distance-bitmap-font-tool/
     * 
     * 
     * Most OpenTK text renderers out there work on the basis of getting System.Drawing to do the hard work 
     * and then write the output to a texture. This one maintains VBOs of MAXCHARS
     * 
     * 
     * Note: This class was originally named Font.
     *       This originally encapsulated both the font texture/metadata AND the VBOs + rendering.
     *       These functions are being split into Font & TextRenderer
     * 
    */
    public class TextRenderer : GameComponentBase, ITextRenderer, ITransformable, IResizeable, IRenderable
    {

        const int MAXCHARS = 4000;
        const int NUMVERTICES = MAXCHARS * 4;
        const int NUMINDICES = MAXCHARS * 6;

        public string Name { get; set; }
        public Font RendererFont { get; set; }

        private Vector3[] vertex;
        private BufferObject<Vector3> vertexVBO;

        private Vector2[] texcoord;
        private BufferObject<Vector2> texcoordVBO;

        private Vector4[] colour;
        private BufferObject<Vector4> colourVBO;

        private BufferObject<uint> indexVBO;

        private ShaderProgram shader;


        public int Count { get; private set; }
        public float GlobalScale { get; set; }

        public Matrix4 ViewMatrix { get; set; }
        public Matrix4 ModelMatrix { get; set; }
        public Matrix4 ProjectionMatrix { get; set; }
        public bool Visible { get; set; } = true;
        public int DrawOrder { get; set; }

        #region shaders
        private const string vertexShaderSource =
            @"#version 450
 
            uniform mat4 projection_matrix;
            uniform mat4 view_matrix;
            uniform mat4 model_matrix;

            layout (location = 0) in vec3 vertex;
            layout (location = 1) in vec2 in_texcoord0;
            layout (location = 2) in vec4 in_col0;

            layout (location = 0) out vec2 texcoord0;
            layout (location = 1) out vec4 col0;
 
            void main() {
                gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertex, 1.0);
                texcoord0 = in_texcoord0;
                col0 = in_col0;
            }
            ";
        private const string fragmentShaderSourceEmbossed =
            @"#version 450
            precision highp float;

            uniform sampler2D tex0;

            layout (location = 0) in vec2 texcoord0;
            layout (location = 1) in vec4 col0;

            layout (location = 0) out vec4 out_Colour;

            const float TEXEL = 1.0/256.0;

            vec3 getNormal(vec2 pos)
            {
                float h1 = texture2D(tex0,vec2(pos.x,pos.y-TEXEL)).a;
                float h2 = texture2D(tex0,vec2(pos.x,pos.y+TEXEL)).a;
                float h3 = texture2D(tex0,vec2(pos.x-TEXEL,pos.y)).a;
                float h4 = texture2D(tex0,vec2(pos.x+TEXEL,pos.y)).a;
                return normalize(vec3(h4-h3,h2-h1,2.0*TEXEL));
            }

            void main() {
                float t = texture2D(tex0,texcoord0.xy).a;
                vec4 col = col0;

                vec3 n = getNormal(texcoord0.xy);
                vec3 l = normalize(vec3(0.6,0.5,0.2));
                float diffuse = clamp(dot(n,l) * 0.5 + 0.5,0,1);
                col *= 0.5 + 0.5 * diffuse;
                col.a = 1.0;


                col.a = col.a * smoothstep(0.4,0.6,t);
                out_Colour = col;
            }
             ";
        private const string fragmentShaderSourceNormal =
            @"#version 450
            precision highp float;

            uniform sampler2D tex0;

            layout (location = 0) in vec2 texcoord0;
            layout (location = 1) in vec4 col0;

            layout (location = 0) out vec4 out_Colour;

            void main() {
                float t = texture2D(tex0,texcoord0.xy).a;
                vec4 col = col0;
                col.a = col.a * smoothstep(0.4,0.6,t);
                out_Colour = col;
            }
             ";
        private const string fragmentShaderSourceShadow =
            @"#version 450
            precision highp float;

            uniform sampler2D tex0;

            layout (location = 0) in vec2 texcoord0;
            layout (location = 1) in vec4 col0;

            layout (location = 0) out vec4 out_Colour;
            
            void main() {

                vec2 shadowOffset = vec2(0.003,0.003);
                float shadow = texture2D(tex0,texcoord0.xy - shadowOffset).a;

                float t = texture2D(tex0,texcoord0.xy).a;

                vec4 shadowcol = vec4(0.0,0.0,1.0,1.0);
                shadowcol.a * smoothstep(0.4,0.6,shadow);
                shadow = max(0.0,shadow - t);

                vec4 col = col0;
                col.a = col.a * smoothstep(0.4,0.6,t);

                col = mix(col, shadowcol, shadow);

                out_Colour = col;
            }
             ";

        private const string fragmentShaderSource =
            @"#version 450
            precision highp float;

            uniform sampler2D tex0;

            layout (location = 0) in vec2 texcoord0;
            layout (location = 1) in vec4 col0;

            layout (location = 0) out vec4 out_Colour;

            const float TEXEL = 1.0/512.0;

            float samplePos(vec2 p){

                float t = 0.0;
                t += texture2D(tex0,p).a;
                //t += texture2D(tex0,p + vec2(TEXEL,0.0)).a;
                //t += texture2D(tex0,p + vec2(-TEXEL,0.0)).a;
                //t += texture2D(tex0,p + vec2(0.0,TEXEL)).a;
                //t += texture2D(tex0,p + vec2(0.0,-TEXEL)).a;
                //t /= 5.0;    
                return t;            
            }

            void main() {

                float t = samplePos(texcoord0.xy);
                vec4 col = col0;
                
                vec4 colBorder = vec4(0.0,0.0,0.0,0.4);
                //vec4 colBorder = vec4(col0.rgb * 0.5,0.4);

                //col = mix(colBorder,col,smoothstep(0.45,0.7,t));


                col.a = col.a * smoothstep(0.5,0.7,t);
                out_Colour = col;
            }
             ";

        #endregion


        public TextRenderer(string name, Font font)
        {
            Name = name;
            RendererFont = font;

            Count = 0;
            GlobalScale = 1.0f;

            ProjectionMatrix = ModelMatrix = ViewMatrix = Matrix4.Identity;

            InitVertexVBOData();
            InitTexcoordVBOData();
            InitColourVBOData();

            Resources.Add(vertexVBO = vertex.ToBufferObject(Name + "_vertex", BufferTarget.ArrayBuffer, BufferUsageHint.DynamicDraw));
            Resources.Add(colourVBO = colour.ToBufferObject(Name + "_colour", BufferTarget.ArrayBuffer, BufferUsageHint.DynamicDraw));
            Resources.Add(texcoordVBO = texcoord.ToBufferObject(Name + "_texcoord", BufferTarget.ArrayBuffer, BufferUsageHint.DynamicDraw));
            Resources.Add(indexVBO = GetIndexVBOData().ToBufferObject(Name + "_index", BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw));

            Resources.Add(shader = new ShaderProgram(Name, "vertex,in_texcoord0,in_col0", "", false, vertexShaderSource, fragmentShaderSource));
        }

        public TextRenderer(Font font)
            : this("TextRenderer", font)
        {
        }


        public uint[] GetIndexVBOData()
        {
            uint[] index = new uint[NUMINDICES];
            int i = 0;

            // indices are static. We'll move vertices and texcoords.
            for (int c = 0; c < MAXCHARS; c++)
            {
                uint c4 = (uint)c * 4;

                index[i++] = c4 + 0;
                index[i++] = c4 + 1;
                index[i++] = c4 + 2;
                index[i++] = c4 + 1;
                index[i++] = c4 + 3;
                index[i++] = c4 + 2;
            }
            return index;
        }

        public void InitVertexVBOData()
        {
            vertex = new Vector3[NUMVERTICES];

            for (int i = 0; i < NUMVERTICES; i++)
            {
                vertex[i] = Vector3.Zero;
            }
        }

        public void InitColourVBOData()
        {
            colour = new Vector4[NUMVERTICES];

            for (int i = 0; i < NUMVERTICES; i++)
            {
                colour[i] = Vector4.One;
            }
        }

        public void InitTexcoordVBOData()
        {
            texcoord = new Vector2[NUMVERTICES];

            for (int i = 0; i < NUMVERTICES; i++)
            {
                texcoord[i] = Vector2.Zero;
            }
        }

        public void Refresh()
        {
            vertexVBO.SetData(vertex);
            colourVBO.SetData(colour);
            texcoordVBO.SetData(texcoord);
        }


        /// <summary>
        /// adds a character to the render list, returns cursor advance amount
        /// </summary>
        /// <param name="c"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public float AddChar(char c, float x, float y, float z, float size, Vector4 col)
        {
            FontCharacter charinfo;
            size *= GlobalScale;

            if (Count < MAXCHARS && RendererFont.Characters.TryGetValue(c, out charinfo))
            {

                int i = Count * 4;  // offset into vertex VBO data

                // top left
                vertex[i].X = x + (charinfo.XOffset * size);
                vertex[i].Y = y + (-charinfo.YOffset * size);
                vertex[i].Z = z;
                colour[i] = col;
                texcoord[i] = charinfo.TexTopLeft;
                i++;

                // top right
                vertex[i].X = x + (charinfo.XOffset + charinfo.Width) * size;
                vertex[i].Y = y + (-charinfo.YOffset * size);
                vertex[i].Z = z;
                colour[i] = col;
                texcoord[i] = charinfo.TexTopRight;
                i++;

                // bottom left
                vertex[i].X = x + (charinfo.XOffset * size);
                vertex[i].Y = y + (-charinfo.YOffset + charinfo.Height) * size;
                vertex[i].Z = z;
                colour[i] = col;
                texcoord[i] = charinfo.TexBottomLeft;
                i++;

                // bottom right
                vertex[i].X = x + (charinfo.XOffset + charinfo.Width) * size;
                vertex[i].Y = y + (-charinfo.YOffset + charinfo.Height) * size;
                vertex[i].Z = z;
                colour[i] = col;
                texcoord[i] = charinfo.TexBottomRight;

                Count++;

                return charinfo.XAdvance * size;
            }
            return 0f;
        }

        public float AddChar(char c, Vector3 position, float size, Vector4 col)
        {
            return AddChar(c, position.X, position.Y, position.Z, size, col);
        }

        public float AddString(string s, float x, float y, float z, float size, Vector4 col)
        {
            float xx = x;
            foreach (char c in s)
            {
                xx += AddChar(c, xx, y, z, size, col);
            }
            return xx - x;
        }

        public float AddString(string s, Vector3 position, float size, Vector4 col)
        {
            return AddString(s, position.X, position.Y, position.Z, size, col);
        }


        public void Render(IFrameRenderData frameData) 
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            RendererFont.Bind(TextureUnit.Texture0);

            shader.UseProgram();
            shader.SetUniform("projection_matrix", ProjectionMatrix);
            shader.SetUniform("view_matrix", ViewMatrix);
            shader.SetUniform("model_matrix", ModelMatrix);
            shader.SetUniform("tex0", 0);
            vertexVBO.Bind(shader.VariableLocations["vertex"]);
            colourVBO.Bind(shader.VariableLocations["in_col0"]);
            texcoordVBO.Bind(shader.VariableLocations["in_texcoord0"]);
            indexVBO.Bind();

            GL.DrawElements(BeginMode.Triangles, Count * 6, DrawElementsType.UnsignedInt, 0);
        }

        public void Clear()
        {
            Count = 0;
        }

        public void Resize(int width, int height)
        {
            if (height > 0)
            {
                ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0.0f, (float)width / (float)height, 1.0f, 0.0f, 0.001f, 10.0f);
                ViewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -1.0f);
                ModelMatrix = Matrix4.Identity;  // TODO: should scale go in here?
            }
            else
            {
                ProjectionMatrix = ModelMatrix = ViewMatrix = Matrix4.Identity;
            }
        }

    }
}
