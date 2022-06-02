using Bunit;
using TableApp.Models;
using TableApp.Shared;
using Xunit;

namespace TableAppTest
{
    public class PaginationTests
    {
        [Fact]
        public void TestParametersHasRightTypes()
        {
            using var ctx = new TestContext();
            ctx.RenderComponent<RecordsPagination>(parameters => parameters
                 .Add(p => p.PaginationParameters, new Pagination
                 {
                     CurrentPage = 1,
                     PageSize = 10
                 })
                 .Add(p => p.TotalPages, 100)
                 .Add(p => p.OnParamsChangeCallback, e => { })
             );
        }

        [Fact]
        public void TestClickOnPageButtonWorksCorrectly()
        {
            Pagination actualParams = new Pagination
            {
                CurrentPage = 1
            };

            using var ctx = new TestContext();
            var cut = ctx.RenderComponent<RecordsPagination>(parameters => parameters
                .Add(p => p.PaginationParameters, new Pagination
                {
                    CurrentPage = 1,
                    PageSize = 10
                })
                .Add(p => p.TotalPages, 100)
                .Add(p => p.OnParamsChangeCallback, e => actualParams = e));

            int expectedPage = 3;

            var button = cut.FindAll("a")[expectedPage];

            button.Click();

            Assert.Equal(expectedPage, cut.Instance.PaginationParameters.CurrentPage);
            Assert.Equal(expectedPage, actualParams.CurrentPage);
        }
        [Fact]
        public void TestClickOnLastPageButtonWorksCorrectly()
        {
            Pagination actualParams = new Pagination
            {
                CurrentPage = 1
            };

            using var ctx = new TestContext();
            var cut = ctx.RenderComponent<RecordsPagination>(parameters => parameters
                .Add(p => p.PaginationParameters, new Pagination
                {
                    CurrentPage = 1,
                    PageSize = 10
                })
                .Add(p => p.TotalPages, 100)
                .Add(p => p.OnParamsChangeCallback, e => actualParams = e));

            int expectedPage = 100;

            var button = cut.FindAll("a")[7];

            button.Click();

            Assert.Equal(expectedPage, cut.Instance.PaginationParameters.CurrentPage);
            Assert.Equal(expectedPage, actualParams.CurrentPage);
        }

        [Fact]
        public void TestClickOnFirstPageButtonWorksCorrectly()
        {
            Pagination actualParams = new Pagination
            {
                CurrentPage = 1
            };

            using var ctx = new TestContext();
            var cut = ctx.RenderComponent<RecordsPagination>(parameters => parameters
                .Add(p => p.PaginationParameters, new Pagination
                {
                    CurrentPage = 50,
                    PageSize = 10
                })
                .Add(p => p.TotalPages, 100)
                .Add(p => p.OnParamsChangeCallback, e => actualParams = e));

            int expectedPage = 1;

            var button = cut.FindAll("a")[0];

            button.Click();

            Assert.Equal(expectedPage, cut.Instance.PaginationParameters.CurrentPage);
            Assert.Equal(expectedPage, actualParams.CurrentPage);
        }

        [Fact]
        public void TestClickOnNextPagerStateWorksCorrectly()
        {
            using var ctx = new TestContext();
            var cut = ctx.RenderComponent<RecordsPagination>(parameters => parameters
                .Add(p => p.PaginationParameters, new Pagination
                {
                    CurrentPage = 50,
                    PageSize = 10
                })
                .Add(p => p.TotalPages, 100));

            var buttons = cut.FindAll("a");

            int startPage = cut.Instance.PaginationParameters.CurrentPage + 2;

            buttons[7].Click();

            buttons = cut.FindAll("a");

            buttons[0].MarkupMatches("<a class=''>«</a>");
            buttons[1].MarkupMatches("<a>...</a>");

            for (int i = 0; i < 5; i++)
            {
                buttons[2 + i].MarkupMatches($"<a class=''>{startPage + i}</a>");
            }

            buttons[7].MarkupMatches("<a>...</a>");
            buttons[8].MarkupMatches("<a class=''>»</a>");
        }

        [Fact]
        public void TestClickOnPreviousPagerStateWorksCorrectly()
        {
            using var ctx = new TestContext();
            var cut = ctx.RenderComponent<RecordsPagination>(parameters => parameters
                .Add(p => p.PaginationParameters, new Pagination
                {
                    CurrentPage = 50,
                    PageSize = 10
                })
                .Add(p => p.TotalPages, 100));

            var buttons = cut.FindAll("a");

            int endPage = cut.Instance.PaginationParameters.CurrentPage - 2;

            buttons[1].Click();

            buttons = cut.FindAll("a");

            buttons[0].MarkupMatches("<a class=''>«</a>");
            buttons[1].MarkupMatches("<a>...</a>");

            for (int i = 0; i < 5; i++)
            {
                buttons[2 + i].MarkupMatches($"<a class=''>{endPage - 4 + i}</a>");
            }

            buttons[7].MarkupMatches("<a>...</a>");
            buttons[8].MarkupMatches("<a class=''>»</a>");
        }

        [Fact]
        public void TestPageSizeSelectorWorksCorrectly()
        {
            Pagination actualParams = new Pagination
            {
                PageSize = 0
            };

            using var ctx = new TestContext();
            var cut = ctx.RenderComponent<RecordsPagination>(parameters => parameters
                .Add(p => p.PaginationParameters, new Pagination
                {
                    PageSize = 10
                })
                .Add(p => p.OnParamsChangeCallback, e => actualParams = e));

            int expectedSize = 20;

            var select = cut.Find("select");

            select.Change(expectedSize);

            Assert.Equal(expectedSize, cut.Instance.PaginationParameters.PageSize);
            Assert.Equal(expectedSize, actualParams.PageSize);
        }
    }
}