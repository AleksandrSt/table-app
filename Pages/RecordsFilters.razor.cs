using Microsoft.AspNetCore.Components;
using TableApp.Data;
using TableApp.Models;
using static System.Int32;

namespace TableApp.Pages
{
    public partial class RecordsFilters: ComponentBase
    {
        [Inject] 
        private PriceHubsService Service { get; set; } = null!;

        [Parameter]
        public EventCallback<int> OnPriceHubSetCallback { get; set; }

        private List<PriceHub>? _priceHubs;

        private int _priceHubId = 0;

        protected override async Task OnInitializedAsync()
        {
            _priceHubs = await Service.GetPriceHubs();
        }

        private async Task ChoosePriceHub(ChangeEventArgs e) {
            string id = e.Value?.ToString() ?? "0";

            await OnPriceHubSetCallback.InvokeAsync(Parse(id));
        }
    }
}
