namespace E_Lang.Tests.Common.Interfaces
{
    public interface IBuilder<TParentBuilder> where TParentBuilder : class
    {
        TParentBuilder Build();
    }
}
