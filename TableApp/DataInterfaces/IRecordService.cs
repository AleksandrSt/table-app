using TableApp.Models;

namespace TableApp.DataInterfaces
{
    public interface IRecordService
    {
        Task<IEnumerable<Record>> GetRecordsAsync();

        void SortQuery(string orderBy, string sort);

        void PaginateQuery(int pageNumber, int itemsOnPage);

        void FilterQueryByPriceHub(int priceHubId);

        void FilterQueryByDate(string? dateType, DateTime? from = null, DateTime? to = null);

        void ClearFilter();

        Task<int> CountAsync();
    }
}
