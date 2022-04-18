using System.Threading.Tasks;

namespace Vatsim.Vatis.Client.AudioForVatsim
{
    public interface IAudioManager
    {
        Task AddOrUpdateBot(byte[] audio, string callsign, uint frequency, double lat, double lon);
        Task RemoveBot(string callsign);
    }
}