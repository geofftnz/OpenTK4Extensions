using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Components.ParticleSystem.Models
{
    /// <summary>
    /// Exists to hold the double-buffered textures for a particle system
    /// </summary>
    public class ParamParticleModel : GameComponentBase, IListTextures
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public bool Visible { get; set; } = false;
        public int DrawOrder { get; set; } = int.MaxValue;

        public Texture ParticlePositionPrevious { get { return particlePosition.PreviousTexture; } }
        public Texture ParticlePositionRead { get { return particlePosition.ReadTexture; } }
        public Texture ParticlePositionWrite { get { return particlePosition.WriteTexture; } }

        public Texture ParticleColourRead { get { return particleColour.ReadTexture; } }
        public Texture ParticleColourWrite { get { return particleColour.WriteTexture; } }

        public Texture ParticleParamRead { get { return particleParam.ReadTexture; } }
        public Texture ParticleParamWrite { get { return particleParam.WriteTexture; } }

        private TripleBufferedTexture particlePosition;
        private DoubleBufferedTexture particleColour;
        private DoubleBufferedTexture particleParam;

        public ParamParticleModel(int width, int height) : base()
        {
            Width = width;
            Height = height;

            Resources.Add(particlePosition = new TripleBufferedTexture("particlepos", Width, Height, TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, Texture.Params().FilterNearest().ClampToEdge().ToArray()));

            Resources.Add(particleColour = new DoubleBufferedTexture("particlecol", Width, Height, TextureTarget.Texture2D, PixelInternalFormat.Rgba16f, PixelFormat.Rgba, PixelType.HalfFloat, Texture.Params().FilterNearest().ClampToEdge().ToArray()));
            
            Resources.Add(particleParam = new DoubleBufferedTexture("particleparam", Width, Height, TextureTarget.Texture2D, PixelInternalFormat.Rgba16f, PixelFormat.Rgba, PixelType.HalfFloat, Texture.Params().FilterNearest().ClampToEdge().ToArray()));
        }

        public void SwapBuffers()
        {
            particlePosition.Swap();
            particleColour.Swap();
            particleParam.Swap();
        }

        public IEnumerable<Texture> Textures()
        {
            yield return particlePosition.ReadTexture;
            yield return particleColour.ReadTexture;
            yield return particleParam.ReadTexture;
        }
    }
}
