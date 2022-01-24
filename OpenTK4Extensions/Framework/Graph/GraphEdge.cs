using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    public class GraphEdge
    {
        public IRenderGraphNode FromNode { get; set; }
        public string FromPort { get; set; }

        public IRenderGraphNode ToNode { get; set; }
        public string ToPort { get; set; }

        public override bool Equals(object obj)
        {
            return obj is GraphEdge edge &&
                   EqualityComparer<IRenderGraphNode>.Default.Equals(FromNode, edge.FromNode) &&
                   FromPort == edge.FromPort &&
                   EqualityComparer<IRenderGraphNode>.Default.Equals(ToNode, edge.ToNode) &&
                   ToPort == edge.ToPort;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FromNode, FromPort, ToNode, ToPort);
        }

        public override string ToString()
        {
            return $"{FromNode.Name}:{FromPort} -> {ToNode.Name}:{ToPort}";
        }
    }
}
