using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    /// <summary>
    /// Represents a node in a render graph.
    /// 
    /// Requires:
    /// - list of inputs
    /// - list of outputs
    /// - indication whether this node has rendered this frame
    /// - whether this node is the root node
    /// 
    /// </summary>
    public interface IRenderGraphNode : IGameComponent, IRenderable
    {
        string Name { get; }

        IReadOnlyList<IPort> Input { get; }

        IReadOnlyList<IPort> Output { get; }

        /// <summary>
        /// This node is the root node of this render graph.
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// This node has been rendered this pass.
        /// </summary>
        bool HasRendered { get; set; }
    }
}
