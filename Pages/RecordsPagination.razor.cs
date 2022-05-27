using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TableApp.Data;
using static System.Int32;

namespace TableApp.Pages
{
    public partial class RecordsPagination : ComponentBase
    {
        [Inject] private RecordService Service { get; set; } = null!;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;


        [Parameter]
        public int CurrentPage { get; set; }

        [Parameter]
        public int PageSize { get; set; }


        #region Pagination variables
        private int _totalPages;
        private int _totalRecords;
        private int _pagerSize;
        private int _startPage;
        private int _endPage;
        #endregion


        public void SetPager(string direction)
        {
            if (direction == "forward" && _endPage < _totalPages)
            {
                _startPage = _endPage++;
                _endPage = SetEndPage();
            }
            else if (direction == "backward" && _startPage > 1)
            {
                _endPage = _startPage--;
                _startPage = SetStartPage();
            }
        }

        public void NavigateToLastPage()
        {
            CurrentPage = _totalPages;
            _endPage = _totalPages;
            _startPage = SetStartPage();
            UpdatePagesQuery();
        }
        public void NavigateToFirstPage()
        {
            CurrentPage = 1;
            _startPage = 1;
            _endPage = SetEndPage();
            UpdatePagesQuery();
        }

        public void SetPage(int page)
        {
            CurrentPage = page;

            if (_endPage - CurrentPage <= 1 && _endPage != _totalPages)
            {
                _startPage++;
                _endPage = SetEndPage();
            }

            if (CurrentPage - _startPage <= 1 && _startPage != 1)
            {
                if (CurrentPage != _totalPages && _endPage - CurrentPage > 1) _endPage--;
                _startPage = SetStartPage();
            }

            UpdatePagesQuery();
        }

        private int SetEndPage() => _startPage + _pagerSize - 1 < _totalPages ? _startPage + _pagerSize - 1 : _totalPages;

        private int SetStartPage() => _endPage - _pagerSize + 1 > 1 ? _endPage - _pagerSize + 1 : 1;

        private void SetPageSize(ChangeEventArgs e)
        {
            string test = e.Value?.ToString() ?? "10";
            PageSize = Parse(test);
            UpdatePagesQuery();
        }

        public void UpdatePagesQuery()
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
