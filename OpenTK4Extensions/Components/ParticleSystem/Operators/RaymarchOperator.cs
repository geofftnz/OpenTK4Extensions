using OpenTKExtensions.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Components.ParticleSystem.Operators
{
    public class RaymarchOperator : OperatorComponentBase
    {
        public RaymarchOperator() : base("raymarch_operator.vert.glsl", "raymarch_operator.frag.glsl")
        {

        }

    }
}
