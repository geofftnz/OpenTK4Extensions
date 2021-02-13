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
    public class DefaultParticleModel : GameComponentBase, IListTextures
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

        private TripleBufferedTexture particlePosition;
        private DoubleBufferedTexture particleColour;

        public DefaultParticleModel(int width, int height) : base()
        {
            Width = width;
            Height = height;

            Resources.Add(particlePosition = new TripleBufferedTexture("particlepos", Width, Height, TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, Texture.Params().FilterNearest().ClampToEdge().ToArray()));

            Resources.Add(particleColour = new DoubleBufferedTexture("particlecol", Width, Height, TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, Texture.Params().FilterNearest().ClampToEdge().ToArray()));
        }

        public void SwapBuffers()
        {
            particlePosition.Swap();
            particleColour.Swap();
        }

        public IEnumerable<Texture> Textures()
        {
            yield return particlePosition.ReadTexture;
            yield return particleColour.ReadTexture;
        }
    }
}
