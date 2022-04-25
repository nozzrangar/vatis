using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vatsim.Vatis.Client.Args
{
    public class ClientEventArgs<T> : EventArgs
    {
        public T Value { get; set; }

        public ClientEventArgs(T value)
        {
            Value = value;
        }
    }
}
