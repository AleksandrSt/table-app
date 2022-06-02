using TableApp.Models;

namespace TableApp.DataInterfaces
{
    public interface IPriceHubsService
    {
        Task<IEnumerable<PriceHub>> GetPriceHubsAsync();
    }
}
