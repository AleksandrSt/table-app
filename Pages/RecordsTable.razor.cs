using Microsoft.AspNetCore.Components;
using TableApp.Data;
using TableApp.Models;

namespace TableApp.Pages
{
    public partial class RecordsTable: ComponentBase
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
        private bool _isSortedAscending;
        private string? _activeSortColumn = "Id";
        private string _sortDir = "ASC";
        #endregion

        #region DateExpanding
        private int? _expandedRecordId;
        private string? _expandedClass = string.Empty;
        #endregion


        [Parameter]
        public TableColumn[]? ColumnsSet { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await FetchRecords();
        }

        private async Task FetchRecords()
        {
            _expandedRecordId = 0;
            
            Service.GetAllRecords(_activeSortColumn!, _sortDir);
            Service.PaginateQuery(CurrentPage, PageSize);
            _records = await Service.GetRecords();
            this.StateHasChanged();
        }

        private async void SortTableAsync(string columnId)
        {
            _expandedClass = columnId == "TradeDate" ? "expanded" : String.Empty;

            if (columnId != _activeSortColumn)
            {
                _isSortedAscending = true;
            }
            else
            {
                if (!_isSortedAscending)
                {
                    columnId = "Id";
                    _expandedClass = String.Empty;
                }
                _isSortedAscending = !_isSortedAscending;
            }
            _activeSortColumn = columnId;
            _sortDir = _isSortedAscending ? "ASC" : "DESC";

            await FetchRecords();
        }

        private void SetExpandedId(int recordId)
        {
            _expandedClass = String.Empty;
            _expandedRecordId = _expandedRecordId != recordId ? recordId : 0;
        }

        private string ExpandDates(int recordId)
        {
            return _expandedRecordId != recordId ? string.Empty : "expanded";
        }

        private string SetExpandIcon(int recordId)
        {
            return _expandedRecordId != recordId ? "bi-calendar-plus" : "bi-calendar-minus";
        }
    }
}
