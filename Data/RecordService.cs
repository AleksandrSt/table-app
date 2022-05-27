using System.Data.SqlClient;
using TableApp.Models;

namespace TableApp.Data
{
    public class RecordService
    {
        private readonly IConfiguration _configuration;
        private string? _query;
        public RecordService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<Record>> GetRecords()
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");
            List<Record> records = new List<Record>();

            await using SqlConnection connection = new SqlConnection(conStr);

            await using SqlCommand command = new SqlCommand(_query);

            command.Connection = connection;

            connection.Open();

            await using (SqlDataReader reader = command.ExecuteReader())
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

            _query = string.Empty;

            connection.Close();

            return records;
        }

        public void GetAllRecords(string orderBy, string sort)
        {
            _query = "SELECT " +
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
                     "INNER JOIN price_hubs ON ice_electric2021final.PriceHubId = price_hubs.Id " +
                     $"ORDER BY {orderBy} {sort} ";
        }

        public void PaginateQuery(int pageNumber, int itemsOnPage)
        {
            _query += $"OFFSET {(pageNumber-1) * itemsOnPage} ROWS FETCH NEXT {itemsOnPage} ROWS ONLY";
        }

        public Task<int> Count()
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");
            int count;
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM ice_electric2021final"))
                {
                    command.Connection = connection;
                    connection.Open();
                    count = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }

            return Task.FromResult(count);
        }
    }
}
