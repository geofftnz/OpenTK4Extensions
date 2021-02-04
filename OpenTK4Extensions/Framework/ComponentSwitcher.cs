using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKExtensions.Input;

namespace OpenTKExtensions.Framework
{
    public class ComponentSwitcher : CompositeGameComponent, IResizeable, IReloadable, IUpdateable, IRenderable, IKeyboardControllable
    {
        public bool SendKeypressesToInvisibleComponents { get; set; } = false;
        public bool UpdateInvisibleComponents { get; set; } = false;

        private int currentComponentIndex = -1;


        public KeySpec KeyForward { get; set; } = new KeySpec(Keys.Tab);
        public KeySpec KeyBackward { get; set; } = new KeySpec(Keys.Tab, KeyModifiers.Shift);

        /// <summary>
        /// Defines what key modifier flags are used when testing the modifiers for keypresses.
        /// </summary>
        public KeyModifiers KeyModifierMask { get; set; } = KeyModifiers.Alt | KeyModifiers.Control | KeyModifiers.Shift;


        public int CurrentComponentIndex
        {
            get { return currentComponentIndex; }
            set
            {
                if (Components.Count > 0)
                {
                    currentComponentIndex = value;
                    if (currentComponentIndex < 0)
                        currentComponentIndex = 0;
                    else if (currentComponentIndex > Components.Count - 1)
                        currentComponentIndex = Components.Count - 1;
                }
                else
                {
                    currentComponentIndex = -1;
                }
            }
        }

        public IGameComponent CurrentComponent
        {
            get
            {
                return currentComponentIndex >= 0 ? this.Components[currentComponentIndex] : null;
            }
        }

        public ComponentSwitcher() : base()
        {
        }

        public override bool ProcessKeyDown(KeyboardKeyEventArgs e)
        {
            // switching
            if (e.Key == KeyForward.Key && ((e.Modifiers & KeyModifierMask) == KeyForward.Modifiers))
            {
                if (Components.Count > 1)
                {
                    CurrentComponentIndex = (CurrentComponentIndex + 1) % Components.Count;
                }
            }

            if (e.Key == KeyBackward.Key && ((e.Modifiers & KeyModifierMask) == KeyBackward.Modifiers))
            {
                if (Components.Count > 1)
                {
                    CurrentComponentIndex = (CurrentComponentIndex + Components.Count - 1) % Components.Count;
                }
            }

            return SendKeypressesToInvisibleComponents
                ? base.ProcessKeyDown(e)
                : (CurrentComponent as IKeyboardControllable)?.ProcessKeyDown(e) ?? false;
        }
        public override bool ProcessKeyUp(KeyboardKeyEventArgs e)
        {
            return SendKeypressesToInvisibleComponents
                ? base.ProcessKeyUp(e)
                : (CurrentComponent as IKeyboardControllable)?.ProcessKeyUp(e) ?? false;
        }

        public override void Update(IFrameUpdateData frameData)
        {
            if (UpdateInvisibleComponents)
            {
                base.Update(frameData);
            }
            else
            {
                (CurrentComponent as IUpdateable)?.Update(frameData);
            }
        }

        public override void Render(IFrameRenderData frameData)
        {
            (CurrentComponent as IRenderable)?.Render(frameData);
        }

        public override void Add(IGameComponent component)
        {
            base.Add(component);
            if (CurrentComponentIndex < 0 && Components.Count > 0)
            {
                CurrentComponentIndex = 0;
            }
        }
        public override void Remove(IGameComponent component)
        {
            base.Remove(component);
            if (CurrentComponentIndex >= Components.Count)
            {
                CurrentComponentIndex = Components.Count - 1;
            }
        }
    }
}
