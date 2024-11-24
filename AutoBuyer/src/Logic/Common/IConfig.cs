namespace AutoBuyer.Logic.Common
{
    public interface IConfig
    {
        object this[string key] { get; }

        string Connection { get; }
    }
}