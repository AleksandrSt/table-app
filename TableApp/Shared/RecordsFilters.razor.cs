using Microsoft.AspNetCore.Components;
using TableApp.DataInterfaces;
using TableApp.Models;
using static System.Int32;

namespace TableApp.Shared
{
    public partial class RecordsFilters : ComponentBase
    {
        [Inject]
        private IPriceHubsService Service { get; set; } = null!;

        [Parameter]
        public EventCallback<int> OnPriceHubSetCallback { get; set; }

        [Parameter]
        public EventCallback<DateFilter> OnDateRangeSetCallback { get; set; }

        private IEnumerable<PriceHub>? _priceHubs;


        [Parameter]
        public int CurrentPriceHubId { get; set; }

        [Parameter]
        public string? DateType { get; set; }

        [Parameter]
        public DateTime? From { get; set; }

        [Parameter]
        public DateTime? To { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _priceHubs = await Service.GetPriceHubsAsync();
        }

        private async Task ChoosePriceHubAsync(ChangeEventArgs e)
        {
            string id = e.Value?.ToString() ?? "0";

            CurrentPriceHubId = Parse(id);

            await OnPriceHubSetCallback.InvokeAsync(CurrentPriceHubId);
        }

        private async Task ToggleDateTypeAsync(string type)
        {
            DateType = DateType == type ? null : type;
            if (DateType == null)
            {
                From = null;
                To = null;
                await SendDateRangeCallbackAsync();
            }
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
            await OnDateRangeSetCallback.InvokeAsync(new DateFilter
            {
                DateType = DateType!,
                From = From,
                To = To
            });
        }
    }
}
