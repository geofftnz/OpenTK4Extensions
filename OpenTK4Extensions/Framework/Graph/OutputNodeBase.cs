using OpenTKExtensions.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework.Graph
{
    public abstract class OutputNodeBase : CompositeGameComponent, IRenderGraphNode
    {
        public string Name { get; set; }

        protected Dictionary<string, GraphNodePort> _input { get; } = new();
        protected DictionaryProxy<string, IPort, GraphNodePort> _inputProxy;
        public IReadOnlyDictionary<string, IPort> Input => _inputProxy;


        protected Dictionary<string, GraphNodePort> _output { get; } = new();
        protected DictionaryProxy<string, IPort, GraphNodePort> _outputProxy;
        public IReadOnlyDictionary<string, IPort> Output => _outputProxy;

        public bool IsRoot { get; protected set; }

        public bool HasRendered { get; set; }

        public OutputNodeBase() : base()
        {
            IsRoot = true;
            IsFinalOutput = true;
            _inputProxy = new DictionaryProxy<string, IPort, GraphNodePort>(_input);
            _outputProxy = new DictionaryProxy<string, IPort, GraphNodePort>(_output);
        }

        protected abstract void AssignInputs();

        protected abstract void AssignOutputs();

        public override void Render(IFrameRenderData frameData, IFrameBufferTarget target = null)
        {
            // get inputs
            AssignInputs();

            base.Render(frameData, target);

            // set outputs
            AssignOutputs();

        }
    }
}
