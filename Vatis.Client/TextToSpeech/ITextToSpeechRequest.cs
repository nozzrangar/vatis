using System.Threading;
using System.Threading.Tasks;

namespace Vatsim.Vatis.Client.TextToSpeech
{
    public interface ITextToSpeechRequest
    {
        Task<byte[]> RequestSynthesizedText(string text, CancellationToken token);
    }
}