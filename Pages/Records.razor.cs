using Microsoft.AspNetCore.Components;
using TableApp.Data;
using TableApp.Models;

namespace TableApp.Pages
{
    public partial class Records : ComponentBase
    {
        [Inject]
        private RecordService Service { get; set; } = null!;

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
            await CountPagesAsync();
        }


        protected override async Task OnParametersSetAsync()
        {
            await FetchRecords();
        }

        private async Task FetchRecords()
        {
            Service.GetAllRecords(_activeSortColumn!, _sortDir!);
            Service.PaginateQuery(CurrentPage, PageSize);
            _records = await Service.GetRecords();
            this.StateHasChanged();
        }

        private async Task CountPagesAsync()
        {
            int totalRecords = await Service.Count();
            _totalPages = (int)Math.Ceiling(totalRecords / (decimal)PageSize);
        }

        private void SetSortedColumn(string columnId)
        {
            _activeSortColumn = columnId;
        }

        private void SetSortDirection(string direction)
        {
            _sortDir = direction;
        }
    }
}
