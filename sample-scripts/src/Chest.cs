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
            // todo: add popup on UIController or something
        }
        public static Tile.InteractBehavior GetBehavior()
        {
            return ChestBehavior;
        }
    }
}
