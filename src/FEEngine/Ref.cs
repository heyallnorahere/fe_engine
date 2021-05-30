namespace FEEngine
{
    /// <summary>
    /// An unsafe class for storing references to unmanaged objects
    /// </summary>
    /// <typeparam name="T">The type of reference to store</typeparam>
    public unsafe class Ref<T> where T : unmanaged
    {
        public Ref(ref T value)
        {
            fixed (T* pointer = &value)
            {
                m = pointer;
            }
        }
        /// <summary>
        /// Gets the reference from the stored pointer
        /// </summary>
        public ref T Value
        {
            get
            {
                return ref *m;
            }
        }
        private readonly T* m;
        public static implicit operator T(Ref<T> @ref) => @ref.Value;
        public static implicit operator Ref<T>(T* pointer) => new(ref *pointer);
    }
}
