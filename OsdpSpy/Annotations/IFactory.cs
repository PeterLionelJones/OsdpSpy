namespace OsdpSpy.Annotations
{
    public interface IFactory<out T>
    {
        T Create();
    }
}