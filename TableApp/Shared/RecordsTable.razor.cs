using Microsoft.AspNetCore.Components;
using TableApp.Models;

namespace TableApp.Shared
{
    public partial class RecordsTable : ComponentBase
    {
        [Parameter]
        public IEnumerable<Record>? Records { get; set; }

        [Parameter]
        public TableColumn[]? ColumnsSet { get; set; }

        [Parameter]
        public int CurrentPriceHubId { get; set; }

        [Parameter]
        public string? FilterErrorMessage { get; set; }

        #region Callbacks

        [Parameter]
        public EventCallback<Sorting> OnSortCallback { get; set; }

        #endregion

        #region Sorting

        private bool _isSortedAscending;
        private Sorting _sortingParams = new Sorting();

        #endregion

        #region DateExpanding

        private int? _expandedRecordId;
        private string? _expandedClass = string.Empty;

        #endregion

        protected override void OnParametersSet()
        {
            _expandedRecordId = 0;
            if (CurrentPriceHubId > 0) ColumnsSet = ColumnsSet!.Skip(1).ToArray();

            StateHasChanged();
        }

        private async void SortTableAsync(string columnId)
        {
            _expandedClass = columnId == "TradeDate" ? "expanded" : string.Empty;

            if (columnId != _sortingParams.ColumnId)
            {
                _isSortedAscending = true;
            }
            else
            {
                if (!_isSortedAscending)
                {
                    columnId = "Id";
                    _expandedClass = string.Empty;
                }
                _isSortedAscending = !_isSortedAscending;
            }
            _sortingParams.ColumnId = columnId;
            _sortingParams.Direction = _isSortedAscending ? "ASC" : "DESC";

            await OnSortCallback.InvokeAsync(_sortingParams);
        }

        private void SetExpandedId(int recordId)
        {
            _expandedClass = string.Empty;
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

        private string SetSortIcon(string columnId)
        {
            return _sortingParams.ColumnId != columnId ? "bi-caret-up" : _isSortedAscending ? "bi-caret-up-fill" : "bi-caret-up-fill rotated";
        }

        private void ToggleDatesExpanding() => @_expandedClass = _expandedClass == "expanded" ? string.Empty : "expanded";
    }
}
