using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TableApp.Data;
using static System.Int32;

namespace TableApp.Pages
{
    public partial class RecordsPagination : ComponentBase
    {

        [Inject] 
        private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter]
        public int TotalPages { get; set; }

        [Parameter]
        public int CurrentPage { get; set; }

        [Parameter]
        public int PageSize { get; set; }

        #region Variables

        private int _pagerSize;
        private int _startPage;
        private int _endPage;

        #endregion


        protected override void OnInitialized()
        {
            _pagerSize = 5;

            _startPage = (CurrentPage - _pagerSize / 2) < 1 ? 1 : (CurrentPage - _pagerSize / 2);
            _endPage = SetEndPage();
        }

        protected override void OnParametersSet()
        {
            this.StateHasChanged();
        }

        public void SetPager(string direction)
        {
            if (direction == "forward" && _endPage < TotalPages)
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
            CurrentPage = TotalPages;
            _endPage = TotalPages;
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

            if (_endPage - CurrentPage <= 1 && _endPage != TotalPages)
            {
                _startPage++;
                _endPage = SetEndPage();
            }

            if (CurrentPage - _startPage <= 1 && _startPage != 1)
            {
                if (CurrentPage != TotalPages && _endPage - CurrentPage > 1) _endPage--;
                _startPage = SetStartPage();
            }

            UpdatePagesQuery();
        }

        private int SetEndPage() => _startPage + _pagerSize - 1 < TotalPages ? _startPage + _pagerSize - 1 : TotalPages;

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
