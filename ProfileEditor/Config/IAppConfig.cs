using System.Collections.Generic;

namespace ProfileEditor.Config
{
    public interface IAppConfig
    {
        string AppPath { get; }
        List<AtisComposite> Composites { get; set; }
        void LoadConfig(string path);
        void SaveConfig();
    }
}