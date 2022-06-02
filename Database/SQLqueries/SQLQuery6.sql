ALTER TABLE ice_electric2021final
ADD CONSTRAINT FK_PriceHubIdName FOREIGN KEY (PriceHubId)
REFERENCES price_hubs(Id);