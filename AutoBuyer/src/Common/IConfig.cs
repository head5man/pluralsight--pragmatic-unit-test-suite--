namespace AutoBuyer.Common
{
    public interface IConfig
    {
        object this[string key] { get; }

        string Connection { get; }
    }
}