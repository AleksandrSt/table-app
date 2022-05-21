using System.ComponentModel.DataAnnotations;

namespace TableApp.Models
{
    public class Record
    {
        [Key]
        public int Id { get; set; }
        public string PriceHub { get; set; }
        public DateTime TradeDate { get; set; }
        public DateTime DeliveryStartDate { get; set; }
        public DateTime DeliveryEndDate { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double WtdAvgPrice { get; set; }
        public double Change { get; set; }
        public int DailyVolume { get; set; }
    }
}
