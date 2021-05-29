using System.Collections.Generic;
using FEEngine.Math;

namespace FEEngine
{
    public interface IMenu
    {
        public IVec2<int> Size { get; }
        public void Render(IVec2<int> originPoint);
    }
    public class UIController
    {
        public struct RenderAgent : IRenderable
        {
            public void Render()
            {
                RenderMenus();
            }
        }
        public static void AddMenu<T>() where T : IMenu, new()
        {
            menus.Add(new T());
        }
        private static void RenderMenus()
        {
            var positionDict = GetPositionDict();
            // todo: render hashtag border
            foreach (KeyValuePair<IMenu, IVec2<int>> pair in positionDict)
            {
                pair.Key.Render(pair.Value);
            }
        }
        private static Dictionary<IMenu, IVec2<int>> GetPositionDict()
        {
            Dictionary<IMenu, IVec2<int>> positionDict = new();
            // todo: iterate through calculate where each menu is rendered
            // each menu should have a space in between, for a hashtag border
            return positionDict;
        }
        private static readonly List<IMenu> menus = new();
    }
}
