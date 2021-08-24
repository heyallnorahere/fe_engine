using System;
using System.Reflection;

namespace FEEngine.GameLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class AssemblyAssetLoaderAttribute : Attribute
    {
        public AssemblyAssetLoaderAttribute(Type assetLoaderType)
        {
            if (!assetLoaderType.DerivesFrom(typeof(AssetLoader)))
            {
                throw new ArgumentException("The given type does not derive from AssetLoader!");
            }
            mLoaderType = assetLoaderType;
        }
        public AssetLoader CreateLoader()
        {
            Type[] parameterTypes = ParameterTypes ?? Array.Empty<Type>();
            ConstructorInfo constructor = this.VerifyValue(mLoaderType.GetConstructor(parameterTypes));
            object[] parameters = Parameters ?? Array.Empty<object>();
            return (AssetLoader)constructor.Invoke(parameters);
        }
        private readonly Type mLoaderType;
        public Type[]? ParameterTypes { get; set; }
        public object[]? Parameters { get; set; }
    }
    public abstract class AssetLoader
    {
        public void Load(Game game)
        {
            LoadContent(game);
        }
        public void Unload()
        {
            UnloadContent();
        }
        protected abstract void LoadContent(Game game);
        protected virtual void UnloadContent() { }
        // might add some utility functions later, however this looks good enough for now
    }
}
