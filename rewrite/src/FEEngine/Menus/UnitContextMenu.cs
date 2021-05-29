using System.Collections.Generic;
using FEEngine.Math;

namespace FEEngine.Menus
{
    public class UnitContextMenu : IMenu
    {
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
                UpdatePage();
                if (mCurrentSelection >= mChildren.Count)
                {
                    mCurrentSelection = mChildren.Count - 1;
                }
                InputManager.State state = InputManager.GetState();
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
                if (state.Back && CanGoBack)
                {
                    Parent.mCurrentChildIndex = -1;
                }
                if (mCurrentChildIndex != -1)
                {
                    mChildren[mCurrentChildIndex].Update();
                }
            }
            public void Render(IVec2<int> originPoint)
            {
                if (mCurrentChildIndex == -1)
                {
                    RenderPage(originPoint);
                }
                else
                {
                    mChildren[mCurrentChildIndex].Render(originPoint);
                }
            }
            private void RenderPage(IVec2<int> originPoint)
            {
                for (int i = 0; i < mChildren.Count; i++)
                {
                    int yOffset = (i * 2) + 1;
                    IVec2<int> absoluteTextPosition = MathUtil.AddVectors(originPoint, new Vec2I(3, -yOffset));
                    if (mCurrentSelection == i)
                    {
                        Renderer.RenderChar(MathUtil.SubVectors(absoluteTextPosition, new Vec2I(2, 0)), '>', Color.Red);
                    }
                    Renderer.RenderString(absoluteTextPosition, mChildren[i].GetTitle());
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
            protected virtual void OnSelect() { }
            protected abstract void UpdatePage();
            protected abstract string GetTitle();
            private int mCurrentChildIndex, mCurrentSelection;
            private readonly List<Page> mChildren;
            protected Page Parent { get; private set; }
            protected List<Page> Children { get => mChildren; }
            protected bool CanGoBack { get; set; }
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
                // todo: refresh menu items
                InputManager.State state = InputManager.GetState();
                if (state.Back)
                {
                    UIController.SelectedUnit.Move(mParent.OriginalUnitPosition, Unit.MovementType.RefundMovement);
                    mParent.OriginalUnitPosition = null;
                    UIController.IsUnitContextMenuOpen = false;
                }
            }
            private readonly UnitContextMenu mParent;
            protected override string GetTitle() { return null; } // noone's gonna call this anyway
        }
        public IVec2<int> Size { get { return new Vec2I(10, 20); } }
        public UnitContextMenu()
        {
            OriginalUnitPosition = null;
            mBasePage = new BasePage(this);
        }
        public void Update()
        {
            mBasePage.Update();
        }
        public void Render(IVec2<int> originPoint)
        {
            mBasePage.Render(originPoint);
        }
        public bool AddPage(Page page)
        {
            return mBasePage.AddChild(page);
        }
        public IVec2<int> OriginalUnitPosition { private get; set; }
        private readonly Page mBasePage;
    }
}
