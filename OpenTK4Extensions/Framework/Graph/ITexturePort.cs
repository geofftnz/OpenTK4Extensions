using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    public interface ITexturePort
    {
        TextureTarget Target { get; }
        PixelFormat Format { get; }
    }
}
