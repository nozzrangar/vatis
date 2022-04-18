using MetarDecoder;

namespace Vatsim.Vatis.Client.Atis
{
    public abstract class AtisMeta
    {
        public string TextToSpeech { get; set; }
        public string Acars { get; set; }
        public abstract void Parse(DecodedMetar metar);
    }
}
