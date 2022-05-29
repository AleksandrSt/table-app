using Microsoft.AspNetCore.Components;
using TableApp.Data;
using TableApp.Models;

namespace TableApp.Pages
{
    public partial class Records : ComponentBase
    {
        [Inject]
        private RecordService Service { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        private List<Record>? _records;

        private int _totalPages;

        #region Pagination

        [Parameter]
        [SupplyParameterFromQuery(Name = "page")]
        public int CurrentPage { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "pageSize")]
        public int PageSize { get; set; }

        #endregion

        #region Sorting
        private string? _activeSortColumn = "Id";
        private string? _sortDir = "ASC";
        #endregion

        #region Filters

        private int _currentPriceHubFilter;
        private DateFilter? _currentDateFilter;

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

            await FetchRecords();
        }


        protected override async Task OnParametersSetAsync()
        {
            await FetchRecords();
        }

        private async Task FetchRecords()
        {
            Service.FilterQueryByPriceHub(_currentPriceHubFilter);
            Service.SortQuery(_activeSortColumn!, _sortDir!);
            Service.PaginateQuery(CurrentPage, PageSize);
            await CountPagesAsync();
            _records = await Service.GetRecords();
            Service.ClearFilter();
            this.StateHasChanged();
        }

        private async Task CountPagesAsync()
        {
            int totalRecords = await Service.Count();
            _totalPages = (int)Math.Ceiling(totalRecords / (decimal)PageSize);
        }

        private async Task SortColumns(Sorting sortingParams)
        {
            _activeSortColumn = sortingParams.ColumnId;
            _sortDir = sortingParams.Direction;
            await FetchRecords();
        }

        private async Task PaginateTable(Pagination paginationParams)
        {
            CurrentPage = paginationParams.CurrentPage;
            PageSize = paginationParams.PageSize;
            UpdateQuery();
            await FetchRecords();
        }

        private async Task FilterByPriceHub(int priceHubId)
        {
            _currentPriceHubFilter = priceHubId;
            await FetchRecords();
        }

        private void UpdateQuery()
        {
            Dictionary<string, object> query = new Dictionary<string, object>
            {
                ["page"] = CurrentPage,
                ["pageSize"] = PageSize
            };
            var address = NavigationManager.GetUriWithQueryParameters(query!);
            NavigationManager.NavigateTo(address);
        }
    }
}
