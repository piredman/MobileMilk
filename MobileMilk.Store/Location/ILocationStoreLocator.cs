using MobileMilk.Store.Location;

namespace MobileMilk.Store
{
    public interface ILocationStoreLocator
    {
        ILocationStore GetStore();
    }
}
