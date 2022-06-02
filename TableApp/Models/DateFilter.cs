namespace TableApp.Models
{
    public class DateFilter
    {
        public string DateType { get; set; } = null!;
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
