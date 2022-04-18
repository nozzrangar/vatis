using System;

namespace Vatsim.Vatis.Client.TextToSpeech
{
    [Serializable]
    public class TextToSpeechRequestDto
    {
        public string Station { get; set; }
        public string Text { get; set; }
        public string Voice { get; set; }
        public string Jwt { get; set; }
    }
}
