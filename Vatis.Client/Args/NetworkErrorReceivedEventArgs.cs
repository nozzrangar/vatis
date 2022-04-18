using System;

namespace Vatsim.Vatis.Client.Args
{
    public class NetworkErrorReceivedEventArgs : EventArgs
    {
        public string Error { get; set; }
        public NetworkErrorReceivedEventArgs(string error)
        {
            Error = error;
        }
    }
}
