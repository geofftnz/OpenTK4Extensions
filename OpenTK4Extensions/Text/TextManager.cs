using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using OpenTK;
using OpenTK.Mathematics;
using OpenTKExtensions.Framework;

namespace OpenTKExtensions.Text
{
    public class TextManager : CompositeGameComponent, IRenderable, IResizeable, IUpdateable
    {
        public string Name { get; set; }
        public TextRenderer Renderer { get; private set; }
        public bool NeedsRefresh { get; private set; }

        Dictionary<string, TextBlock> blocks = new Dictionary<string, TextBlock>();
        public Dictionary<string, TextBlock> Blocks { get { return blocks; } }

        public TextManager(string name, Font font)
        {
            Name = name;
            NeedsRefresh = false;
            Visible = true;
            DrawOrder = int.MaxValue;

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

        public override void Render(IFrameRenderData frameData)
        {
            if (NeedsRefresh)
            {
                Refresh();
            }

            base.Render(frameData);
        }

    }
}
