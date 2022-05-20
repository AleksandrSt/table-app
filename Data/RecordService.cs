using System.Data.SqlClient;
using TableApp.Models;

namespace TableApp.Data
{
    public class RecordService
    {
        private readonly IConfiguration _configuration;
        private string _query;
        public RecordService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Record> GetRecords()
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");
            List<Record> records = new List<Record>();
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand(_query))
                {
                    command.Connection = connection;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
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
                }
            }

            return records;
        }

        public void GetAllRecords()
        {
            _query = "SELECT * FROM ice_electric2021final ORDER BY Id ";
        }

        public void PaginateQuery(int pageNumber, int itemsOnPage)
        {
            _query += $"OFFSET {(pageNumber-1) * itemsOnPage} ROWS FETCH NEXT {itemsOnPage} ROWS ONLY";
        }
    }
}
