namespace ObjectDiffer
{
    public interface IDiffer
    {
        Difference Diff<T>(T newObj, T oldObj);
    }
}
