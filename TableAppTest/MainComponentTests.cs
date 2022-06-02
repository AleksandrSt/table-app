using Bunit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TableApp.DataInterfaces;
using TableApp.Models;
using TableApp.Pages;
using TableApp.Shared;
using Telerik.JustMock;
using Xunit;
using Record = TableApp.Models.Record;

namespace TableAppTest
{
    public class MainComponentTests
    {
        [Fact]
        public void TestParametersHasRightTypes()
        {
            using var ctx = GetTestContextWithService();

            ctx.RenderComponent<Records>(parameters => parameters
                .Add(p => p.CurrentPage, 1)
                .Add(p => p.PageSize, 10)
                .Add(p => p.PriceHubId, 0)
                .Add(p => p.DateType, "Test")
                .Add(p => p.From, new DateTime(2021, 12, 11))
                .Add(p => p.To, new DateTime(2021, 12, 11))
            );
        }

        [Fact]
        public void TestMarkupWhenDataIsNull()
        {
            var ctx = GetTestContextWithService();

            var cut = ctx.RenderComponent<Records>();

            cut.MarkupMatches("<div class='lds-dual-ring'></div>");
        }

        [Fact]
        public void TestInitializationWhenDataIsDefined()
        {
            List<Record> response = new()
            {
                new()
                {
                    Id = 1,
                    PriceHub = "Test",
                    TradeDate = new DateTime(2021, 1, 7),
                    DeliveryStartDate = new DateTime(2021, 1, 8),
                    DeliveryEndDate = new DateTime(2021, 1, 8),
                    HighPrice = 1.1,
                    LowPrice = 2.1,
                    WtdAvgPrice = 3.1,
                    Change = 1.68,
                    DailyVolume = 13000
                }
            };

            TableColumn[] columns = {
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

            var ctx = GetTestContextWithService(response);

            var cut = ctx.RenderComponent<Records>();
            var actualTable = cut.FindComponent<RecordsTable>();

            var expectedTable = ctx.RenderComponent<RecordsTable>(parameters =>
                parameters.Add(p => p.Records, response)
                    .Add(p => p.ColumnsSet, columns));

            actualTable.MarkupMatches(expectedTable.Markup);
        }


        public TestContext GetTestContextWithService(IEnumerable<Record>? response = null)
        {
            using var ctx = new TestContext();

            var recordServiceMock = Mock.Create<IRecordService>();
            var priceHubsServiceMock = Mock.Create<IPriceHubsService>();

            Mock.Arrange(() => recordServiceMock.GetRecordsAsync())
                .Returns(response == null
                    ? new TaskCompletionSource<IEnumerable<Record>>().Task
                    : Task.FromResult(response));

            Mock.Arrange(() => priceHubsServiceMock.GetPriceHubsAsync())
                .Returns(new TaskCompletionSource<IEnumerable<PriceHub>>().Task);

            ctx.Services.AddSingleton(recordServiceMock);
            ctx.Services.AddSingleton(priceHubsServiceMock);

            return ctx;
        }
    }
}
