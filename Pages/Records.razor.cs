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

        #region Pagination
        [Parameter]
        [SupplyParameterFromQuery(Name = "page")]
        public int CurrentPage { get; set; }
        [Parameter]
        [SupplyParameterFromQuery(Name = "pageSize")]
        public int PageSize { get; set; }
        #endregion

        #region Sorting
        //private bool _isSortedAscending;
        private string? _activeSortColumn = "Id";
        private string? _sortDir = "ASC";
        #endregion

        //#region DateExpanding
        //private int? _expandedRecordId;
        //private string? _expandedClass = string.Empty;
        //#endregion


        //[Parameter]
        //public TableColumn[]? ColumnsSet { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await FetchRecords();
        }

        private async Task FetchRecords()
        {
            Service.GetAllRecords(_activeSortColumn!, _sortDir);
            Service.PaginateQuery(CurrentPage, PageSize);
            _records = await Service.GetRecords();
            this.StateHasChanged();
        }
        
        private void SetSortedColumn(string columnId)
        {
            _activeSortColumn = columnId;
        }

        private void SetSortDirection(string direction)
        {
            _sortDir = direction;
        }

        //private async void SortTableAsync(string columnId)
        //{
        //    _expandedClass = columnId == "TradeDate" ? "expanded" : String.Empty;

        //    if (columnId != _activeSortColumn)
        //    {
        //        _isSortedAscending = true;
        //    }
        //    else
        //    {
        //        if (!_isSortedAscending)
        //        {
        //            columnId = "Id";
        //            _expandedClass = String.Empty;
        //        }
        //        _isSortedAscending = !_isSortedAscending;
        //    }
        //    _activeSortColumn = columnId;
        //    _sortDir = _isSortedAscending ? "ASC" : "DESC";

        //    await FetchRecords();
        //}
    }
}
