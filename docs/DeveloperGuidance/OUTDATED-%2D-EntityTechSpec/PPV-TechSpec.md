This page is outdated and will be deleted soon. Please navigate to sharepoint site for most current version - https://<Client>365.sharepoint.com/sites/GTR/SitePages/EntityTechSpec.aspx 
## PPV (Private Passenger Vehicle)

Entity.Id = VehicleId or GUID you’ve created for this Vehicle

Entity.EntityType is PPV

The characteristic names that are available on operator are defined below. Please note that these will be renamed and moved.

```markdown
| Characteristic Name     | Element Type | Description                                                                                                                                                         |
| ----------------------- | ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| VIN                     | string       | 17 digits alphanumeric                                                                                                                                              |
| retentionId             | String       | RetentionId of the insurance application if available required for Sales applications optional for Service applications                                             |
| vehicleId               | Guid         | Vehicle Id of the Vehicle for which the location is associated with required for all applications                                                                   |
| locationType            | String       | Type of the address location ValueList(Partial): PLG for GaragedAddress                                                                                             |
| productId               | String       | ProductVersion used to identify the business rules• Front-end systems do not need to send this value PremiumCalc domain to identify the product version and send it |
| addressLine1            | String       | Address Line of the location                                                                                                                                        |
| unitType                | String       | Unit Type                                                                                                                                                           |
| unitNumber              | string       | Unit Number                                                                                                                                                         |
| city                    | String       | Address City                                                                                                                                                        |
| stateCode               | String       | Address State, 50 States + DC                                                                                                                                       |
| zipCode5                | String       | 5-digit zipcode of the location                                                                                                                                     |
| zipCode4                | String       | 4-digit extension of the zipcode                                                                                                                                    |
| cassCode                | String       | Cass Code, Single character to identify whether the address is customer reported or not ValueList(Partial): C, R, D, L, N                                           |
| matchCode               | String       | Spectrum match code (4digits)                                                                                                                                       |
| matchCodeIsValidAddress | Boolean      | MatchCode is Valid Address if applicable                                                                                                                            |
| recordType              | String       | RecordType from Spectrum                                                                                                                                            |
```