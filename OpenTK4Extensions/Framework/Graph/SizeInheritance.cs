using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    /// <summary>
    /// How a child component should behave when the parent is resized.
    /// </summary>
    public enum SizeInheritance
    {
        /// <summary>
        /// No size inheritance
        /// </summary>
        None = 0,
        /// <summary>
        /// Inherit both width and height
        /// </summary>
        Inherit,
        /// <summary>
        /// Inherit both width and height, half resolution
        /// </summary>
        InheritHalf,
        /// <summary>
        /// Inherit both width and height, quarter resolution
        /// </summary>
        InheritQuarter,
        /// <summary>
        /// Inherit both width and height, eighth resolution
        /// </summary>
        InheritEighth,
        /// <summary>
        /// Square texture, inherit smaller dimension
        /// </summary>
        SquareInheritSmallest,
        /// <summary>
        /// Square texture, inherit larger dimension
        /// </summary>
        SquareInheritLargest,
        /// <summary>
        /// Square texture, inherit smaller dimension nearest higher power-of-two.
        /// </summary>
        SquareInheritSmallestPow2,
        /// <summary>
        /// Square texture, inherit larger dimension nearest higher power-of-two.
        /// </summary>
        SquareInheritLargestPow2,
    }
}
