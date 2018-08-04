namespace TCP_Reader
{
    public interface IDataProvider<T>
    {
        event GenericEvent<T> NextValue;
        event GenericEvent<Header> OnHeaderReceived;
        void Start();
    }
}
