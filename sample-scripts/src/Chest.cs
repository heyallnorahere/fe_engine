using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine;
using FEEngine.Math;

namespace Scripts
{
    public class Chest
    {
        private static void ChestBehavior(Unit trigger, Tile tile, Vec2<int> position)
        {
            Item item = Item.NewItem<Vulnerary>("Vulnerary");
            trigger.AddItem(item);
#if FEENGINE_DEBUG
            Logger.Print($"Got vector: ({position.X}, {position.Y})");
#endif
            // todo: add popup on UIController or something
        }
        public static Tile.InteractionBehavior GetBehavior()
        {
            return ChestBehavior;
        }
    }
}
