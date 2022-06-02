using Bunit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TableApp.DataInterfaces;
using TableApp.Models;
using TableApp.Shared;
using Telerik.JustMock;
using Xunit;

namespace TableAppTest
{
    public class FiltersTests
    {
        [Fact]
        public void TestParametersHasRightTypes()
        {
            using var ctx = GetTestContextWithService();

            ctx.RenderComponent<RecordsFilters>(parameters => parameters
                 .Add(p => p.DateType, "TradeDate")
                 .Add(p => p.From, new DateTime(2001, 1, 1))
                 .Add(p => p.To, new DateTime(2001, 1, 1))
                 .Add(p => p.OnDateRangeSetCallback, _ => { })
                 .Add(p => p.OnPriceHubSetCallback, _ => { })
             );
        }

        #region Price Hub Filter tests

        [Fact]
        public void TestSelectionIsEmptyOnNullResponse()
        {
            using var ctx = GetTestContextWithService();

            int expectedCount = 0;

            var cut = ctx.RenderComponent<RecordsFilters>();

            var options = cut.FindAll("option");

            Assert.Equal(expectedCount, options.Count);
        }

        [Fact]
        public void TestSelectionHasOptionsFromServer()
        {
            List<PriceHub> response = new() { new PriceHub { Id = 1, Name = "Price hub 1" } };

            var ctx = GetTestContextWithService(response);

            // All selection + option from response
            int expectedCount = 2;

            var cut = ctx.RenderComponent<RecordsFilters>();

            var options = cut.FindAll("option");

            var select = cut.Find("select");

            var expectedSelection = "<select class='form-select' value='0'>" +
                                    "<option value='0' selected>All</option>" +
                                    "<option value='1'>Price hub 1</option>" +
                                    "</select>";

            Assert.Equal(expectedCount, options.Count);
            select.MarkupMatches(expectedSelection);
        }

        [Fact]
        public void TestSelectPriceHubWorksCorrectly()
        {
            List<PriceHub> response = new()
            {
                new PriceHub { Id = 1, Name = "Price hub 1" },
                new PriceHub { Id = 1, Name = "Price hub 2" }
            };

            using var ctx = GetTestContextWithService(response);

            int actualPriceHub = 0;

            var cut = ctx.RenderComponent<RecordsFilters>(parameters => parameters
            .Add(p => p.OnPriceHubSetCallback, e => actualPriceHub = e));

            int expectedPriceHub = 1;

            var select = cut.Find("select");
            select.Change(expectedPriceHub);

            Assert.Equal(expectedPriceHub, cut.Instance.CurrentPriceHubId);
            Assert.Equal(expectedPriceHub, actualPriceHub);
        }

        #endregion

        #region Date Range Filter tests

        [Fact]
        public void TestDateTypeButtonsChangeDateType()
        {
            var ctx = GetTestContextWithService();

            var cut = ctx.RenderComponent<RecordsFilters>(parameters => parameters
                .Add(p => p.DateType, "Test"));

            string expectedDateType = "DeliveryStartDate";

            var buttons = cut.FindAll("input");

            buttons[1].Change(true);

            Assert.Equal(expectedDateType, cut.Instance.DateType);
        }

        [Fact]
        public void TestFromInputAppliesFilter()
        {
            var ctx = GetTestContextWithService();

            DateFilter actualDateFilter = new()
            {
                DateType = "Test",
                From = DateTime.MinValue,
                To = DateTime.MaxValue
            };

            var cut = ctx.RenderComponent<RecordsFilters>(parameters => parameters
                .Add(p => p.DateType, "TradeDate")
                .Add(p => p.From, null)
                .Add(p => p.To, null)
                .Add(p => p.OnDateRangeSetCallback, e => actualDateFilter = e));

            DateTime expectedFromDate = new DateTime(2021, 1, 7);

            var inputs = cut.FindAll("input");
            inputs[3].Change(expectedFromDate.ToString("yyyy-MM-dd"));

            Assert.Equal(expectedFromDate, cut.Instance.From);
            Assert.Equal(expectedFromDate, actualDateFilter.From);
        }

        [Fact]
        public void TestToInputAppliesFilter()
        {
            var ctx = GetTestContextWithService();

            DateFilter actualDateFilter = new()
            {
                DateType = "Test",
                From = DateTime.MinValue,
                To = DateTime.MaxValue
            };

            var cut = ctx.RenderComponent<RecordsFilters>(parameters => parameters
                .Add(p => p.DateType, "TradeDate")
                .Add(p => p.From, null)
                .Add(p => p.To, null)
                .Add(p => p.OnDateRangeSetCallback, e => actualDateFilter = e));

            DateTime expectedToDate = new DateTime(2021, 1, 7);

            var inputs = cut.FindAll("input");
            inputs[4].Change(expectedToDate.ToString("yyyy-MM-dd"));

            Assert.Equal(expectedToDate, cut.Instance.To);
            Assert.Equal(expectedToDate, actualDateFilter.To);
        }

        #endregion

        public TestContext GetTestContextWithService(IEnumerable<PriceHub>? response = null)
        {
            using var ctx = new TestContext();

            var serviceMock = Mock.Create<IPriceHubsService>();

            Mock.Arrange(() => serviceMock.GetPriceHubsAsync())
                .Returns(response == null
                    ? new TaskCompletionSource<IEnumerable<PriceHub>>().Task
                    : Task.FromResult(response));

            ctx.Services.AddSingleton(serviceMock);

            return ctx;
        }
    }
}
