using System.Device.Location;

namespace MobileMilk.Service
{
    public interface ILocationService
    {
        GeoCoordinate TryToGetCurrentLocation();
    }
}
