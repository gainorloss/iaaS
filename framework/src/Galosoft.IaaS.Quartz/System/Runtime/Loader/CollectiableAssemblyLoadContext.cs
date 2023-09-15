namespace System.Runtime.Loader
{
    public class CollectiableAssemblyLoadContext
        : AssemblyLoadContext
    {
        public CollectiableAssemblyLoadContext()
            : base(isCollectible: true)
        { }
    }
}
