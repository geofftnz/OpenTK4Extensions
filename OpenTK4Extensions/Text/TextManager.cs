using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using OpenTK;
using OpenTK.Mathematics;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Resources;

namespace OpenTKExtensions.Text
{
    public class TextManager : CompositeGameComponent, IRenderable, IResizeable, IUpdateable, ITransformable
    {
        public string Name { get; set; }
        public TextRenderer Renderer { get; private set; }
        public bool NeedsRefresh { get; private set; }

        Dictionary<string, TextBlock> blocks = new Dictionary<string, TextBlock>();
        public Dictionary<string, TextBlock> Blocks { get { return blocks; } }

        public Matrix4 ViewMatrix { get => Renderer.ViewMatrix; set => Renderer.ViewMatrix = value; }
        public Matrix4 ModelMatrix { get => Renderer.ModelMatrix; set => Renderer.ModelMatrix = value; }
        public Matrix4 ProjectionMatrix { get => Renderer.ProjectionMatrix; set => Renderer.ProjectionMatrix = value; }

        public TextManager(string name, Font font)
        {
            Name = name;
            NeedsRefresh = false;
            Visible = true;
            DrawOrder = int.MaxValue;
            IsFinalOutput = true;

            this.Components.Add(Renderer = new TextRenderer(font));
        }

        public void Clear()
        {
            Blocks.Clear();
            NeedsRefresh = true;
        }

        public bool Add(TextBlock b)
        {
            if (!Blocks.ContainsKey(b.Name))
            {
                Blocks.Add(b.Name, b);
                NeedsRefresh = true;
                return true;
            }
            return false;
        }

        public void AddOrUpdate(TextBlock b)
        {
            if (!Add(b))
            {
                Blocks[b.Name] = b;
                NeedsRefresh = true;
            }
        }

        public bool Remove(string blockName)
        {
            if (Blocks.ContainsKey(blockName))
            {
                Blocks.Remove(blockName);
                NeedsRefresh = true;
                return true;
            }
            return false;
        }
        public bool RemoveAllByPrefix(string blockNamePrefix)
        {
            bool hit = false;

            foreach (var blockToRemove in Blocks.Keys.Where(n => n.StartsWith(blockNamePrefix)).ToList())
            {
                Blocks.Remove(blockToRemove);
                hit = true;
            }

            if (hit)
            {
                NeedsRefresh = true;
                return true;
            }
            return false;
        }


        public void Refresh()
        {
            //LogTrace($"({Name}): Refreshing {Blocks.Count} blocks...");

            if (Renderer == null)
            {
                LogWarn($"({Name}): Font not specified so bailing out.");
                return;
            }

            // refresh character arrays
            Renderer.Clear();

            foreach (var b in Blocks.Values)
            {
                Renderer.AddString(b.Text, b.Position, b.Size, b.Colour);
            }

            Renderer.Refresh();
            NeedsRefresh = false;
        }

        public override void Render(IFrameRenderData frameData, IFrameBufferTarget target = null)
        {
            if (NeedsRefresh)
            {
                Refresh();
            }

            base.Render(frameData, target);
        }

    }
}
