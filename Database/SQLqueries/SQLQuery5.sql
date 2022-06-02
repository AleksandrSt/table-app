UPDATE ice_electric2021final
SET ice_electric2021final.PriceHubId = price_hubs.Id
from ice_electric2021final
INNER JOIN price_hubs ON ice_electric2021final.PriceHub = price_hubs.Name;