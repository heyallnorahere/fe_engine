using Newtonsoft.Json;

namespace FEEngine
{
    /// <summary>
    /// A template class for <see cref="IRegisteredObject{T}"/>
    /// </summary>
    /// <typeparam name="T">The derived class</typeparam>
    public abstract class RegisteredObjectBase<T> : IRegisteredObject<T> where T : RegisteredObjectBase<T>, IRegisteredObject<T>
    {
        [JsonIgnore]
        public int RegisterIndex { get { return mRegisterIndex; } }
        public Register<T> GetRegister()
        {
            return mRegister;
        }
        public void SetRegister(int index, Register<T> register)
        {
            mRegisterIndex = index;
            mRegister = register;
        }
        public virtual void OnDeserialized() { }
        protected int mRegisterIndex;
        protected Register<T> mRegister;
    }
}
