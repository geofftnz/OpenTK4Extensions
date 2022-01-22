using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    public class NodePortReference
    {
        public string Node;
        public int Port;

        public override bool Equals(object obj)
        {
            return obj is NodePortReference reference &&
                   Node == reference.Node &&
                   Port == reference.Port;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Node, Port);
        }
    }
}
