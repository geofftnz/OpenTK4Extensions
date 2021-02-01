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
        private Dictionary<Keys, List<Tuple<KeyModifiers, Action>>> keymap = new Dictionary<Keys, List<Tuple<KeyModifiers, Action>>>();

        public KeyboardActionManager()
        {
        }

        public void ProcessKeyDown(Keys key, KeyModifiers keyModifiers)
        {
            List<Tuple<KeyModifiers, Action>> actions;
            lock (keymap)
            {

                if (keymap.TryGetValue(key, out actions))
                {
                    foreach (var maction in actions)
                    {
                        if (keyModifiers == maction.Item1)
                            maction.Item2();
                    }
                }
            }
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

        public bool ProcessKeyDown(KeyboardKeyEventArgs e)
        {
            ProcessKeyDown(e.Key, e.Modifiers);
            return false;
        }

        public bool ProcessKeyUp(KeyboardKeyEventArgs e)
        {
            return false;
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
