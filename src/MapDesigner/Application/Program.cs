namespace MapDesigner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Window window = new("test", 800, 600, true);
            window.Loop();
        }
    }
}
