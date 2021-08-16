using System.Collections.Generic;
using FEEngine.Math;

namespace FEEngine
{
    public class Logger
    {
        public static int MaxLogSize { get; set; }
        public class RenderAgent : IRenderable
        {
            public IVec2<int> MinSize
            {
                get
                {
                    int longestMessageLength = 0;
                    foreach (Message message in log)
                    {
                        string text = message.Text;
                        if (text.Length > longestMessageLength)
                        {
                            longestMessageLength = text.Length;
                        }
                    }
                    return new Vec2I(longestMessageLength, MaxLogSize);
                }
            }
            public void Render(RenderContext context)
            {
                int startAt = mRenderSize.Y - 1;
                for (int i = 0; i < log.Count; i++)
                {
                    int yPosition = startAt - i;
                    Message message = log[i];
                    context.RenderString(new Vec2I(0, yPosition), message.Text, message.Color);
                }
            }
            public void SetSize(IVec2<int> size)
            {
                mRenderSize = size;
            }
            private IVec2<int> mRenderSize = new Vec2I(0);
        }
        public static void Print(Color color, string format, params object[] args)
        {
            log.Add(new()
            {
                Text = string.Format(format, args),
                Color = color
            });
        }
        internal static void PurgeLog()
        {
            int lastValidMessage = log.Count - MaxLogSize;
            bool keepGoing = true;
            while (keepGoing)
            {
                keepGoing = false;
                for (int i = 0; i < log.Count; i++)
                {
                    if (i < lastValidMessage)
                    {
                        log.RemoveAt(i);
                        keepGoing = true;
                        lastValidMessage--;
                        break;
                    }
                }
            }
        }
        private Logger() { }
        static Logger()
        {
            MaxLogSize = 10; // by default
            log = new();
        }
        private struct Message
        {
            public string Text { get; set; }
            public Color Color { get; set; }
        }
        private static readonly List<Message> log;
    }
}
