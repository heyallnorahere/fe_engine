using System;
using System.Collections.Generic;
using FEEngine;
using FEEngine.Math;
using FEEngine.Classes;
using ExampleGame;

[assembly: AssemblyAssetLoader(typeof(ExampleGameAssetLoader))]

namespace ExampleGame
{
    public class ExampleGameAssetLoader : AssetLoader 
    {
        public override void Load(Game game)
        {
            IVec2<int> size = new Vec2I(20, 10);
            var units = new List<int>();
            units.AddRange(AddUnits(Unit.UnitAffiliation.Player, 0, size.X, game));
            units.AddRange(AddUnits(Unit.UnitAffiliation.Enemy, size.Y - 2, size.X, game));
            var map = new Map(size.X, size.Y, units);
            game.Registry.GetRegister<Map>().Add(map);
            // todo: figure out a better way to do all of this
            var unitRegister = game.Registry.GetRegister<Unit>();
            foreach (int index in units)
            {
                Unit unit = unitRegister[index];
                unit.Parent = map;
            }
        }
        private static List<int> AddUnits(Unit.UnitAffiliation affiliation, int y, int mapWidth, Game game)
        {
            var stats = Unit.CreateStats(hp: 15, str: 9, mag: 1, dex: 7, spd: 5, lck: 8, def: 7, res: 5, cha: 10, mv: 5);
            var indices = new List<int>();
            Register<Unit> unitRegister = game.Registry.GetRegister<Unit>();
            Register<Item> itemRegister = game.Registry.GetRegister<Item>();
            for (int x = 0; x < mapWidth; x++)
            {
                int unitY = y + (x % 2);
                var unit = new Unit(new Vec2I(x, unitY), affiliation, stats)
                {
                    Class = new Soldier(),
                };
                indices.Add(unitRegister.Count);
                unitRegister.Add(unit);
                unit.AddSkill<HawkeyePlus>();
                int lanceIndex = CreateLance(itemRegister);
                unit.EquippedWeapon = itemRegister[lanceIndex];
            }
            return indices;
        }
        private static int CreateLance(Register<Item> itemRegister)
        {
            var stats = new WeaponStats
            {
                Attack = 5,
                HitRate = 80,
                CritRate = 0,
                Weight = 1,
                Type = WeaponType.Lance
            };
            var lance = new Item(false, weaponStats: stats, name: "C#ium Lance");
            int index = itemRegister.Count;
            itemRegister.Add(lance);
            return index;
        }
    }
}
