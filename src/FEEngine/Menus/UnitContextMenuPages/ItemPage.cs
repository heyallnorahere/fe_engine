using System;

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
                Parent.GoBack();
                GoBack();
                UIController.ResetSelectedUnit();
                UIController.IsUnitContextMenuOpen = false;
            }
            private readonly Item mItem;
        }
        private class ItemBackPage : UnitContextMenu.Page
        {
            protected override string GetTitle()
            {
                return "Back";
            }
            protected override void OnSelect()
            {
                Parent.GoBack();
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
            private readonly Item mItem;
        }
        protected override string GetTitle()
        {
            return "Item";
        }
        protected override void UpdatePage()
        {
            Children.Clear();
            Unit unit = UIController.SelectedUnit;
            if (unit == null)
            {
                throw new ArgumentNullException();
            }
            Register<Item> itemRegister = UIController.GameInstance.Registry.GetRegister<Item>();
            foreach (int index in unit.Inventory)
            {
                Item item = itemRegister[index];
                Children.Add(new ItemSubPage(item));
            }
        }
        internal override bool IsInternal { get { return true; } }
    }
}
