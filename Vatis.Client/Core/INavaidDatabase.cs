namespace Vatsim.Vatis.Client.Core
{
    public interface INavaidDatabase
    {
        Airport GetAirport(string id);
        Navaid GetNavaid(string id);
    }
}