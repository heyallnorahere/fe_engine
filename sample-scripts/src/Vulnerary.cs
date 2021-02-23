using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine;
using FEEngine.Math;

namespace Scripts
{
    class Vulnerary : ItemBehavior
    {
        void OnUse()
        {
            uint hp = this.Parent.Parent.HP;
            uint maxHp = this.Parent.Parent.Stats.MaxHP;
            uint diff = maxHp - hp;
            uint amount = 10;
            if (diff < amount)
            {
                amount = diff;
            }
            this.Parent.Parent.HP += amount;
        }
    }
}
