using Newtonsoft.Json;
using System;

namespace ProfileEditor.Config
{
    [Serializable]
    public class AtisPreset : IAtisProfile
    {
        public string Name { get; set; }
        public string AirportConditions { get; set; }
        public string Notams { get; set; }
        public string ArbitraryText { get; set; }
        public string Template { get; set; }
        public override string ToString() => Name;

        [JsonIgnore] public bool IsAirportConditionsDirty { get; set; }
        [JsonIgnore] public bool IsNotamsDirty { get; set; }
        [JsonIgnore] public bool IsTemplateDirty { get; set; }

        internal AtisPreset Clone()
        {
            return new AtisPreset
            {
                AirportConditions = AirportConditions,
                Notams = Notams,
                ArbitraryText = ArbitraryText,
                Template = Template
            };
        }
    }
}
