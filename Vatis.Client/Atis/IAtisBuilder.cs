using System.Threading;
using System.Threading.Tasks;
using Vatsim.Vatis.Client.Config;

namespace Vatsim.Vatis.Client.Atis
{
    public interface IAtisBuilder
    {
        Task BuildAutomaticAtisAsync(AtisComposite composite, CancellationToken token);
        Task BuildManualAtisAsync(AtisComposite composite, byte[] memoryStream, CancellationToken token);
    }
}