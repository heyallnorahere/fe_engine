using System;
using System.Reflection;

namespace FEEngine
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
            ConstructorInfo constructor = this.VerifyValue(assetLoaderType.GetConstructor(Array.Empty<Type>()));
            Loader = (AssetLoader)constructor.Invoke(Array.Empty<object>());
        }
        public AssetLoader Loader { get; }
    }
    public abstract class AssetLoader
    {
        public abstract void Load(Game game);
        public virtual void Unload() { }
        // might add some utility functions later, however this looks good enough for now
    }
}
