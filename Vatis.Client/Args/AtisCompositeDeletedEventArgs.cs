using System;

namespace Vatsim.Vatis.Client.Args
{
    public class AtisCompositeDeletedEventArgs : EventArgs
    {
        public Guid Id { get; set; }
        public AtisCompositeDeletedEventArgs(Guid id)
        {
            Id = id;
        }
    }
}
