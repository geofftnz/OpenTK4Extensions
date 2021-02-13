using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTKExtensions;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Resources;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKExtensions.Components.ParticleSystem.Renderers
{
    //TODO: finish this.
    /// <summary>
    /// 
    /// </summary>
    public class PostProcessRenderer : OperatorComponentBase
    {
        public PostProcessRenderer(bool usingFilenames = true, params Tuple<ShaderType, string>[] shaderSourceOrFilenames) : base(usingFilenames, shaderSourceOrFilenames)
        {
        }
    }
}
