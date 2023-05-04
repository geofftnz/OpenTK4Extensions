using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTKExtensions.Framework.Graph;
using Xunit;

namespace OpenTK4Extensions.Test.Framework.Graph
{
    public class SizeInheritanceHelperTest
    {
        public static IEnumerable<object[]> SizeInheritanceHelperTestData => new List<object[]>
        {
            new object[] { SizeInheritance.None, new Vector2i(256, 256), new Vector2i(200,100), new Vector2i(256,256) },
            new object[] { SizeInheritance.Inherit,new Vector2i(256, 256), new Vector2i(200,100), new Vector2i(200,100) },
            new object[] { SizeInheritance.InheritHalf,new Vector2i(256, 256), new Vector2i(200,100), new Vector2i(100,50) },
            new object[] { SizeInheritance.InheritQuarter,new Vector2i(256, 256), new Vector2i(200,100), new Vector2i(50,25) },
            new object[] { SizeInheritance.InheritEighth,new Vector2i(256, 256), new Vector2i(200,100), new Vector2i(25,12) },
            new object[] { SizeInheritance.SquareInheritSmallest,new Vector2i(256, 256), new Vector2i(200,100), new Vector2i(100,100) },
            new object[] { SizeInheritance.SquareInheritLargest,new Vector2i(256, 256), new Vector2i(200,100), new Vector2i(200,200) },
            new object[] { SizeInheritance.SquareInheritSmallestPow2,new Vector2i(256, 256), new Vector2i(200,100), new Vector2i(128,128) },
            new object[] { SizeInheritance.SquareInheritLargestPow2,new Vector2i(512, 512), new Vector2i(200,100), new Vector2i(256,256) },
        };

        [Theory]
        [MemberData(nameof(SizeInheritanceHelperTestData))]
        public void calculation_is_correct(SizeInheritance method, Vector2i current, Vector2i parent, Vector2i expected)
        {
            Assert.Equal(expected, parent.InheritUsing(method, current));
        }

        [Fact]
        public void non_vector_passthrough()
        {
            int X = 200, Y = 100;
            int newX, newY;

            (newX, newY) = SizeInheritance.InheritHalf.InheritFrom(X, Y);
            Assert.Equal(100, newX);
            Assert.Equal(50, newY);
        }
    }
}
