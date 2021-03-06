﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKExtensions
{
    public interface ITextureParameter
    {
        TextureParameterName ParameterName { get; set; }
        void Apply(TextureTarget target);
    }
}
