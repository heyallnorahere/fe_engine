﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine.Math;
using FEEngine.Util;

namespace FEEngine
{
    class Test
    {
        public delegate int TestCallback(int a, int b);
        private Unit unit;
        public void Init()
        {
            this.unit = Map.GetMap().GetUnit(0);
        }
        public void DoStuff()
        {
            Vec2<int> pos = this.unit.Position;
            pos.Y++;
            this.unit.Position = pos;
        }
        private static int CallbackInstance(int a, int b)
        {
            return a + b;
        }
        public static TestCallback GetCallback()
        {
            return CallbackInstance;
        }
    }
    class TestItem : ItemBehavior
    {
        public void OnUse()
        {
            UI.UIController uiController = ObjectRegistry.GetRegister<UI.UIController>().Get(0);
            if (uiController.HasUnitSelected())
            {
                Logger.Print("Selected unit: " + uiController.GetUnitMenuTarget().Name, Renderer.Color.GREEN);
            }
            Vec2<int> pos = this.Parent.Parent.Position;
            pos += new Vec2<int>(1, 1);
            this.Parent.Parent.Position = pos;
        }
    }
}
