using System.Collections.Generic;

namespace Vatsim.Vatis.Client.Config
{
    public interface IProfile
    {
        string Name { get; set; }
        List<AtisComposite> Composites { get; set; }
    }
}