﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Framework
{
    public interface ITimedComponent
    {
        TimeSpan LastRenderTime { get; }
        void StartRenderTimer();
        void StopRenderTimer();
    }
}
