using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    public class GraphNodePort<T> : GraphNodePort, IPort where T : class
    {
        public T InternalValue { get; set; }

        public override Type PortType
        {
            get { return typeof(T); }
            set { }
        }

        public override object Value
        {
            get
            {
                return InternalValue;
            }
            set
            {
                InternalValue = value is T v ? v : throw new InvalidCastException($"Value supplied for port {Name} must be a {typeof(T).Name}.");
            }

        }
    }
}
