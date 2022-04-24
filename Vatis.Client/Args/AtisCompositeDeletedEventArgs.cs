using System;

namespace Vatsim.Vatis.Client.Args
{
    public class AtisCompositeDeletedEventArgs : EventArgs
    {
        public Guid Identifier { get; set; }
        public AtisCompositeDeletedEventArgs(Guid identifier)
        {
            Identifier = identifier;
        }
    }
}
