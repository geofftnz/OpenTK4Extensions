using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Text
{
    public interface IFont
    {
        Dictionary<char, FontCharacter> Characters { get; }
        void Bind(TextureUnit textureUnit);
    }
}
