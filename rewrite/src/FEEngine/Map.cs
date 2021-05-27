using System.Collections.Generic;
using Newtonsoft.Json;

namespace FEEngine
{
    [JsonObject]
    public class Map : RegistedObjectTemplate<Map>
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<int> Units { get; private set; }
        public void AddUnit(Unit unit)
        {
            Units.Add(unit.RegisterIndex);
        }
        [JsonConstructor]
        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Units = new List<int>();
        }
    }
}
