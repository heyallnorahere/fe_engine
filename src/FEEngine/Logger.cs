using System.Collections.Generic;

namespace FEEngine
{
    public class Logger
    {
        public static int MaxLogSize { get; set; }
        public class RenderAgent : IRenderable
        {
            public Vector2 MinSize
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
                    return new Vector2(longestMessageLength, MaxLogSize);
                }
            }
            public void Render(RenderContext context)
            {
                int startAt = mRenderSize.Y - 1;
                for (int i = 0; i < log.Count; i++)
                {
                    int yPosition = startAt - i;
                    Message message = log[i];
                    context.RenderString(new Vector2(0, yPosition), message.Text, message.Color);
                }
            }
            public void SetSize(Vector2 size)
            {
                mRenderSize = size;
            }
            private Vector2 mRenderSize = new(0);
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
