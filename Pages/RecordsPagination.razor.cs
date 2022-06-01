using Microsoft.AspNetCore.Components;
using TableApp.Models;
using static System.Int32;

namespace TableApp.Pages
{
    public partial class RecordsPagination : ComponentBase
    {

        [Parameter] 
        public Pagination PaginationParameters { get; set; } = null!;

        [Parameter]
        public EventCallback<Pagination> OnParamsChangeCallback { get; set; }

        [Parameter]
        public int TotalPages { get; set; }

        #region Variables

        private int _pagerSize;

        private int _startPage;
        private int _endPage;

        #endregion

        protected override async Task OnInitializedAsync()
        {
            _pagerSize = 5;

            await SetPagerOnInitAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            await SetPagerOnInitAsync();
            StateHasChanged();
        }

        private async Task SetPagerOnInitAsync()
        {
            if (PaginationParameters.CurrentPage > TotalPages && TotalPages > 0)
            {
                PaginationParameters.CurrentPage = TotalPages;
                await OnParamsChangeCallback.InvokeAsync(PaginationParameters);
            }
            _startPage = (PaginationParameters.CurrentPage - _pagerSize / 2) < 1 ? 1 : (PaginationParameters.CurrentPage - _pagerSize / 2);
            _endPage = SetEndPage();
        }


        private void SetPager(string direction)
        {
            switch (direction)
            {
                case "forward" when _endPage < TotalPages:
                    _startPage = _endPage++;
                    _endPage = SetEndPage();
                    break;
                case "backward" when _startPage > 1:
                    _endPage = _startPage--;
                    _startPage = SetStartPage();
                    break;
            }
        }
        private async Task NavigateToFirstPage()
        {
            PaginationParameters.CurrentPage = 1;
            _startPage = 1;
            _endPage = SetEndPage();
            await OnParamsChangeCallback.InvokeAsync(PaginationParameters);
        }

        private async Task NavigateToLastPage()
        {
            PaginationParameters.CurrentPage = TotalPages;
            _endPage = TotalPages;
            _startPage = SetStartPage();
            await OnParamsChangeCallback.InvokeAsync(PaginationParameters);
        }

        private async Task SetPage(int page)
        {
            PaginationParameters.CurrentPage = page;

            if (_endPage - PaginationParameters.CurrentPage <= 1 && _endPage != TotalPages)
            {
                _startPage++;
                _endPage = SetEndPage();
            }

            if (PaginationParameters.CurrentPage - _startPage <= 1 && _startPage != 1)
            {
                if (PaginationParameters.CurrentPage != TotalPages && _endPage - PaginationParameters.CurrentPage > 1) _endPage--;
                _startPage = SetStartPage();
            }
            await OnParamsChangeCallback.InvokeAsync(PaginationParameters);
        }

        private int SetEndPage() => _startPage + _pagerSize - 1 < TotalPages ? _startPage + _pagerSize - 1 : TotalPages;

        private int SetStartPage() => _endPage - _pagerSize + 1 > 1 ? _endPage - _pagerSize + 1 : 1;

        private async Task SetPageSize(ChangeEventArgs e)
        {
            string pageSize = e.Value?.ToString() ?? "10";
            PaginationParameters.PageSize = Parse(pageSize);

            await OnParamsChangeCallback.InvokeAsync(PaginationParameters);
        }
    }
}
