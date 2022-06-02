using Bunit;
using System;
using System.Collections.Generic;
using TableApp.Models;
using TableApp.Shared;
using Xunit;
using Record = TableApp.Models.Record;

namespace TableAppTest
{
    public class TableTests
    {
        readonly TableColumn[] _columns = {
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

        readonly List<Record> _records = new()
        {
            new()
            {
                Id = 2,
                PriceHub = "B",
                TradeDate = new DateTime(2021, 1, 8),
                DeliveryStartDate = new DateTime(2021, 1, 8),
                DeliveryEndDate = new DateTime(2021, 1, 8),
                HighPrice = 2.1,
                LowPrice = 2.1,
                WtdAvgPrice = 2.1,
                Change = 2.1,
                DailyVolume = 2
            },
            new()
            {
                Id = 3,
                PriceHub = "C",
                TradeDate = new DateTime(2021, 1, 9),
                DeliveryStartDate = new DateTime(2021, 1, 9),
                DeliveryEndDate = new DateTime(2021, 1, 9),
                HighPrice = 3.1,
                LowPrice = 3.1,
                WtdAvgPrice = 3.1,
                Change = 3.1,
                DailyVolume = 3
            },
            new()
            {
                Id = 1,
                PriceHub = "A",
                TradeDate = new DateTime(2021, 1, 7),
                DeliveryStartDate = new DateTime(2021, 1, 7),
                DeliveryEndDate = new DateTime(2021, 1, 7),
                HighPrice = 1.1,
                LowPrice = 1.1,
                WtdAvgPrice = 1.1,
                Change = 1.1,
                DailyVolume = 1
            }
        };

        [Fact]
        public void TestParametersHasRightTypes()
        {

            using var ctx = new TestContext();
            ctx.RenderComponent<RecordsTable>(parameters => parameters
                .Add(p => p.Records, _records)
                .Add(p => p.ColumnsSet, _columns)
                .Add(p => p.CurrentPriceHubId, 0)
                .Add(p => p.FilterErrorMessage, "No records")
                .Add(p => p.OnSortCallback, e => { })
            );
        }

        [Fact]
        public void TestSortChangesParameter()
        {
            var actualSorting = new Sorting { ColumnId = "Test", Direction = "Test" };

            using var ctx = new TestContext();
            var cut = ctx.RenderComponent<RecordsTable>(parameters => parameters
                .Add(p => p.ColumnsSet, _columns)
                .Add(p => p.Records, _records)
                .Add(p => p.OnSortCallback, e => actualSorting = e));

            string expectedColumnId = "PriceHub";
            string expectedDirection = "ASC";

            var headers = cut.FindAll("th");
            headers[0].Click();

            Assert.Equal(expectedColumnId, actualSorting.ColumnId);
            Assert.Equal(expectedDirection, actualSorting.Direction);
        }
    }
}
