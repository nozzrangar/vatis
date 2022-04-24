using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vatsim.Vatis.Client.Config
{
    internal class ProfileEditorConfig : IProfileEditorConfig
    {
        [JsonIgnore] public string AppPath { get; }

        [JsonIgnore] public string ConfigPath { get; }

        public List<AtisComposite> Composites { get; set; } = new List<AtisComposite>();

        public ProfileEditorConfig()
        {
            AppPath = Path.GetDirectoryName(Environment.ProcessPath);
            ConfigPath = Path.Combine(AppPath, "ProfileEditorConfig.json");

            try
            {
                LoadConfig(ConfigPath);
            }
            catch (FileNotFoundException)
            {
                SaveConfig();
            }
            catch (Exception)
            {
                SaveConfig();
            }
        }

        public void LoadConfig(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(fs))
                {
                    JsonConvert.PopulateObject(sr.ReadToEnd(), this);
                }
            }
        }

        public void SaveConfig()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new OrderedContractResolver(),
            };
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented, jsonSerializerSettings));
        }
        private class OrderedContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var @base = base.CreateProperties(type, memberSerialization);
                var ordered = @base
                    .OrderBy(p => p.Order ?? int.MaxValue)
                    .ThenBy(p => p.PropertyName)
                    .ToList();
                return ordered;
            }
        }
    }
}