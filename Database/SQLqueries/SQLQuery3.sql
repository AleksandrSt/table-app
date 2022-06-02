INSERT INTO price_hubs (Name)
SELECT DISTINCT PriceHub
FROM ice_electric2021final
ORDER BY PriceHub ASC;