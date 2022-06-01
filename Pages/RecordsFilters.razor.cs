using Microsoft.AspNetCore.Components;
using TableApp.Data;
using TableApp.Models;
using static System.Int32;

namespace TableApp.Pages
{
    public partial class RecordsFilters : ComponentBase
    {
        [Inject]
        private PriceHubsService Service { get; set; } = null!;

        [Parameter]
        public EventCallback<int> OnPriceHubSetCallback { get; set; }

        [Parameter]
        public EventCallback<DateFilter> OnDateRangeSetCallback { get; set; }

        private List<PriceHub>? _priceHubs;



        [Parameter]
        public int CurrentPriceHubId { get; set; }

        [Parameter]
        public string? DateType { get; set; }

        [Parameter]
        public DateTime? From { get; set; }

        [Parameter]
        public DateTime? To { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _priceHubs = await Service.GetPriceHubs();
        }

        private async Task ChoosePriceHubAsync(ChangeEventArgs e)
        {
            string id = e.Value?.ToString() ?? "0";

            await OnPriceHubSetCallback.InvokeAsync(Parse(id));
        }

        private void ToggleDateType(string type)
        {
            DateType = DateType == type ? null : type;
        }

        private async Task SetFromDateAsync(ChangeEventArgs e)
        {
            From = DateParse(e);
            await SendDateRangeCallbackAsync();
        }

        private async Task SetToDateAsync(ChangeEventArgs e)
        {
            To = DateParse(e);
            await SendDateRangeCallbackAsync();
        }

        private DateTime? DateParse(ChangeEventArgs e)
        {
            string dateString = e.Value!.ToString()!;
            return dateString == "" ? null : DateTime.Parse(dateString);
        }

        private async Task SendDateRangeCallbackAsync()
        {
            if (DateType == null) return;
            await OnDateRangeSetCallback.InvokeAsync(new DateFilter
            {
                DateType = DateType!,
                From = From,
                To = To
            });
        }
    }
}
