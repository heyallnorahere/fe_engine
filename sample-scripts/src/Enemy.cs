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
            List<Unit> units = new List<Unit>();
            for (ulong i = 0; i < Map.GetUnitCount(); i++)
            {
                Unit unit = Map.GetUnit(i);
                if (unit.Affiliation == Unit.UnitAffiliation.PLAYER)
                {
                    units.Add(unit);
                }
            }
            Vec2 delta = new Vec2(0, 0);
            foreach (Unit unit in units)
            {
                delta += unit.Position - this.Parent.Position;
            }
            this.Parent.Move(delta);
            List<Unit> canAttack = new List<Unit>();
            if (this.Parent.HasWeaponEquipped())
            {
                Vec2 range = this.Parent.GetEquippedWeapon().Stats.Range;
                foreach (Unit unit in units)
                {
                    int distance = (unit.Position - this.Parent.Position).TaxicabLength();
                    if (distance >= range.X && distance <= range.Y)
                    {
                        canAttack.Add(unit);
                        Logger.Print(this.Parent.Index + " can attack " + unit.Index);
                    }
                }
            }
            Unit closest = this.Parent;
            foreach (Unit unit in canAttack)
            {
                if (unit.Index == this.Parent.Index)
                {
                    continue;
                }
                int distance = (unit.Position - this.Parent.Position).TaxicabLength();
                int closestDistance = (closest.Position - this.Parent.Position).TaxicabLength();
                if (distance < closestDistance || closest.Index == this.Parent.Index)
                {
                    closest = unit;
                }
            }
            if (closest.Index != this.Parent.Index)
            {
                this.Parent.Attack(closest);
                Logger.Print(this.Parent.Index + ": Attacked unit " + closest.Index, Renderer.Color.RED);
            }
        }
        /*private void RenderWeapon(Renderer renderer, Weapon weapon, char indexChar, int y)
        {
            renderer.RenderStringAt(new Vec2(0, y), indexChar + ": " + weapon.Name, Renderer.Color.RED);
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
                renderer.RenderStringAt(new Vec2(0, y), "E: None", Renderer.Color.RED);
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
        }*/
    }
}
