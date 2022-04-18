using System;
using System.IO;

namespace Vatsim.Vatis.Client.Args
{
    public class RecordedAtisChangedEventArgs : EventArgs
    {
        public MemoryStream AtisMemoryStream { get; set; }
        public RecordedAtisChangedEventArgs(MemoryStream stream)
        {
            AtisMemoryStream = stream;
        }
    }
}
