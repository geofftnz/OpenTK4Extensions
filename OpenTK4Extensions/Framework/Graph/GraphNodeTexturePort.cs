using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    public class GraphNodeTexturePort : GraphNodePort<Texture>, ITexturePort
    {
        public TextureTarget Target { get; set; }

        public PixelFormat Format { get; set; }
    }
}
