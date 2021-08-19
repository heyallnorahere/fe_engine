using System;
using System.Collections.Generic;
using System.Reflection;
using FEEngine.Math;
using FEEngine.Menus;

namespace FEEngine.Launcher
{
    public class GameFrame : IRenderable
    {
        public GameFrame(Dictionary<string, Assembly> assemblies, Game game, Player player, string? filename)
        {
            mAssemblyIndex = 0;
            mSize = new Vec2I(0);
            mGame = game;
            mAssemblies = assemblies;
            mLayout = new BorderLayout();
            SetupBorderLayout();
            mLoadedAssembly = false;
            mOptions = new List<string>();
            if (filename != null)
            {
                LoadAssembly(filename);
            }
            else
            {
                mPlayer = player;
                mPlayer.AddUpdateHook(Update);
                foreach (var pair in mAssemblies)
                {
                    mOptions.Add(pair.Key);
                }
            }
        }
        public IVec2<int> MinSize => mLayout.MinSize;
        public void Render(RenderContext context)
        {
            if (mLoadedAssembly)
            {
                mLayout.Render(context);
            }
            else
            {
                if (mAssemblies.Count <= 0)
                {
                    context.RenderString(new Vec2I(0, mSize.Y - 1), "No assemblies were loaded!");
                }
                else
                {
                    for (int i = 0; i < mOptions.Count; i++)
                    {
                        int x;
                        string text;
                        Color color;
                        if (i == mAssemblyIndex)
                        {
                            text = "> " + mOptions[i];
                            x = 0;
                            color = Color.Red;
                        }
                        else
                        {
                            text = mOptions[i];
                            x = 2;
                            color = Color.White;
                        }
                        int y = mSize.Y - ((i + 1) * 2);
                        IVec2<int> position = new Vec2I(x, y);
                        context.RenderString(position, text, color);
                    }
                }
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
            var attribute = assembly.GetCustomAttribute<AssemblyAssetLoaderAttribute>();
            if (attribute != null)
            {
                Loader = attribute.Loader;
                Loader.Load(mGame);
            }
            else
            {
                throw new ArgumentException("The loaded assembly did not specify an AssetLoader class!");
            }
            mLoadedAssembly = true;
            Register<Map> mapRegister = mGame.Registry.GetRegister<Map>();
            mGame.PhaseManager.CyclePhase(mapRegister[mGame.CurrentMapIndex]);
            if (mPlayer != null)
            {
                mPlayer.RemoveUpdateHook(Update);
                mPlayer = null;
            }
        }
        private void Update()
        {
            var state = InputManager.GetState();
            if (mAssemblyIndex > 0 && state.Up)
            {
                mAssemblyIndex--;
            }
            if (mAssemblyIndex < mOptions.Count - 1 && state.Down)
            {
                mAssemblyIndex++;
            }
            if (state.OK)
            {
                LoadAssembly(mOptions[mAssemblyIndex]);
            }
        }
        public AssetLoader? Loader { get; set; }
        private readonly BorderLayout mLayout;
        private readonly Game mGame;
        private readonly Dictionary<string, Assembly> mAssemblies;
        private bool mLoadedAssembly;
        private IVec2<int> mSize;
        private int mAssemblyIndex;
        private Player? mPlayer;
        private readonly List<string> mOptions;
    }
}
