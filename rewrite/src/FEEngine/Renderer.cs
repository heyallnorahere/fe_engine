using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FEEngine.Math;

namespace FEEngine
{
    public interface IRenderable
    {
        void Render(RenderContext context);
        IVec2<int> MinSize { get; }
        void SetSize(IVec2<int> size);
    }
    public enum Color
    {
        Red = 1,
        Green = 2,
        Blue = 4,
        Yellow = 3,
        LightBlue = 6,
        Purple = 5,
        White = 7,
        Black = 0,
    }
    public class RenderContext
    {
        public RenderContext(List<char> characterBuffer, List<Color> colorBuffer, IVec2<int> bufferSize)
        {
            mStack = new();
            mCharacterBuffer = characterBuffer;
            mColorBuffer = colorBuffer;
            mBufferSize = bufferSize;
        }
        public struct OffsetClipPair
        {
            public IVec2<int> Offset, Clip;
        }
        public void PushPair(OffsetClipPair pair)
        {
            mStack.Push(pair);
        }
        public void PopPair()
        {
            mStack.Pop();
        }
        public bool RenderChar(IVec2<int> position, char character, Color color = Color.White)
        {
            OffsetClipPair currentData = GetCurrentOffsetClipData();
            if (MathUtil.IsVectorOutOfBounds(position, currentData.Clip))
            {
                return false;
            }
            int bufferIndex = Renderer.GetBufferIndex(MathUtil.AddVectors(position, currentData.Offset), mBufferSize);
            mCharacterBuffer[bufferIndex] = character;
            mColorBuffer[bufferIndex] = color;
            return true;
        }
        public bool RenderString(IVec2<int> position, string text, Color color = Color.White)
        {
            foreach (char character in text)
            {
                if (!RenderChar(position, character, color))
                {
                    return false;
                }
            }
            return true;
        }
        private OffsetClipPair GetCurrentOffsetClipData()
        {
            OffsetClipPair data = new()
            {
                Offset = new Vec2I(0),
                Clip = new Vec2I(mBufferSize)
            };
            foreach (OffsetClipPair pair in mStack)
            {
                MathUtil.AddVectors(ref data.Offset, pair.Offset);
                if (pair.Clip.X < data.Clip.X)
                {
                    data.Clip.X = pair.Clip.X;
                }
                if (pair.Clip.Y < data.Clip.Y)
                {
                    data.Clip.Y = pair.Clip.Y;
                }
            }
            return data;
        }
        private readonly Stack<OffsetClipPair> mStack;
        private readonly List<char> mCharacterBuffer;
        private readonly List<Color> mColorBuffer;
        private readonly IVec2<int> mBufferSize;
    }
    public class Renderer
    {
        public BorderLayout Root { get; set; }
        public void Render()
        {
            RenderContext context = new(mCharacterBuffer, mColorBuffer, mBufferSize);
            Root.SetSize(BufferSize);
            Root.Render(context);
            Present();
        }
        private void Present()
        {
            if (Game.HasNativeImplementation)
            {
                ClearNativeBuffer_Native();
                for (int y = mBufferSize.Y - 1; y >= 0; y--)
                {
                    for (int x = 0; x < mBufferSize.X; x++)
                    {
                        int bufferIndex = GetBufferIndex(new Vec2I(x, y), mBufferSize);
                        char character = mCharacterBuffer[bufferIndex];
                        Color color = mColorBuffer[bufferIndex];
                        WriteColoredChar_Native(character, color);
                    }
                    WriteColoredChar_Native('\n', Color.White);
                }
                Present_Native();
            }
            else
            {
#if CLEAR_MODE_FULL_CLEAR
                Console.Clear();
#endif
                Console.SetCursorPosition(0, 0);
                string text = "";
                for (int y = mBufferSize.Y - 1; y >= 0; y--)
                {
                    for (int x = 0; x < mBufferSize.X; x++)
                    {
                        int bufferIndex = GetBufferIndex(new Vec2I(x, y), mBufferSize);
                        char character = mCharacterBuffer[bufferIndex];
                        text += character;
                    }
                    text += '\n';
                }
                foreach (char character in text)
                {
                    Console.Write(character);
                }
            }
        }
        public IVec2<int> BufferSize
        {
            get => mBufferSize;
            set
            {
                int length = value.X * value.Y;
                int currentLength = mBufferSize.X * mBufferSize.Y;
                mBufferSize = value;
                int difference = length - currentLength;
                if (difference > 0)
                {
                    for (int i = currentLength; i < length; i++)
                    {
                        mCharacterBuffer.Add((char)0);
                        mColorBuffer.Add(Color.Black);
                    }
                }
                ClearBuffer();
            }
        }
        public void ClearBuffer()
        {
            for (int x = 0; x < mBufferSize.X; x++)
            {
                for (int y = 0; y < mBufferSize.Y; y++)
                {
                    int bufferIndex = GetBufferIndex(new Vec2I(x, y), mBufferSize);
                    mCharacterBuffer[bufferIndex] = ' ';
                    mColorBuffer[bufferIndex] = Color.Black;
                }
            }
        }
        public static int GetBufferIndex(IVec2<int> position, IVec2<int> bufferSize)
        {
            return (position.Y * bufferSize.X) + position.X;
        }
        public Renderer()
        {
            BufferSize = new Vec2I(60, 30);
            Root = new BorderLayout();
        }
        static Renderer()
        {
            if (Game.HasNativeImplementation)
            {
                DisableCursor_Native();
            }
        }
        private readonly List<char> mCharacterBuffer = new();
        private readonly List<Color> mColorBuffer = new();
        private IVec2<int> mBufferSize = new Vec2I(0);
        // native methods
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void WriteColoredChar_Native(char character, Color color);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void ClearNativeBuffer_Native();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Present_Native();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void DisableCursor_Native();
    }
}
