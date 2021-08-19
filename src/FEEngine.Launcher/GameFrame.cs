using System;
using System.Collections.Generic;
using System.Reflection;
using FEEngine.Math;
using FEEngine.Menus;

namespace FEEngine.Launcher
{
    public class GameFrame : IRenderable
    {
        public GameFrame(Dictionary<string, Assembly> assemblies, Game game, string? filename)
        {
            mSize = new Vec2I(0);
            mGame = game;
            mAssemblies = assemblies;
            mLayout = new BorderLayout();
            mLoadedAssembly = false;
            SetupBorderLayout();
            if (filename != null)
            {
                LoadAssembly(filename);
            }
        }
        public IVec2<int> MinSize => mLayout.MinSize;
        private struct SelectableOption
        {
            public string GameFilename { get; set; }
            public string Text { get; set; }
            public IVec2<int> Position { get; set; }
        }
        public void Render(RenderContext context)
        {
            if (mLoadedAssembly)
            {
                mLayout.Render(context);
            }
            else
            {
                var options = new List<SelectableOption>();
                // todo: render menu
            }
        }
        public void SetSize(IVec2<int> size)
        {
            mLayout.SetSize(size);
            mSize = size;
        }
        private void SetupBorderLayout()
        {
            mLayout.Center = new BorderedObject(new Map.MapRenderer(mGame));
            mLayout.AddChild(new BorderedObject(new Logger.RenderAgent()), BorderLayout.Alignment.Bottom);
            mLayout.AddChild(new BorderedMenu(this.VerifyValue(UIController.FindMenu<UnitContextMenu>())), BorderLayout.Alignment.Right);
            mLayout.AddChild(new BorderedMenu(this.VerifyValue(UIController.FindMenu<TileInfoMenu>())), BorderLayout.Alignment.Left);
        }
        private void LoadAssembly(string name)
        {
            Assembly assembly = mAssemblies[name];
            // todo: load content
            mLoadedAssembly = true;
            Register<Map> mapRegister = mGame.Registry.GetRegister<Map>();
            mGame.PhaseManager.CyclePhase(mapRegister[mGame.CurrentMapIndex]);
        }
        private readonly BorderLayout mLayout;
        private readonly Game mGame;
        private readonly Dictionary<string, Assembly> mAssemblies;
        private bool mLoadedAssembly;
        private IVec2<int> mSize;
    }
}
