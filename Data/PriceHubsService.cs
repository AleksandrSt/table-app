using System.Data.SqlClient;
using TableApp.Models;

namespace TableApp.Data
{
    public class PriceHubsService
    {
        private readonly IConfiguration _configuration;
        public PriceHubsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<PriceHub>> GetPriceHubs()
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");
            List<PriceHub> priceHubs = new List<PriceHub>();

            await using SqlConnection connection = new SqlConnection(conStr);

            await using SqlCommand command = new SqlCommand("SELECT * FROM price_hubs");

            command.Connection = connection;

            connection.Open();

            await using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    priceHubs.Add(new PriceHub
                    {
                        Id = Convert.ToInt32(reader[nameof(PriceHub.Id)]),
                        Name = reader[nameof(PriceHub.Name)].ToString()!,
                    });
                }
            }

            connection.Close();

            return priceHubs;
        }
    }
}
