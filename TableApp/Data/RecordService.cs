using System.Data.SqlClient;
using TableApp.DataInterfaces;
using TableApp.Models;

namespace TableApp.Data
{
    public class RecordService : IRecordService
    {
        private readonly IConfiguration _configuration;

        private readonly string? _baseQuery;
        private string? _sort;
        private string? _filters = string.Empty;
        private string? _pagination;


        public RecordService(IConfiguration configuration)
        {
            _configuration = configuration;

            _baseQuery = "SELECT " +
                     "ice_electric2021final.Id, " +
                     "price_hubs.Name AS PriceHub, " +
                     "TradeDate, " +
                     "DeliveryStartDate, " +
                     "DeliveryEndDate, " +
                     "HighPrice, " +
                     "LowPrice, " +
                     "WtdAvgPrice, " +
                     "Change, " +
                     "DailyVolume " +
                     "from ice_electric2021final " +
                     "INNER JOIN price_hubs ON ice_electric2021final.PriceHubId = price_hubs.Id ";
        }

        public async Task<IEnumerable<Record>> GetRecordsAsync()
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");
            List<Record> records = new List<Record>();

            await using SqlConnection connection = new SqlConnection(conStr);

            await using SqlCommand command = new SqlCommand(_baseQuery
                                                            + _filters
                                                            + _sort
                                                            + _pagination);

            command.Connection = connection;

            connection.Open();

            await using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    records.Add(new Record
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        PriceHub = reader["PriceHub"].ToString()!,
                        TradeDate = Convert.ToDateTime(reader["TradeDate"]),
                        DeliveryStartDate = Convert.ToDateTime(reader["DeliveryStartDate"]),
                        DeliveryEndDate = Convert.ToDateTime(reader["DeliveryEndDate"]),
                        HighPrice = Convert.ToDouble(reader["HighPrice"]),
                        LowPrice = Convert.ToDouble(reader["LowPrice"]),
                        WtdAvgPrice = Convert.ToDouble(reader["WtdAvgPrice"]),
                        Change = Convert.ToDouble(reader["Change"]),
                        DailyVolume = Convert.ToInt32(reader["DailyVolume"])
                    });
                }
            }

            connection.Close();

            return records;
        }

        public void SortQuery(string orderBy, string sort)
        {
            _sort = $"ORDER BY {orderBy} {sort} ";
        }

        public void PaginateQuery(int pageNumber, int itemsOnPage)
        {
            _pagination = $"OFFSET {(pageNumber - 1) * itemsOnPage} ROWS FETCH NEXT {itemsOnPage} ROWS ONLY";
        }

        public void FilterQueryByPriceHub(int priceHubId)
        {
            if (priceHubId <= 0) return;

            _filters += _filters == string.Empty ? "WHERE " : "AND ";
            _filters += $"PriceHubId = {priceHubId} ";
        }
        public void FilterQueryByDate(string? dateType, DateTime? from = null, DateTime? to = null)
        {
            if (dateType == null) return;

            DateTime minDate = new DateTime(1753, 1, 1);
            // minimum date value in SQL
            from = from == null || from < minDate ? minDate : from;
            to ??= DateTime.MaxValue;

            _filters += _filters == string.Empty ? "WHERE " : "AND ";

            _filters += $"{dateType} >= '{from.Value:yyyyMMdd}' ";
            _filters += $"AND {dateType} <= '{to.Value:yyyyMMdd}' ";
        }

        public void ClearFilter() => _filters = string.Empty;

        public async Task<int> CountAsync()
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            await using SqlConnection connection = new SqlConnection(conStr);

            await using SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM ice_electric2021final "
                                                            + _filters);

            command.Connection = connection;

            connection.Open();

            int count = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            return count;
        }
    }
}
