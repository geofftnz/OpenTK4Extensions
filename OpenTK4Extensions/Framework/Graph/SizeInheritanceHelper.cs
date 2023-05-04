using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    public static class SizeInheritanceHelper
    {

        public static Vector2i InheritUsing(this Vector2i parent, SizeInheritance method, Vector2i current)
            => method switch
            {
                SizeInheritance.None => current,
                SizeInheritance.Inherit => parent,
                SizeInheritance.InheritHalf => parent / 2,
                SizeInheritance.InheritQuarter => parent / 4,
                SizeInheritance.InheritEighth => parent / 8,
                SizeInheritance.SquareInheritSmallest => new Vector2i(Math.Min(parent.X, parent.Y)),
                SizeInheritance.SquareInheritLargest => new Vector2i(Math.Max(parent.X, parent.Y)),
                SizeInheritance.SquareInheritSmallestPow2 => new Vector2i(MathHelper.NextPowerOfTwo(Math.Min(parent.X, parent.Y))),
                SizeInheritance.SquareInheritLargestPow2 => new Vector2i(MathHelper.NextPowerOfTwo(Math.Max(parent.X, parent.Y))),
                _ => throw new InvalidOperationException($"Unsupported method {method}"),
            };

        public static (int X, int Y) InheritFrom(this SizeInheritance method, int inX, int inY)
        {
            var result = new Vector2i(inX, inY).InheritUsing(method, Vector2i.One);
            return (result.X, result.Y);
        }
    }
}
