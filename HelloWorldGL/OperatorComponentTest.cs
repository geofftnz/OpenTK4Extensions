using OpenTK.Mathematics;
using OpenTKExtensions.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldGL
{
    public class OperatorComponentTest : OperatorComponentBase, IRenderable, IReloadable, ITransformable
    {
        public Matrix4 ViewMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ProjectionMatrix { get; set; } = Matrix4.Identity;

        public OperatorComponentTest() : base("operatorcomponenttest.vert.glsl", "operatorcomponenttest.frag.glsl")
        {
            TextureBinds = () => { };

            SetShaderUniforms = (sp) =>
            {
                if (sp != null)
                {
                    sp
                    .SetUniform("projectionMatrix", ProjectionMatrix)
                    .SetUniform("modelMatrix", ModelMatrix)
                    .SetUniform("viewMatrix", ViewMatrix);
                }
            };

        }
    }
}
