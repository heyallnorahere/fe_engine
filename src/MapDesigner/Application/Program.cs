namespace MapDesigner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Window window = new("test", 800, 600, true);
            Control control = new Control(window, "Static", "Hello", 100, 100, 0, 0);
            window.Loop();
        }
    }
}
