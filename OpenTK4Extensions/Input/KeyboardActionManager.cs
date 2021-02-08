using OpenTK;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKExtensions.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Input
{
    public class KeyboardActionManager : GameComponentBase, IKeyboardControllable
    {

        public KeyModifiers ModifierMask { get; set; } = KeyModifiers.Alt | KeyModifiers.Shift | KeyModifiers.Control;

        /// <summary>
        /// When true, will return true to process key calls when the key is bound to an action, preventing further processing of this key
        /// </summary>
        public bool CaptureBoundKeypresses { get; set; } = false;

        /// <summary>
        /// When true, will return true to all process key calls, preventing further processing of all keys
        /// </summary>
        public bool CaptureAllKeypresses { get; set; } = false;

        private Dictionary<Keys, List<Tuple<KeyModifiers, Action>>> keymap = new Dictionary<Keys, List<Tuple<KeyModifiers, Action>>>();

        public KeyboardActionManager()
        {
        }

        public bool ProcessKeyDown(Keys key, KeyModifiers keyModifiers)
        {
            bool processed = false;

            List<Tuple<KeyModifiers, Action>> actions;
            lock (keymap)
            {

                if (keymap.TryGetValue(key, out actions))
                {
                    foreach (var maction in actions)
                    {
                        if ((keyModifiers & ModifierMask) == maction.Item1)
                        {
                            maction.Item2();
                            processed = true;
                        }
                    }
                }
            }
            return processed;
        }

        public void Add(Keys key, KeyModifiers modifiers, Action action)
        {
            List<Tuple<KeyModifiers, Action>> actions;

            lock (keymap)
            {
                if (!keymap.TryGetValue(key, out actions))
                {
                    actions = new List<Tuple<KeyModifiers, Action>>();
                    keymap.Add(key, actions);
                }

                actions.Add(new Tuple<KeyModifiers, Action>(modifiers, action));
            }
        }

        public void Add(KeySpec keySpec, Action action)
        {
            Add(keySpec.Key, keySpec.Modifiers, action);
        }

        public void Clear()
        {
            lock (keymap)
            {
                keymap.Clear();
            }
        }

        public bool ProcessKeyDown(KeyboardKeyEventArgs e)
        {
            bool processed = ProcessKeyDown(e.Key, e.Modifiers);
            return CaptureAllKeypresses || (processed & CaptureBoundKeypresses);
        }

        public bool ProcessKeyUp(KeyboardKeyEventArgs e)
        {
            return CaptureAllKeypresses;
        }

        public int Count
        {
            get
            {
                return keymap.Values.SelectMany(v => v).Count();
            }
        }

        public int KeyboardPriority { get; set; } = 0;
    }
}
