using System;

namespace Vatsim.Vatis.Client.Args
{
    public class AtisCompositeDeletedEventArgs : EventArgs
    {
        public string Identifier { get; set; }
        public AtisCompositeDeletedEventArgs(string identifier)
        {
            Identifier = identifier;
        }
    }
}
