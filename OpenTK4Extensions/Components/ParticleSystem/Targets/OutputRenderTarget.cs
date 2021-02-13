using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Components.ParticleSystem.Targets
{
    public class OutputRenderTarget : RenderTargetBase
    {
        protected Texture renderTexture;
        public OutputRenderTarget(bool wantDepth = false, bool inheritSize = true, int width = 256, int height = 256) : base(wantDepth, inheritSize, width, height)
        {
            Resources.Add(renderTexture = new Texture(256, 256, TextureTarget.Texture2D, PixelInternalFormat.Rgba16f, PixelFormat.Rgba, PixelType.HalfFloat));
            SetOutput(0, renderTexture);
        }
    }
}
