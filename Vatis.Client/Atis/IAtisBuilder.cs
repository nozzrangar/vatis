using System.Threading;
using System.Threading.Tasks;
using Vatsim.Vatis.Client.Config;

namespace Vatsim.Vatis.Client.Atis
{
    public interface IAtisBuilder
    {
        Task BuildAtisAsync(AtisComposite composite, CancellationToken cancellationToken);
        void GenerateAcarsText(AtisComposite composite);
    }
}