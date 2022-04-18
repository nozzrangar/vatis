using System;

namespace Vatsim.Vatis.Client.Args
{
    public class MetarResponseReceivedEventArgs : EventArgs
    {
        public string Metar { get; set; }
        public bool IsUpdated { get; set; }
        public MetarResponseReceivedEventArgs(string metar, bool isUpdated)
        {
            Metar = metar;
            IsUpdated = isUpdated;
        }
    }
}
