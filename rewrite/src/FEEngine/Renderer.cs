using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FEEngine.Math;

namespace FEEngine
{
    public interface IRenderable
    {
        void Render();
    }
    public class RenderQueue
    {
        public bool Open { get; private set; }
        public RenderQueue()
        {
            Open = true;
            mQueue = new();
        }
        public void Submit(IRenderable renderable)
        {
            if (!Open)
            {
                throw new Exception("You cannot submit an object to a closed RenderQueue");
            }
            mQueue.Enqueue(renderable);
        }
        public void Close()
        {
            Open = false;
        }
        public IRenderable Peek()
        {
            return mQueue.Peek();
        }
        public IRenderable Pop()
        {
            try
            {
                return mQueue.Dequeue();
            }
            catch (Exception)
            {
                return null;
            }
        }
        private Queue<IRenderable> mQueue;
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
    public class Renderer
    {
        public static void Render(RenderQueue renderQueue)
        {
            if (renderQueue.Open)
            {
                renderQueue.Close();
            }
            Context context = new();
            context.BufferSize = bufferSize;
            IRenderable currentObject;
            while ((currentObject = renderQueue.Pop()) != null)
            {
                currentObject.Render(context);
            }
            Present();
        }
        private static void Present()
        {
            if (Game.HasNativeImplementation)
            {
                ClearNativeBuffer_Native();
                for (int y = bufferSize.Y - 1; y >= 0; y--)
                {
                    for (int x = 0; x < bufferSize.X; x++)
                    {
                        int bufferIndex = GetBufferIndex(new Vec2I(x, y));
                        char character = characterBuffer[bufferIndex];
                        Color color = colorBuffer[bufferIndex];
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
                for (int y = bufferSize.Y - 1; y >= 0; y--)
                {
                    for (int x = 0; x < bufferSize.X; x++)
                    {
                        int bufferIndex = GetBufferIndex(new Vec2I(x, y));
                        char character = characterBuffer[bufferIndex];
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
        public static bool RenderChar(IVec2<int> position, char character, Color color = Color.White)
        {
            if (MathUtil.IsVectorOutOfBounds(position, bufferSize))
            {
                return false;
            }
            int bufferIndex = GetBufferIndex(position);
            characterBuffer[bufferIndex] = character;
            colorBuffer[bufferIndex] = color;
            return true;
        }
        public static IVec2<int> BufferSize
        {
            get => bufferSize;
            set
            {
                int length = value.X * value.Y;
                int currentLength = bufferSize.X * bufferSize.Y;
                bufferSize = value;
                int difference = length - currentLength;
                if (difference > 0)
                {
                    for (int i = currentLength; i < length; i++)
                    {
                        characterBuffer.Add((char)0);
                        colorBuffer.Add(Color.Black);
                    }
                }
                ClearBuffer();
            }
        }
        public static void ClearBuffer()
        {
            for (int x = 0; x < bufferSize.X; x++)
            {
                for (int y = 0; y < bufferSize.Y; y++)
                {
                    int bufferIndex = GetBufferIndex(new Vec2I(x, y));
                    characterBuffer[bufferIndex] = ' ';
                    colorBuffer[bufferIndex] = Color.Black;
                }
            }
        }
        private static int GetBufferIndex(IVec2<int> position)
        {
            return (position.Y * bufferSize.X) + position.X;
        }
        private Renderer() { }
        static Renderer()
        {
            if (Game.HasNativeImplementation)
            {
                DisableCursor_Native();
            }
            BufferSize = new Vec2I(60, 30);
        }
        private static readonly List<char> characterBuffer = new();
        private static readonly List<Color> colorBuffer = new();
        private static IVec2<int> bufferSize = new Vec2I(0);
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
