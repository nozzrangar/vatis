namespace Vatsim.Vatis.Client.Config
{
    public interface IConfig
    {
        void LoadConfig(string path);
        void SaveConfig();
    }
}