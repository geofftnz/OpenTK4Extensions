using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{

    /// <summary>
    /// Represents a collection of components connected in a graph, with outputs of one component used as inputs for another.
    /// </summary>
    public class ComponentGraph : CompositeGameComponent
    {

        public List<GraphEdge> GraphEdges { get; } = new();
        private IRenderGraphNode rootNode = null;
        private Dictionary<string, IRenderGraphNode> nodeMap = new();

        public ComponentGraph() : base()
        {

        }

        public override void Add(IGameComponent component)
        {
            var node = component as IRenderGraphNode;

            if (node == null)
            {
                throw new InvalidOperationException("Nodes in a graph must be a node");
            }

            if (node.IsRoot)
            {
                if (rootNode == null)
                {
                    rootNode = node;
                }
                else
                {
                    throw new InvalidOperationException("Can't have more than one root node");
                }
            }

            nodeMap.Add(node.Name, node);

            base.Add(component);
        }

        public void AddEdge(NodePortReference from, NodePortReference to)
        {
            // check nodes
            if (!nodeMap.ContainsKey(from.Node))
            {
                throw new ArgumentException($"Node {from.Node} does not exist.");
            }
            if (!nodeMap.ContainsKey(to.Node))
            {
                throw new ArgumentException($"Node {to.Node} does not exist.");
            }

            var fromNode = nodeMap[from.Node];
            var toNode = nodeMap[to.Node];

            // check ports
            if (!fromNode.Output.ContainsKey(from.Port))
            {
                throw new ArgumentException($"Output port {from.Port} is not present on source node.");
            }
            if (!toNode.Input.ContainsKey(to.Port))
            {
                throw new ArgumentException($"Input port {to.Port} is not present on target node.");
            }

            // check port compatibility
            if (!fromNode.Output[from.Port].CanBindWith(toNode.Input[to.Port]))
            {
                throw new ArgumentException($"Ports are not compatible.");
            }

            // make sure we don't already have this edge
            if (GraphEdges.Any(e => e.FromNode == fromNode && e.FromPort == from.Port && e.ToNode == toNode && e.ToPort == to.Port))
            {
                throw new ArgumentException("Edge already exists.");
            }

            GraphEdges.Add(new GraphEdge { FromNode = fromNode, FromPort = from.Port, ToNode = toNode, ToPort = to.Port });
        }

        public void RemoveEdge(NodePortReference from, NodePortReference to)
        {
            GraphEdges.RemoveAll(e =>
                e.FromNode.Name == from.Node &&
                e.FromPort == from.Port &&
                e.ToNode.Name == to.Node &&
                e.ToPort == to.Port
            );
        }

        public override void Update(IFrameUpdateData frameData)
        {
            // traverse graph

            base.Update(frameData);
        }
        public override void Render(IFrameRenderData frameData)
        {
            // clear render flags
            Components.Do<IRenderGraphNode>(n => n.HasRendered = false);

            RenderNode(rootNode, frameData);
        }

        protected void RenderNode(IRenderGraphNode node, IFrameRenderData frameData)
        {
            if (node == null)
                return;

            // ensure dependencies are rendered
            foreach (var edge in GraphEdges.Where(e => e.ToNode == node))
            {
                if (!edge.FromNode.HasRendered)
                {
                    RenderNode(edge.FromNode, frameData);
                }

                // set input port
                node.Input[edge.ToPort].Value = edge.FromNode.Output[edge.FromPort].Value;
            }

            // render this node
            node.Render(frameData);
            node.HasRendered = true;

        }

    }
}
