namespace E_Lang.Builder.Interfaces
{
    public interface IBuilder<TParentBuilder> where TParentBuilder : class
    {
        TParentBuilder Build();
    }
}
