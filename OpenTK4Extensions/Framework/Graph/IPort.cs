using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    /// <summary>
    /// Represents a single input or output in a render node.
    /// </summary>
    public interface IPort
    {
        string Name { get; }
        // type
        Type PortType { get; }

        bool CanBindWith(IPort other) => this.PortType == other.PortType;
    }
}
