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
        object Value { get; set; }
    }

    /*
    public interface IEffectPort
    {
        string Name { get; }

        object Value { get; }

        bool IsSet { get; }

        void Reset();
    }

    public interface IEffectPort<T>
    {
        T Value { get; }
    }
    */
}
