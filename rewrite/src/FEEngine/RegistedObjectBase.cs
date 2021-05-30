using Newtonsoft.Json;

namespace FEEngine
{
    public abstract class RegistedObjectBase<T> : IRegisteredObject<T> where T : RegistedObjectBase<T>, IRegisteredObject<T>
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
        public virtual void OnDeserialization() { }
        protected int mRegisterIndex;
        protected Register<T> mRegister;
    }
}
