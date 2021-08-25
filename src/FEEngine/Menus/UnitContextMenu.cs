using System;
using System.Collections.Generic;
using FEEngine.Menus.UnitContextMenuPages;

namespace FEEngine.Menus
{
    /// <summary>
    /// A menu for controlling <see cref="Unit"/>s
    /// </summary>
    public class UnitContextMenu : IMenu
    {
        /// <summary>
        /// A page of a <see cref="UnitContextMenu"/>
        /// </summary>
        public abstract class Page
        {
            public Page()
            {
                mChildren = new();
                mCurrentChildIndex = -1;
                mCurrentSelection = 0;
                CanGoBack = true;
            }
            public void Update()
            {
                if (mCurrentChildIndex != -1)
                {
                    mChildren[mCurrentChildIndex].Update();
                    return;
                }
                UpdatePage();
                if (mCurrentSelection >= mChildren.Count)
                {
                    mCurrentSelection = mChildren.Count - 1;
                }
                InputManager.State state = this.VerifyValue(UIController.GameInstance).InputManager.GetState();
                if (state.Up && mCurrentSelection > 0)
                {
                    mCurrentSelection--;
                }
                if (state.Down && mCurrentSelection < mChildren.Count - 1)
                {
                    mCurrentSelection++;
                }
                if (state.OK)
                {
                    mCurrentChildIndex = mCurrentSelection;
                    mChildren[mCurrentChildIndex].OnSelect();
                }
                if (state.Back)
                {
                    GoBack();
                }
            }
            public void Render(RenderContext context, Vector2 availableSize)
            {
                if (mCurrentChildIndex == -1)
                {
                    RenderPage(context, availableSize);
                }
                else
                {
                    mChildren[mCurrentChildIndex].Render(context, availableSize);
                }
            }
            private void RenderPage(RenderContext context, Vector2 availableSize)
            {
                for (int i = 0; i < mChildren.Count; i++)
                {
                    var position = new Vector2(3, availableSize.Y - ((i + 1) * 2));
                    Color color = Color.White;
                    if (mCurrentSelection == i)
                    {
                        color = Color.Red;
                        context.RenderChar(position - (2, 0), '>', color);
                    }
                    context.RenderString(position, mChildren[i].GetTitle(), color);
                }
            }
            public bool AddChild(Page page)
            {
                if (mChildren.Count >= 5)
                {
                    return false;
                }
                page.Parent = this;
                mChildren.Add(page);
                return true;
            }
            // i meant for this to be protected, but ItemPage.ItemUsePage couldn't access it as protected, so...
            public void GoBack()
            {
                if (CanGoBack)
                {
                    if (Parent == null) {
                        throw new NullReferenceException();
                    }
                    Parent.mCurrentChildIndex = -1;
                }
            }
            public Page? Parent { get; private set; }
            protected virtual void OnSelect() { }
            protected virtual void UpdatePage() { }
            protected abstract string GetTitle();
            private int mCurrentChildIndex, mCurrentSelection;
            private readonly List<Page> mChildren;
            protected List<Page> Children { get => mChildren; }
            protected bool CanGoBack { get; set; }
        }
        private class WaitPage : Page
        {
            protected override string GetTitle()
            {
                return "Wait";
            }
            protected override void OnSelect()
            {
                UIController.SelectedUnit?.Wait();
                GoBack();
                UIController.ResetSelectedUnit();
                UIController.IsUnitContextMenuOpen = false;
            }
        }
        private class BasePage : Page
        {
            public BasePage(UnitContextMenu parent)
            {
                CanGoBack = false;
                mParent = parent;
            }
            protected override void UpdatePage()
            {
                RefreshMenuItems();
                InputManager.State state = this.VerifyValue(UIController.GameInstance).InputManager.GetState();
                if (state.Back && mParent != null)
                {
                    UIController.SelectedUnit?.Move(mParent.OriginalUnitPosition ?? throw new NullReferenceException(), Unit.MovementType.RefundMovement);
                    mParent.OriginalUnitPosition = null;
                    UIController.IsUnitContextMenuOpen = false;
                }
            }
            private void RefreshMenuItems()
            {
                bool goAgain = true;
                while (goAgain)
                {
                    goAgain = false;
                    foreach (Page page in Children)
                    {
                        if (page.GetType().Assembly == GetType().Assembly)
                        {
                            goAgain = true;
                            Children.Remove(page);
                            break; // to stop the enumerator from throwing an exception
                        }
                    }
                }
                var attackPage = new AttackPage();
                Unit selectedUnit = this.VerifyValue(UIController.SelectedUnit);
                if (attackPage.AreUnitsInRange() && selectedUnit.CanAttack)
                {
                    AddChild(attackPage);
                }
                var gambitPage = new GambitPage();
                if (gambitPage.IsAvailable())
                {
                    AddChild(gambitPage);
                }
                if (selectedUnit.Inventory.Count > 0 || selectedUnit.EquippedWeapon != null)
                {
                    AddChild(new ItemPage());
                }
                AddChild(new WaitPage());
            }
            private readonly UnitContextMenu mParent;
            protected override string GetTitle()
            {
                throw new NotImplementedException(); // no ones gonna call this anyway
            }
        }
        public Vector2 MinSize => new(25, 26 - Logger.MaxLogSize);
        internal UnitContextMenu()
        {
            mRenderSize = new Vector2(0);
            OriginalUnitPosition = null;
            mBasePage = new BasePage(this);
        }
        public void Update()
        {
            mBasePage.Update();
        }
        public void Render(RenderContext context)
        {
            if (UIController.IsUnitContextMenuOpen)
            {
                if (mRenderSize.X < MinSize.X || mRenderSize.Y < MinSize.Y)
                {
                    return;
                }
                int xDifference = mRenderSize.X - MinSize.X;
                if (xDifference % 2 > 0)
                {
                    xDifference--;
                }
                int padding = xDifference / 2;
                context.PushPair(new RenderContext.OffsetClipPair()
                {
                    Offset = new Vector2(padding, 0),
                    Clip = mRenderSize - (padding, 0)
                });
                mBasePage.Render(context, (MinSize.X, mRenderSize.Y));
                context.PopPair();
            }
        }
        public bool AddPage(Page page)
        {
            return mBasePage.AddChild(page);
        }
        public void SetSize(Vector2 size)
        {
            mRenderSize = size;
        }
        public string GetTitle()
        {
            return "Unit Context Menu";
        }
        public Vector2? OriginalUnitPosition { private get; set; }
        private Vector2 mRenderSize;
        private readonly Page mBasePage;
    }
}
