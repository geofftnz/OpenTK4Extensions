using OpenTKExtensions.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Framework
{
    public interface IRenderable
    {
        bool Visible { get; set; }
        int DrawOrder { get; set; }

        /// <summary>
        /// Specifies whether this component represents the final output of a composite operation.
        /// </summary>
        bool IsFinalOutput { get; set; }


        //void Render(IFrameRenderData frameData) { Render(frameData, null); }

        /// <summary>
        /// Render this component.
        /// 
        /// If a framebuffer target is supplied, the comment should:
        /// - Wrap the target Bind/Unbind calls around the parts of the rendering that do not involve other render-to-texture operations, OR
        /// - Pass the target to a (singular) child component
        /// </summary>
        /// <param name="frameData"></param>
        /// <param name="target"></param>
        void Render(IFrameRenderData frameData, IFrameBufferTarget target = null);
        event EventHandler<EventArgs> PreRender;
        void OnPreRender();
        /*
          
         public bool IsFinalOutput { get; set; } = true;

        , IFrameBufferTarget target
          , IFrameBufferTarget target = null
          
                target?.BindForWriting();
                target?.ClearAllColourBuffers(Vector4.Zero);

                target?.UnbindFromWriting();



        public void Render(IFrameRenderData frameData, IFrameBufferTarget target = null)
        {
            target?.BindForWriting();
            target?.ClearAllColourBuffers(Vector4.Zero);

            Render(frameData);
            
            target?.UnbindFromWriting();
        }


        */

    }
}
