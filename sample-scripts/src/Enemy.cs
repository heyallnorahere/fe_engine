using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine;
using FEEngine.Math;

namespace Scripts
{
    class Enemy : Behavior
    {
        private List<Unit> units;
        public void OnAttach()
        {
            this.units = new List<Unit>();
            for (ulong i = 0; i < Map.GetUnitCount(); i++)
            {
                Unit unit = Map.GetUnit(i);
                if (unit.Index == this.Parent.Index)
                {
                    continue;
                }
                this.units.Add(unit);
            }
        }
        public void OnUpdate()
        {
            if (this.Parent.HP < this.Parent.Stats.MaxHP)
            {
                for (ulong i = 0; i < this.Parent.GetInventorySize(); i++)
                {
                    Item item = this.Parent.GetInventoryItem(i);
                    if (item.Name == "Vulnerary")
                    {
                        item.Use();
                        break;
                    }
                }
            }
            Vec2 delta = new Vec2(0, 0);
            foreach (Unit u in units)
            {
                delta += u.Position - this.Parent.Position;
            }
            this.Parent.Move(delta);
        }
        private void RenderWeapon(Renderer renderer, Weapon weapon, char indexChar, int y)
        {
            renderer.RenderStringAt(new Vec2(0, y), indexChar + ": " + weapon.Name, Renderer.Color.WHITE);
        }
        public void OnRender(Renderer renderer)
        {
            Vec2 bufferSize = renderer.GetBufferSize();
            Vec2 mapSize = Map.GetSize();
            int y = bufferSize.Y - (mapSize.Y + 2);
            if (this.Parent.HasWeaponEquipped())
            {
                this.RenderWeapon(renderer, this.Parent.GetEquippedWeapon(), 'E', y);
            }
            else
            {
                renderer.RenderStringAt(new Vec2(0, y), "E: None", Renderer.Color.WHITE);
            }
            List<Weapon> weapons = new List<Weapon>();
            for (ulong i = 0; i < this.Parent.GetInventorySize(); i++)
            {
                if (this.Parent.GetInventoryItem(i).IsWeapon())
                {
                    weapons.Add(this.Parent.GetInventoryWeapon(i));
                }
            }
            for (int i = 0; i < weapons.Count; i++)
            {
                Weapon weapon = weapons[i];
                this.RenderWeapon(renderer, weapon, (i + 1).ToString()[0], y - (2 + i));
            }
        }
    }
}
