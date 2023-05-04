using OpenTK.Mathematics;
using OpenTKExtensions.Common;
using OpenTKExtensions.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    public abstract class RenderGraphNodeBase : WrappedGameComponentBase, IRenderGraphNode, IRenderable, IResizeable, IReloadable
    {

        public string Name { get; set; }

        public SizeInheritance InheritSizeFromParent { get; set; } = SizeInheritance.None;
        protected GBuffer OutputBuffer;

        protected Dictionary<string, GraphNodePort> _input { get; } = new();
        protected DictionaryProxy<string, IPort, GraphNodePort> _inputProxy;
        public IReadOnlyDictionary<string, IPort> Input => _inputProxy;


        protected Dictionary<string, GraphNodePort> _output { get; } = new();
        protected DictionaryProxy<string, IPort, GraphNodePort> _outputProxy;
        public IReadOnlyDictionary<string, IPort> Output => _outputProxy;

        public bool IsRoot { get; set; }

        public bool HasRendered { get; set; }

        public RenderGraphNodeBase(bool wantDepth = false, SizeInheritance inheritSize = SizeInheritance.None, int width = 256, int height = 256) : base()
        {
            InheritSizeFromParent = inheritSize;
            Resources.Add(OutputBuffer = new GBuffer("gbuffer", wantDepth, width, height));

            _inputProxy = new DictionaryProxy<string, IPort, GraphNodePort>(_input);
            _outputProxy = new DictionaryProxy<string, IPort, GraphNodePort>(_output);

        }


        protected abstract void AssignInputs(IFrameRenderData frameData);
        protected abstract void AssignOutputs(IFrameRenderData frameData);

        public override void Render(IFrameRenderData frameData, IFrameBufferTarget target)
        {
            if (Visible)
            {
                AssignInputs(frameData);
                (ChildComponent as IRenderable)?.Render(frameData, IsRoot ? target : OutputBuffer);
                AssignOutputs(frameData);
            }
        }


        public override void Resize(int width, int height)
        {
            (width, height) = InheritSizeFromParent.InheritFrom(width, height);

            if (InheritSizeFromParent != SizeInheritance.None)
            {
                OutputBuffer?.Resize(width, height);
            }
            base.Resize(width, height);
        }

        public void SetOutput(int index, TextureSlotParam texparam)
        {
            LogTrace($"{index} -> {texparam}");
            OutputBuffer.SetSlot(index, texparam);
        }
        public Texture GetTexture(int slot)
        {
            return OutputBuffer.GetTextureAtSlot(slot);
        }


    }
}
