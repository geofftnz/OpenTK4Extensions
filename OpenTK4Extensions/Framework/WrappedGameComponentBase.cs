using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTKExtensions.Resources;

namespace OpenTKExtensions.Framework
{
    public class WrappedGameComponentBase : GameComponentBase, IResizeable, IReloadable, IUpdateable, IRenderable, IKeyboardControllable, ITransformable
    {
        private IGameComponent _childComponent;
        protected IGameComponent ChildComponent
        {
            get => _childComponent;
            set
            {
                if (_childComponent != null)
                {
                    throw new InvalidOperationException($"Child component has already been set on {GetType().Name}");
                }
                _childComponent = value;
            }
        }

        public bool Visible { get; set; } = true;
        public int DrawOrder { get; set; } = 0;
        public bool IsFinalOutput { get; set; } = true;
        public int KeyboardPriority { get; set; } = 0;
        public Matrix4 ViewMatrix { get; set; }
        public Matrix4 ModelMatrix { get; set; }
        public Matrix4 ProjectionMatrix { get; set; }

        public WrappedGameComponentBase()
            : base()
        {
            Loading += WrappedGameComponent_Loading;
            Unloading += WrappedGameComponent_Unloading;
        }

        private void WrappedGameComponent_Loading(object sender, EventArgs e)
        {
            ChildComponent?.Load();
        }

        private void WrappedGameComponent_Unloading(object sender, EventArgs e)
        {
            ChildComponent?.Unload();
        }

        /// <summary>
        /// For completeness only. Add can only be called once.
        /// </summary>
        /// <param name="component"></param>
        public virtual void Add(IGameComponent component)
        {
            ChildComponent = component;
        }

        public virtual void Resize(int width, int height)
        {
            (ChildComponent as IResizeable)?.Resize(width, height);
        }

        public virtual void Reload()
        {
            (ChildComponent as IReloadable)?.Reload();
        }

        public virtual void Update(IFrameUpdateData frameData)
        {
            (ChildComponent as IUpdateable)?.Update(frameData);
        }

        public virtual void Render(IFrameRenderData frameData, IFrameBufferTarget target = null)
        {
            if (Visible)
            {
                (ChildComponent as IRenderable)?.Render(frameData, target);
            }
        }

        public virtual bool ProcessKeyDown(KeyboardKeyEventArgs e)
        {
            return (ChildComponent as IKeyboardControllable)?.ProcessKeyDown(e) ?? false;
        }

        public virtual bool ProcessKeyUp(KeyboardKeyEventArgs e)
        {
            return (ChildComponent as IKeyboardControllable)?.ProcessKeyUp(e) ?? false;
        }
    }
}
