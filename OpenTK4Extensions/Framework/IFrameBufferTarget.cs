using OpenTK.Mathematics;

namespace OpenTKExtensions.Framework
{
    public interface IFrameBufferTarget
    {
        void BindForWriting();
        void ClearAllColourBuffers(Vector4 colour);
        void ClearColourBuffer(int drawBuffer, Vector4 colour);
        void UnbindFromWriting();
    }
}