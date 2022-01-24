using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    public interface IInputPort : IPort
    {
        object Value { set; }
    }
}
