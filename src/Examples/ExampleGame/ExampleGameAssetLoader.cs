using System;
using System.Collections.Generic;
using FEEngine;
using FEEngine.Classes;
using FEEngine.GameLoader;
using FEEngine.Math;

[assembly: AssemblyAssetLoader(typeof(ExampleGame.ExampleGameAssetLoader))]

namespace ExampleGame
{
    public class ExampleGameAssetLoader : AssetLoader 
    {
        protected override void LoadContent(Game game)
        {
            IVec2<int> size = new Vec2I(25, 15);
            var units = new List<int>();
            units.AddRange(AddUnits(Unit.UnitAffiliation.Player, size.Y - 5, size.X, game));
            units.AddRange(AddUnits(Unit.UnitAffiliation.Enemy, size.Y - 2, size.X, game));
            var map = new Map(size.X, size.Y);
            var unitRegister = game.Registry.GetRegister<Unit>();
            foreach (int index in units)
            {
                map.AddUnit(unitRegister[index]);
            }
            game.Registry.GetRegister<Map>().Add(map);
        }
        private static List<int> AddUnits(Unit.UnitAffiliation affiliation, int y, int mapWidth, Game game)
        {
            var stats = Unit.CreateStats(hp: 15, str: 9, mag: 1, dex: 7, spd: 5, lck: 8, def: 7, res: 5, cha: 10, mv: 5);
            var indices = new List<int>();
            Register<Unit> unitRegister = game.Registry.GetRegister<Unit>();
            Register<Item> itemRegister = game.Registry.GetRegister<Item>();
            Register<Battalion> battalionRegister = game.Registry.GetRegister<Battalion>();
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
                unit.Inventory.Add(lanceIndex);
                unit.EquippedWeapon = itemRegister[lanceIndex];
                unit.Battalion = CreateBattalion(battalionRegister, unit);
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
                Type = WeaponType.Lance,
                Range = new Vec2I(1),
                Weight = 1,
                Durability = 25
            };
            var lance = new Item(false, weaponStats: stats, name: "C#ium Lance");
            int index = itemRegister.Count;
            itemRegister.Add(lance);
            return index;
        }
        private static Battalion CreateBattalion(Register<Battalion> battalionRegister, Unit unit)
        {
            var battalion = new Battalion
            {
                Name = "Test Battalion",
                Parent = unit,
                StatBoosts = new BattalionStatBoosts
                {
                    CharmBoost = 5
                }
            };
            battalion.SetGambit<TestGambit>();
            battalionRegister.Add(battalion);
            return battalion;
        }
    }
}
