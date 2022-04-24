using System.Collections.Generic;

namespace Vatsim.Vatis.Client.Config
{
    public interface IProfileEditorConfig : IConfig
    {
        string AppPath { get; }
        List<AtisComposite> Composites { get; set; }
        void LoadConfig(string path);
        void SaveConfig();
    }
}