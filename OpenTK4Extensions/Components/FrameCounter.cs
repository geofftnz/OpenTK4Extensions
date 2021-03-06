﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Text;
using OpenTK;
using OpenTK.Mathematics;

namespace OpenTKExtensions.Components
{
    public class FrameCounter : CompositeGameComponent, IRenderable, IResizeable
    {
        const int BUFLEN = 30;
        private Stopwatch sw = new Stopwatch();
        private double[] tickBuffer = new double[BUFLEN];
        private int bufferPos = 0;
        private long frameCount = 0;

        public float TextSize { get { return textBlock.Size; } set { textBlock.Size = value; } }
        public Vector3 Position { get { return textBlock.Position; } set { textBlock.Position = value; } }
        public Vector4 Colour { get { return textBlock.Colour; } set { textBlock.Colour = value; } }

        public long FrameCount { get { return frameCount; } }

        //public bool Visible { get; set; }
        //public int DrawOrder { get; set; }

        private double fpsSmoothed = 0.0;
        private double fpsLowpassAmount = 1.0;

        private TextManager textManager;

        private TextBlock textBlock = new TextBlock("fps", "", new Vector3(0.01f, 0.06f, 0.0f), 0.001f, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
        public TextBlock TextBlock
        {
            get
            {
                textBlock.Text = string.Format("{0:0}", FPS);
                return textBlock;
            }
        }


        public double FPS
        {
            get
            {
                var ts = tickBuffer[bufferPos] - tickBuffer[(bufferPos + 1) % BUFLEN];

                if (ts > 0.0)
                {
                    return (double)BUFLEN / ts;
                }
                return 0.0;
            }
        }

        public double FPSSmooth
        {
            get
            {
                return fpsSmoothed;
            }
        }

        public double TotalSeconds
        {
            get
            {
                return sw.Elapsed.TotalSeconds;
            }
        }

        public long Frames
        {
            get
            {
                return frameCount;
            }
        }


        public FrameCounter(Font font)
        {
            this.Visible = true;
            this.DrawOrder = 1000;

            components.Add(textManager = new TextManager("fps", font));

            this.Loading += FrameCounter_Loading;

        }

        void FrameCounter_Loading(object sender, EventArgs e)
        {
            Start();
        }

        public void Start()
        {
            frameCount = 0;
            sw.Start();

            double ticks = sw.Elapsed.TotalSeconds;

            for (int i = 0; i < BUFLEN; i++)
            {
                tickBuffer[i] = ticks;
            }
        }

        public void Stop()
        {
            sw.Stop();
            sw.Reset();
        }

        public void Frame()
        {
            frameCount++;
            bufferPos++;
            bufferPos %= BUFLEN;
            tickBuffer[bufferPos] = sw.Elapsed.TotalSeconds;

            var f = this.FPS;
            this.fpsSmoothed = f * this.fpsLowpassAmount + (1.0 - this.fpsLowpassAmount) * this.fpsSmoothed;
        }

        public override void Render(IFrameRenderData frameData)
        {
            this.Frame();
            textManager.AddOrUpdate(this.TextBlock);

            base.Render(frameData);
        }



    }
}
