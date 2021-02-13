using OpenTKExtensions.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Components.ParticleSystem.Operators
{
    public class PosVelColOperator : OperatorComponentBase
    {
        public PosVelColOperator() : base("posvelcol_operator.vert.glsl", "posvelcol_operator.frag.glsl")
        {

        }

    }
}
