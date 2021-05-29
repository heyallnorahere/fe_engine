namespace FEEngine
{
    public unsafe class Ref<T> where T : unmanaged
    {
        public Ref(ref T value)
        {
            fixed (T* pointer = &value)
            {
                m = pointer;
            }
        }
        public ref T Value
        {
            get
            {
                return ref *m;
            }
        }
        private readonly T* m;
    }
}
