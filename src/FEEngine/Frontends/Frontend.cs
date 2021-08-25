using System;
using System.Collections.Generic;
using System.Reflection;

namespace FEEngine.Frontends
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    internal sealed class RegisteredFrontendAttribute : Attribute
    {
        public RegisteredFrontendAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
    internal abstract class Frontend
    {
        public abstract Renderer CreateRenderer();
        public abstract InputManager CreateInputManager();
        public abstract void SetUpdateCallback(Action<Player> update);
        public abstract void SetRenderCallback(Action render);
        public abstract void Run(Player player);
        static Frontend()
        {
            mFrontends = new Dictionary<string, ConstructorInfo>();
            var assembly = typeof(Frontend).Assembly;
            foreach (Type type in assembly.GetTypes())
            {
                var attribute = type.GetCustomAttribute<RegisteredFrontendAttribute>();
                if (attribute != null)
                {
                    var constructor = type.GetConstructor(Array.Empty<Type>());
                    if (constructor != null)
                    {
                        mFrontends.Add(attribute.Name, constructor);
                    }
                }
            }
        }
        public static Frontend Create(string name)
        {
            ConstructorInfo constructor = mFrontends[name];
            return (Frontend)constructor.Invoke(Array.Empty<object>());
        }
        private static readonly Dictionary<string, ConstructorInfo> mFrontends;
    }
}
