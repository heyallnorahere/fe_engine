using System;
using System.Collections.Generic;

namespace FEEngine.Menus.UnitContextMenuPages
{
    internal class ItemPage : UnitContextMenu.Page
    {
        private class ItemUsePage : UnitContextMenu.Page
        {
            public ItemUsePage(Item item)
            {
                mItem = item;
            }
            protected override string GetTitle()
            {
                return "Use";
            }
            protected override void OnSelect()
            {
                mItem.Use();
                Parent?.GoBack();
                GoBack();
                UIController.ResetSelectedUnit();
                UIController.IsUnitContextMenuOpen = false;
            }
            private readonly Item mItem;
        }
        private class ItemEquipPage : UnitContextMenu.Page
        {
            public ItemEquipPage(Item item)
            {
                mItem = item;
            }
            protected override string GetTitle()
            {
                return "Equip";
            }
            protected override void OnSelect()
            {
                this.VerifyValue(UIController.SelectedUnit).EquippedWeapon = mItem;
                Parent?.GoBack();
                GoBack();
            }
            private readonly Item mItem;
        }
        private class ItemUnequipPage : UnitContextMenu.Page
        {
            protected override string GetTitle()
            {
                return "Unequip";
            }
            protected override void OnSelect()
            {
                Unit unit = this.VerifyValue(UIController.SelectedUnit);
                Item? item = unit.EquippedWeapon;
                unit.EquippedWeapon = null;
                if (item != null) {
                    unit.Inventory.Add(item.RegisterIndex);
                }
                Parent?.GoBack();
                GoBack();
            }
        }
        private class ItemBackPage : UnitContextMenu.Page
        {
            protected override string GetTitle()
            {
                return "Back";
            }
            protected override void OnSelect()
            {
                Parent?.GoBack();
                GoBack();
            }
        }
        private class ItemSubPage : UnitContextMenu.Page
        {
            public ItemSubPage(Item item)
            {
                mItem = item;
            }
            protected override string GetTitle()
            {
                return mItem.Name;
            }
            protected override void OnSelect()
            {
                Unit selectedUnit = this.VerifyValue(UIController.SelectedUnit);
                if (mItem.Usable)
                {
                    AddChild(new ItemUsePage(mItem));
                }
                if (mItem.IsWeapon && mItem.RegisterIndex != selectedUnit.EquippedWeaponIndex)
                {
                    AddChild(new ItemEquipPage(mItem));
                }
                else if (mItem.RegisterIndex == selectedUnit.EquippedWeaponIndex)
                {
                    AddChild(new ItemUnequipPage());
                }
                AddChild(new ItemBackPage());
            }
            private readonly Item mItem;
        }
        protected override string GetTitle()
        {
            return "Item";
        }
        protected override void UpdatePage()
        {
            Children.Clear();
            Unit? unit = UIController.SelectedUnit;
            if (unit == null)
            {
                throw new ArgumentNullException();
            }
            Register<Item> itemRegister = this.VerifyValue(UIController.GameInstance).Registry.GetRegister<Item>();
            List<int> items = new();
            items.AddRange(unit.Inventory);
            if (unit.EquippedWeaponIndex != -1)
            {
                items.Add(unit.EquippedWeaponIndex);
            }
            foreach (int index in items)
            {
                Item item = itemRegister[index];
                AddChild(new ItemSubPage(item));
            }
        }
        internal override bool IsInternal { get { return true; } }
    }
}
