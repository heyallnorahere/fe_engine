using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FEEngine
{
    /// <summary>
    /// An interface to a renderable object
    /// </summary>
    public interface IRenderable
    {
        void Render(RenderContext context);
        Vector2 MinSize { get; }
        void SetSize(Vector2 size);
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
    /// <summary>
    /// An object that provides information about the available, renderable space and carries out instructions on that space
    /// </summary>
    public class RenderContext
    {
        public RenderContext(List<char> characterBuffer, List<Color> colorBuffer, Vector2 bufferSize)
        {
            mStack = new();
            mCharacterBuffer = characterBuffer;
            mColorBuffer = colorBuffer;
            mBufferSize = bufferSize;
        }
        /// <summary>
        /// A set of an offset and a clip, for positioning renderable objects
        /// </summary>
        public struct OffsetClipPair
        {
            public Vector2 Offset, Clip;
        }
        /// <summary>
        /// Pushes a <see cref="OffsetClipPair"/> onto the stack
        /// </summary>
        /// <param name="pair">The <see cref="OffsetClipPair"/> to push</param>
        public void PushPair(OffsetClipPair pair)
        {
            mStack.Push(pair);
        }
        /// <summary>
        /// Pops a <see cref="OffsetClipPair"/> off of the stack
        /// </summary>
        public void PopPair()
        {
            mStack.Pop();
        }
        /// <summary>
        /// Renders a character at the specified position with the specified <see cref="Color"/>.
        /// </summary>
        /// <param name="position">Where to render the character</param>
        /// <param name="character">What character to render</param>
        /// <param name="color">The <see cref="Color"/> to render the character as</param>
        /// <returns></returns>
        public bool RenderChar(Vector2 position, char character, Color color = Color.White)
        {
            OffsetClipPair currentData = GetCurrentOffsetClipData();
            if (position.IsOutOfBounds(currentData.Clip - currentData.Offset))
            {
                return false;
            }
            int bufferIndex = Renderer.GetBufferIndex(position + currentData.Offset, mBufferSize);
            mCharacterBuffer[bufferIndex] = character;
            mColorBuffer[bufferIndex] = color;
            return true;
        }
        /// <summary>
        /// Renders a string at the specified position with the specified <see cref="Color"/>.
        /// </summary>
        /// <param name="position">Where to render the text</param>
        /// <param name="text">What text to render</param>
        /// <param name="color">The <see cref="Color"/> to render the text as</param>
        /// <returns></returns>
        public bool RenderString(Vector2 position, string text, Color color = Color.White)
        {
            int xOffset = 0;
            foreach (char character in text)
            {
                if (!RenderChar(position + (xOffset, 0), character, color))
                {
                    return false;
                }
                xOffset++;
            }
            return true;
        }
        private OffsetClipPair GetCurrentOffsetClipData()
        {
            OffsetClipPair data = new()
            {
                Offset = new Vector2(0),
                Clip = new Vector2(mBufferSize)
            };
            OffsetClipPair[] array = new OffsetClipPair[mStack.Count];
            mStack.CopyTo(array, 0);
            for (int i = array.Length - 1; i >= 0; i--)
            {
                OffsetClipPair pair = array[i];
                data.Offset += pair.Offset;
                Vector2 clipWithOffset = data.Offset + pair.Clip;
                if (clipWithOffset.X < data.Clip.X)
                {
                    data.Clip.X = clipWithOffset.X;
                }
                if (clipWithOffset.Y < data.Clip.Y)
                {
                    data.Clip.Y = clipWithOffset.Y;
                }
            }
            return data;
        }
        private readonly Stack<OffsetClipPair> mStack;
        private readonly List<char> mCharacterBuffer;
        private readonly List<Color> mColorBuffer;
        private readonly Vector2 mBufferSize;
    }
    public class Renderer
    {
        /// <summary>
        /// The root renderable object
        /// </summary>
        public IRenderable? Root { get; set; }
        /// <summary>
        /// Renders the root object (<see cref="Root"/>) and all of its children.
        /// </summary>
        public void Render()
        {
            RenderContext context = new(mCharacterBuffer, mColorBuffer, mBufferSize);
            if (Root != null)
            {
                Root.SetSize(BufferSize);
                Root.Render(context);
            }
            Present();
        }
        private void Present()
        {
            var buffer = new List<KeyValuePair<char, Color>>();
            for (int y = mBufferSize.Y - 1; y >= 0; y--)
            {
                for (int x = 0; x < mBufferSize.X; x++)
                {
                    int bufferIndex = GetBufferIndex(new Vector2(x, y), mBufferSize);
                    char character = mCharacterBuffer[bufferIndex];
                    Color color = mColorBuffer[bufferIndex];
                    buffer.Add(new KeyValuePair<char, Color>(character, color));
                }
                if (y > 0)
                {
                    buffer.Add(new KeyValuePair<char, Color>('\n', Color.White));
                }
            }
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            foreach (var pair in buffer)
            {
                Console.ForegroundColor = pair.Value switch
                {
                    Color.White => ConsoleColor.White,
                    Color.Black => ConsoleColor.Black,
                    Color.Red => ConsoleColor.Red,
                    Color.Green => ConsoleColor.Green,
                    Color.Blue => ConsoleColor.Blue,
                    Color.Yellow => ConsoleColor.Yellow,
                    Color.LightBlue => ConsoleColor.Cyan,
                    Color.Purple => ConsoleColor.Magenta,
                    _ => throw new ArgumentException("Invalid color!")
                };
                Console.Write(pair.Key);
            }
        }
        /// <summary>
        /// The size of the allocated buffer
        /// </summary>
        public Vector2 BufferSize
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
        /// <summary>
        /// Clears the <see cref="Renderer"/>'s buffer
        /// </summary>
        public void ClearBuffer()
        {
            for (int x = 0; x < mBufferSize.X; x++)
            {
                for (int y = 0; y < mBufferSize.Y; y++)
                {
                    int bufferIndex = GetBufferIndex(new Vector2(x, y), mBufferSize);
                    mCharacterBuffer[bufferIndex] = ' ';
                    mColorBuffer[bufferIndex] = Color.Black;
                }
            }
        }
        /// <summary>
        /// A utility function for getting an index in a 1-dimensional array with a 2D vector
        /// </summary>
        /// <param name="position">The position to calculate</param>
        /// <param name="bufferSize">The virtual 2D size of the buffer</param>
        /// <returns>The computed index</returns>
        public static int GetBufferIndex(Vector2 position, Vector2 bufferSize)
        {
            return (position.Y * bufferSize.X) + position.X;
        }
        public Renderer()
        {
            // 119 x 30 is the default win32 console size, if i recall
            BufferSize = new Vector2(100, 30);
        }
        static Renderer()
        {
            Console.CursorVisible = false;
        }
        private readonly List<char> mCharacterBuffer = new();
        private readonly List<Color> mColorBuffer = new();
        private Vector2 mBufferSize = new(0);
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
