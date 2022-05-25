using Microsoft.AspNetCore.Components;
using TableApp.Data;
using TableApp.Models;

namespace TableApp.Pages
{
    public partial class RecordsTable: ComponentBase
    {
        [Inject]
        private RecordService _service { get; set; } = null!;


        private List<Record>? _records;
        private bool _isSortedAscending;
        private string? _activeSortColumn = "Id";
        private int? _expandedRecordId;
        private string? _expandedClass = string.Empty;

        #region MyRegion

        int totalPages;
        int totalRecords;
        int curPage;
        int pagerSize;
        int pageSize;
        int startPage;
        int endPage;
        //string sortColumnName = "Id";
        string sortDir = "ASC";

        #endregion


        [Parameter]
        public TableColumn[]? ColumnsSet { get; set; }

        protected override async Task OnInitializedAsync()
        {
            pagerSize = 3;
            pageSize = 10;
            curPage = 1;

            totalRecords = await _service.Count();
            totalPages = (int)Math.Ceiling(totalRecords / (decimal)pageSize);
            SetPagerSize("forward");

            await FetchRecords();
        }

        private async Task FetchRecords()
        {
            _service.GetAllRecords(_activeSortColumn!, sortDir);
            _service.PaginateQuery(curPage, pageSize);
            _records = await _service.GetRecords();
            this.StateHasChanged();
        }

        private async void SortTableAsync(string columnId)
        {
            _expandedRecordId = 0;
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
            sortDir = _isSortedAscending ? "ASC" : "DESC";

            await FetchRecords();
        }

        private string SetSortIcon(string columnId)
        {
            return _activeSortColumn != columnId ? "bi-caret-up" : _isSortedAscending ? "bi-caret-up-fill" : "bi-caret-up-fill rotated";
        }

        public async Task RefreshRecords(int page)
        {
            _expandedRecordId = 0;
            curPage = page;
            await FetchRecords();
        }

        public void SetPagerSize(string direction)
        {
            if (direction == "forward" && endPage < totalPages)
            {
                startPage = endPage + 1;
                if (endPage + pagerSize < totalPages)
                {
                    endPage = startPage + pagerSize - 1;
                }
                else
                {
                    endPage = totalPages;
                }
                this.StateHasChanged();
            }
            else if (direction == "back" && startPage > 1)
            {
                endPage = startPage - 1;
                startPage = -pagerSize;
            }
        }

        public async Task NavigateToPage(string direction)
        {
            if (direction == "next")
            {
                if (curPage < totalPages)
                {
                    if (curPage == endPage)
                    {
                        SetPagerSize("forward");
                    }
                    curPage += 1;
                }
            }
            else if (direction == "previous")
            {
                if (curPage > 1)
                {
                    if (curPage == startPage)
                    {
                        SetPagerSize("back");
                    }
                    curPage -= 1;
                }
            }
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
