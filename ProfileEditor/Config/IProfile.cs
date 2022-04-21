using System.Collections.Generic;

namespace ProfileEditor.Config
{
    public interface IProfile
    {
        string Name { get; set; }
        List<AtisComposite> Composites { get; set; }
    }
}