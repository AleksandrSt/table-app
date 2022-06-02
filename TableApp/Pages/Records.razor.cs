using Microsoft.AspNetCore.Components;
using TableApp.DataInterfaces;
using TableApp.Models;

namespace TableApp.Pages
{
    public partial class Records : ComponentBase
    {
        [Inject]
        private IRecordService Service { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        private IEnumerable<Record>? _records;

        private int _totalPages;

        #region Pagination

        [Parameter]
        [SupplyParameterFromQuery(Name = "page")]
        public int CurrentPage { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "page_size")]
        public int PageSize { get; set; }

        #endregion

        #region Sorting
        private string? _activeSortColumn = "Id";
        private string? _sortDir = "ASC";
        #endregion

        #region Filters

        [Parameter]
        [SupplyParameterFromQuery(Name = "price_hub_id")]
        public int PriceHubId { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "date_type")]
        public string? DateType { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "from")]
        public DateTime? From { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "to")]
        public DateTime? To { get; set; }

        private string? _filterErrorMessage;

        #endregion

        readonly TableColumn[]? _columns = {
            new()
            {
                ColumnId = "PriceHub",
                ColumnName = "Price hub"
            },
            new()
            {
                ColumnId = "HighPrice",
                ColumnName = "High price $/MWh"
            },
            new()
            {
                ColumnId = "LowPrice",
                ColumnName = "Low price $/MWh"
            },
            new()
            {
                ColumnId = "WtdAvgPrice",
                ColumnName = "Wth avg price $/MWh"
            },
            new()
            {
                ColumnId = "Change",
                ColumnName = "Change"
            },
            new()
            {
                ColumnId = "DailyVolume",
                ColumnName = "Daily volume MWh"
            },
            new()
            {
                ColumnId = "TradeDate",
                ColumnName = "Dates"
            }
        };

        protected override async Task OnInitializedAsync()
        {
            PageSize = PageSize > 0 ? PageSize : 10;
            CurrentPage = CurrentPage > 0 ? CurrentPage : 1;

            await FetchRecordsAsync();
        }


        protected override async Task OnParametersSetAsync()
        {
            await FetchRecordsAsync();
        }

        private async Task FetchRecordsAsync()
        {
            Service.FilterQueryByPriceHub(PriceHubId);
            Service.FilterQueryByDate(DateType!, From, To);
            Service.SortQuery(_activeSortColumn!, _sortDir!);
            Service.PaginateQuery(CurrentPage, PageSize);
            await CountPagesAsync();
            _records = await Service.GetRecordsAsync();
            Service.ClearFilter();
            StateHasChanged();
        }

        private async Task CountPagesAsync()
        {
            int totalRecords = await Service.CountAsync();
            _totalPages = (int)Math.Ceiling(totalRecords / (decimal)PageSize);
        }

        private async Task SortColumnsAsync(Sorting sortingParams)
        {
            _activeSortColumn = sortingParams.ColumnId;
            _sortDir = sortingParams.Direction;
            await FetchRecordsAsync();
        }

        private async Task PaginateTableAsync(Pagination paginationParams)
        {
            CurrentPage = paginationParams.CurrentPage;
            PageSize = paginationParams.PageSize;
            UpdateQuery();
            await FetchRecordsAsync();
        }

        private async Task FilterByPriceHubAsync(int priceHubId)
        {
            PriceHubId = priceHubId;
            UpdateQuery();
            await FetchRecordsAsync();
        }

        private async Task FilterByDateAsync(DateFilter dateFilterParams)
        {
            DateType = dateFilterParams.DateType;
            From = dateFilterParams.From;
            To = dateFilterParams.To;

            _filterErrorMessage = From != null && To != null && To < From
                ? "End date can not be earlier that start date"
                : null;

            UpdateQuery();
            await FetchRecordsAsync();
        }

        private void UpdateQuery()
        {
            Dictionary<string, object?> query = new Dictionary<string, object?>
            {
                ["page"] = CurrentPage,
                ["page_size"] = PageSize,
                ["price_hub_id"] = PriceHubId,
                ["date_type"] = DateType,
                ["from"] = From,
                ["to"] = To
            };

            var address = NavigationManager.GetUriWithQueryParameters(query!);
            NavigationManager.NavigateTo(address);
        }
    }
}
