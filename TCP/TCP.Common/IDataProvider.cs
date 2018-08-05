namespace TCP.Common
{
    public interface IDataProvider<T>
    {
        event GenericEvent<T> NextValue;
        event GenericEvent<Header> OnHeaderReceived;
        void Start();
    }
}
