namespace Vatsim.Vatis.Client.Args
{
    public class KillRequestReceivedEventArgs
    {
        public string Reason { get; set; }
        public KillRequestReceivedEventArgs(string reason)
        {
            Reason = reason;
        }
    }
}
