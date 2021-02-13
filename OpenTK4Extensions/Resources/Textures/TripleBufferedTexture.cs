using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{

    /// <summary>
    /// Represents three identical textures in a prev/read/write set.
    /// These can be swapped at the end of each frame.
    /// 
    /// Prev -> Read -> Write
    /// </summary>
    public class TripleBufferedTexture : ResourceBase, IResource
    {
        private const int NUMTEXTURES = 3;

        public int Width { get { return Get(t => t.Width); } }
        public int Height { get { return Get(t => t.Height); } }
        public TextureTarget Target { get { return Get(t => t.Target); } }
        public PixelInternalFormat InternalFormat { get { return Get(t => t.InternalFormat); } }
        public PixelFormat Format { get { return Get(t => t.Format); } }
        public PixelType Type { get { return Get(t => t.Type); } }
        public bool LoadEmpty
        {
            get
            {
                return Get(t => t.LoadEmpty);
            }
            set
            {
                OnAll(t => t.LoadEmpty = value);
            }
        }

        protected int currentWriteTarget = 1;
        protected int currentReadTarget { get { return (currentWriteTarget - 1 + NUMTEXTURES) % NUMTEXTURES; } }
        protected int currentPreviousTarget { get { return (currentWriteTarget - 2 + NUMTEXTURES) % NUMTEXTURES; } }

        public Texture PreviousTexture { get { return Textures[currentPreviousTarget]; } }
        public Texture ReadTexture { get { return Textures[currentReadTarget]; } }
        public Texture WriteTexture { get { return Textures[currentWriteTarget]; } }

        protected Texture[] Textures;

        public TripleBufferedTexture(string name, int width, int height, TextureTarget target, PixelInternalFormat internalFormat, PixelFormat format, PixelType type, params ITextureParameter[] texParams) : base(name)
        {
            Textures = new Texture[NUMTEXTURES];
            for (int i = 0; i < NUMTEXTURES; i++)
            {
                Textures[i] = new Texture($"{name}_{i}", width, height, target, internalFormat, format, type, texParams);

                Textures[i].LoadEmpty = true;
            }
        }

        public TripleBufferedTexture(int width, int height, TextureTarget target, PixelInternalFormat internalFormat, PixelFormat format, PixelType type)
            : this("unnamed", width, height, target, internalFormat, format, type)
        {
        }

        public TripleBufferedTexture(string name, Texture t0, Texture t1, Texture t2) : base(name)
        {
            if (t0 == null)
                throw new ArgumentNullException("t0");

            if (t1 == null)
                throw new ArgumentNullException("t1");

            if (t2 == null)
                throw new ArgumentNullException("t2");

            // TODO: check that t1 & t2 are identical
            Textures = new Texture[NUMTEXTURES];
            Textures[0] = t0;
            Textures[1] = t1;
            Textures[2] = t2;
        }
        public TripleBufferedTexture(string name, Func<Texture> textureFactory)
        {
            if (textureFactory == null)
                throw new ArgumentNullException("textureFactory");

            Textures = new Texture[NUMTEXTURES];
            for (int i = 0; i < NUMTEXTURES; i++)
            {
                Textures[i] = textureFactory();
                Textures[i].LoadEmpty = true;
            }
        }

        public void Swap()
        {
            currentWriteTarget = (currentWriteTarget + 1) % NUMTEXTURES;
        }

        public void Load()
        {
            OnAll(t => t.Load());
        }

        public void Unload()
        {
            OnAll(t => t.Unload());
        }

        #region set operations
        private void OnAll(Action<Texture> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            for (int i = 0; i < NUMTEXTURES; i++)
                if (Textures[i] != null)
                    action(Textures[i]);
        }

        private T Get<T>(Func<Texture, T> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            if (Textures[0] == null)
                throw new InvalidOperationException("Textures not initialised");

            return func(Textures[0]);
        }
        private T Get<T>(Func<Texture, T> func, T defaultValue)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            if (Textures[0] == null)
                return defaultValue;

            return func(Textures[0]);
        }
        #endregion
    }
}
