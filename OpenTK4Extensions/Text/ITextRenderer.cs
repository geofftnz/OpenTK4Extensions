using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
namespace OpenTKExtensions.Text
{
    interface ITextRenderer
    {
        float AddChar(char c, float x, float y, float z, float size, Vector4 col);
        float AddString(string s, float x, float y, float z, float size, Vector4 col);
    }
}
