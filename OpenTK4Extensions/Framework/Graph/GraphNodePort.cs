using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    public class GraphNodePort : IPort
    {
        public virtual object Value { get; set; }

        public string Name { get; set; }

        public virtual Type PortType { get; set; }

    }
}
