using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework
{
    /// <summary>
    /// Represents an input/output port to an effect.
    /// 
    /// 
    /// 
    /// </summary>
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

}
