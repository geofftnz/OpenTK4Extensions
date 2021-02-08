using OpenTK.Mathematics;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Image;
using OpenTKExtensions.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKExtensions.Text
{
    /// <summary>
    /// Split from (now called) TextRenderer, so that a single font can be used by multiple text renderers.
    /// </summary>
    public class Font : GameComponentBase, IFont
    {
        public string Name { get; set; }

        public int TexWidth { get; private set; }
        public int TexHeight { get; private set; }

        private Texture sdfTexture;

        private Dictionary<char, FontCharacter> characters = new Dictionary<char, FontCharacter>();
        public Dictionary<char, FontCharacter> Characters
        {
            get
            {
                return characters;
            }
        }

        public Font(string name, string imageFilename, string metadataFilename)
        {
            // create resources
            LoadTexture(imageFilename);
            LoadMetaData(metadataFilename);
            NormalizeTexcoords();
        }

        public Font(string imageFilename, string metadataFilename) : this("Font0", imageFilename, metadataFilename)
        {

        }


        public void LoadMetaData(string fileName)
        {
            LogTrace($"Font {Name} loading meta-data from {fileName}");
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                LoadMetaData(fs);
            }
        }
        public void LoadMetaData(Stream input)
        {
            Characters.Clear();

            using (var sr = new StreamReader(input))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var tempChar = new FontCharacter();

                    if (FontMetaParser.TryParseCharacterInfoLine(line, out tempChar))
                    {
                        char key = (char)tempChar.ID;

                        if (!Characters.ContainsKey(key))
                        {
                            Characters.Add(key, tempChar);
                        }
                    }
                }
            }
            LogTrace($"Font {Name} meta data loaded. {Characters.Count} characters parsed.");
        }

        public void LoadTexture(string fileName)
        {
            LogTrace($"Font {Name} loading texture from {fileName}");

            ImageLoader.ImageInfo info;

            // load red channel from file.
            var data = fileName.LoadImage(out info).ExtractChannelFromRGBA(3);

            TexWidth = info.Width;
            TexHeight = info.Height;

            // setup texture

            sdfTexture = new Texture(Name + "_tex", info.Width, info.Height, TextureTarget.Texture2D, PixelInternalFormat.Alpha, PixelFormat.Alpha, PixelType.UnsignedByte);
            sdfTexture
                .Set(TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
                .Set(TextureParameterName.TextureMinFilter, TextureMinFilter.LinearMipmapLinear)
                .Set(TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
                .Set(TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge);

            sdfTexture.ReadyForContent += (s, e) =>
            {
                sdfTexture.Upload(data);
                sdfTexture.Bind();
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            };

            Resources.Add(sdfTexture);

            LogTrace($"Font {Name} texture loaded, resolution {TexWidth}x{TexHeight}");
        }

        public void NormalizeTexcoords()
        {
            foreach (var c in Characters)
            {
                c.Value.NormalizeTexcoords((float)TexWidth, (float)TexHeight);
            }
        }

        public Vector3 MeasureChar(char c, float size, float scale = 1.0f)
        {
            FontCharacter charinfo;
            size *= scale;
            if (Characters.TryGetValue(c, out charinfo))
            {
                return new Vector3(charinfo.XAdvance * size, charinfo.Height * size, 0f);
            }
            return Vector3.Zero;
        }

        public Vector3 MeasureString(string s, float size)
        {
            Vector3 r = Vector3.Zero;

            foreach (char c in s)
            {
                var charsize = MeasureChar(c, size);
                r.X += charsize.X;
                if (r.Y < charsize.Y)
                {
                    r.Y = charsize.Y;
                }
                if (r.Z < charsize.Z)
                {
                    r.Z = charsize.Z;
                }

            }

            return r;
        }

        public void Bind(TextureUnit textureUnit)
        {
            sdfTexture.Bind(textureUnit);
        }

    }
}
