using MetarDecoder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Vatsim.Vatis.Client.Config
{
    public class AtisComposite : IAtisComposite
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public List<ContractionMeta> Contractions { get; set; } = new List<ContractionMeta>();
        public int AtisFrequency { get; set; } = 18000;
        public ObservationTimeMeta ObservationTime { get; set; } = new ObservationTimeMeta();
        public MagneticVariationMeta MagneticVariation { get; set; } = new MagneticVariationMeta();
        public AtisVoiceMeta AtisVoice { get; set; } = new AtisVoiceMeta();
        public string IDSEndpoint { get; set; }
        public List<AtisPreset> Presets { get; set; } = new List<AtisPreset>();
        public List<DefinedText> AirportConditionDefinitions { get; set; } = new List<DefinedText>();
        public List<DefinedText> NotamDefinitions { get; set; } = new List<DefinedText>();
        public List<TransitionLevel> TransitionLevels { get; set; } = new List<TransitionLevel>();

        [JsonIgnore] public DecodedMetar DecodedMetar { get; set; }
        [JsonIgnore] public AtisPreset CurrentPreset { get; set; }
        [JsonIgnore] public string CurrentAtisLetter { get; set; }
        [JsonIgnore] public string AcarsText { get; set; }
        [JsonIgnore] public uint AfvFrequency => ((uint)AtisFrequency + 100000) * 1000;
        [JsonIgnore] public string AtisCallsign { get; set; }

        internal AtisComposite Clone()
        {
            return new AtisComposite
            {
                Contractions = Contractions,
                AtisFrequency = AtisFrequency,
                ObservationTime = ObservationTime,
                MagneticVariation = MagneticVariation,
                AirportConditionDefinitions = AirportConditionDefinitions,
                AtisVoice = AtisVoice,
                Presets = Presets,
                IDSEndpoint = IDSEndpoint
            };
        }
    }

    [Serializable]
    public class AtisVoiceMeta
    {
        public bool UseTextToSpeech { get; set; } = true;
        public string Voice { get; set; } = "Default";
        [JsonIgnore]
        public string GetVoiceNameForRequest
        {
            get
            {
                switch (Voice)
                {
                    case "Default":
                        return "default";
                    case "US Male":
                        return "us-male";
                    case "US Female":
                        return "us-female";
                    case "UK Male":
                        return "uk-male";
                    case "UK Female":
                        return "uk-female";
                    default:
                        return "default";
                }
            }
        }
    }

    [Serializable]
    public class ContractionMeta
    {
        public string String { get; set; }
        public string Spoken { get; set; }
    }

    [Serializable]
    public class MagneticVariationMeta
    {
        public bool Enabled { get; set; } = false;
        public int MagneticDegrees { get; set; } = 0;
    }

    [Serializable]
    public class ObservationTimeMeta
    {
        public bool Enabled { get; set; } = false;
        public uint Time { get; set; } = 0;
    }

    [Serializable]
    public class DefinedText
    {
        public int Ordinal { get; set; }
        public string Text { get; set; }
        public bool Enabled { get; set; }
    }

    [Serializable]
    public class TransitionLevel
    {
        public int Low { get; set; }
        public int High { get; set; }
        public int Altitude { get; set; }
    }
}
