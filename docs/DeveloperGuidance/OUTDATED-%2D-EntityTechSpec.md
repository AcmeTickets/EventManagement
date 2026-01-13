This page is outdated and will be deleted soon. Please navigate to sharepoint site for most current version -
https://<Client>365.sharepoint.com/sites/GTR/SitePages/EntityTechSpec.aspx

# Add/Update/Remove Entity Technical Specifications

## Purpose

For anyone willing to communicate with the GTR systems to obtain a rate, we have recently changed our structure platform to be Entity Based. In short, an Entity is simply any generic <thing> that houses or contains Characteristics. Characteristics are attributes of an entity that describe. This document is explaining how to Add/Update/Remove entities for the GTR systems.

Previous versions of documents exist like this that describe entities. They will be revised to fit this new feature, but this overall document will serve as a main reference on how to add/update/remove entities from an application including starting an application. A short section at the end describes what you can do to include mappings for Characteristics.

# ASB Messaging Information

############################################################################

# Queue

All information should be sent to the Queue with the name below under the above environment.

- **<Client>.tieringrating.applicationprocessor**

############################################################################

# Endpoint

Below table provides the environment details.
Only DV1 is available right now (when the document is prepared). More environments will be added as provisioned.

```markdown
| Environment | Endpoint Url                                              |
| ----------- | --------------------------------------------------------- |
| DV1         | **https://gze-dombus-dv1-bns-001.servicebus.windows.net** |
```

\*\*\* Reach out to PACE System’s team (Rodriguez Rizo, Pedro PRodriguezRizo@<Client>.com) for credentials information.

############################################################################

# Headers

All messages need to contain a variation of this header to be accepted correctly by NServiceBus.

````markdown
```json
{
  "NServiceBus.MessageId": "accedda4-522d-47c1-b3d7-ae59012e84e5", // Any GUID will do
  "NServiceBus.ContentType": "application/json",
  "NServiceBus.EnclosedMessageTypes": "SEE TABLE BELOW",
  "NServiceBus.TimeSent": "2022-03-15 18:21:26:331441 Z" // Timestamp of when you send it.
}
```
````
# TimeSent

Timesent must be in a specific format

Similar to the timestamp function  .ToString("yyyy-MM-dd HH:mm:ss:ffffff 'Z'") **This must match this exactly similar to above**

# Coded Header MessageType

See the coded message contract names before any namespace or business stage changes.

```markdown
| Business Stage  | Command            | Command Type                                                                                              |
| --------------- | ------------------ | --------------------------------------------------------------------------------------------------------- |
| New Business    | Start              | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.StartApplication" |
|                 | Add                | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.AddEntity"        |
|                 | Update             | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.UpdateEntity"     |
|                 | Remove             | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.RemoveEntity"     |
| --------------- | ------------------ | --------------------------------------------------------------------------------------------------------- |
```

# NOT FINALIZED


The ‘EnclosedMessageTypes’ Parameter from the headers is highlighted below based on business stage:

# Proposed Header MessageType

```markdown
| Business Stage  | Command            | Command Type                                                                                              |
| --------------- | ------------------ | --------------------------------------------------------------------------------------------------------- |
| New Business    | Start              | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.StartNewBusinessApplication" |
|                 | Add                | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.AddNewBusinessEntity"        |
|                 | Update             | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.UpdateNewBusinessEntity"     |
|                 | Remove             | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.RemoveNewBusinessEntity"     |
| --------------- | ------------------ | --------------------------------------------------------------------------------------------------------- |
| Endorsement     | Start              | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.StartEndorsement"            |
|                 | Add                | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.AddEndorsementEntity"        |
|                 | Update             | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.UpdateEndorsementEntity"     |
|                 | Remove             | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.RemoveEndorsementEntity"     |
| --------------- | ------------------ | --------------------------------------------------------------------------------------------------------- |
| Renewal         | Start              | "<Client>.TieringRating.ApplicationProcessor.MessageContracts.Internal.Commands.StartRenewal"                |
| --------------- | ------------------ | --------------------------------------------------------------------------------------------------------- |
```
*Further decisions on Renewal commands will come later. 

# END NOT FINALIZED

#

############################################################################

# Commands

## Command – StartApplication

This is how you tell the GTR system how to start a new application. Should always be sent FIRST prior to additional commands to add/update/remove entities.

### Elements

```markdown
| Element Name  | Element Type | Required? | Description                                       |
| ------------- | ------------ | --------- | ------------------------------------------------- |
| ApplicationId | Guid         | Required  | ApplicationId you want to start, for your record. |
| DateStarted   | DateTime     | Required  | Date the application started.                     |
```

### Example Contract

````markdown
```json
{
  "ApplicationId": "677f8ee8-9b53-429e-9d1d-7047a4044147",
  "DateStarted": "2022-07-27T00:00:00-04:00"
}
```
````

## Command – AddEntity and UpdateEntity.

These are the first main commands. Following will be some documentation on how to utilize them when sending messages in ASB. The only difference is how you create the contract. AddEntity is designated in the message as AddEntity, UpdateEntity is designed similarly. UpdateEntity is designed to take all the characteristics and update an entity based on the characteristics provided or overriding ones you removed. Being careful to not update characteristics with null values unless its intentional.

### Elements

```markdown
| Element Name  | Element Type      | Required? | Description                                                  |
| ------------- | ----------------- | --------- | ------------------------------------------------------------ |
| ApplicationId | Guid              | Required  | ApplicationId sent when ‘StartApplication’ message was sent. |
| Entity        | Object(See below) | Required  | Entity you want to add OR update.                            |
```

### Example Contract

````markdown
```json
{
  "ApplicationId": "677f8ee8-9b53-429e-9d1d-7047a4044147",
  "Entity": {
    "Id": "49473c29-6f53-4993-a7b0-60fe57766502",
    "EntityType": "Policy",
    "Ordinal": 1,
    "Characteristics": {
      "ResidenceType": {
        "StringValue": "Own"
      }
    },
    "RelatedEntities": []
  }
}
```
````

## Command – RemoveEntity

Simply put, removes an entity from an application. Again, please see the section below for ASB information on utilizing these commands.

### Elements

```markdown
| Element Name  | Element Type | Required? | Description                                                  |
| ------------- | ------------ | --------- | ------------------------------------------------------------ |
| ApplicationId | Guid         | Required  | ApplicationId sent when ‘StartApplication’ message was sent. |
| EntityId      | String       | Required  | EntityId you want to remove.                                 |
```

### Example Contract

````markdown
```json
{
  "ApplicationId": "677f8ee8-9b53-429e-9d1d-7047a4044147",
  "EntityId": "49473c29-6f53-4993-a7b0-60fe57766502"
}
```
````

############################################################################

# Contract – Entity

Entity is an agnostic type of data we would like to add to an application or similar structure in GTR. There are currently 3 Entity Types defined in GTR: Operator, PPV (Private Passenger Vehicle), and Policy.

### Elements

```markdown
| Element Name    | Element Type                 | Required? | Description                                                                                                                                                                                                 |
| --------------- | ---------------------------- | --------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| EntityType      | String                       | Required  | The type of the entity. Should be from this list, not inclusive of this document or existing documents that might provide a different list or new entity types: Operator, Policy, PPV (Vehicle)            |
| Id              | String(guid)                 | Required  | Id of the entity. Traditionally this is a DriverId, VehicleId, AddressId… etc                                                                                                                               |
| Ordinal         | Int                          | Optional  | The ordinal associated with the entity, optional                                                                                                                                                            |
| Characteristics | Dictionary<string, ValueBox> | Optional  | Dictionary Key is the NAME of the Characteristic. Dictionary Value is a ValueBox(description below) ValueBox is the value of the characteristic, wrapped. Can be null, resulting in empty characteristics.  |
| RelatedEntities | List<RelatedEntity>          | Optional  | List of related entities. Described in this document.                                                                                                                                                       |
```

## Contract – ValueBox

ValueBox is an wrapper used by the GTR system to wrap the value of a Chracteristic into a 4 type field. Simply should have only one of 4 values. String,Bool,DateTime,Decimal

### Elements

```markdown
| Element Name  | Element Type | Required? | Description                |
| ------------- | ------------ | --------- | -------------------------- |
| StringValue   | String       | Optional  | Value for a string field   |
| DecimalValue  | Decimal      | Optional  | Value for a decimal field  |
| DateTImeValue | DateTime     | Optional  | Value for a DateTime field |
| BoolValue     | Bool         | Optional  | Value for a bool field     |
```

### Example Contract

## Contract – RelatedEntity

RelatedEntity is to show a relationship between types of entities. 

### Elements

```markdown
| Element Name | Element Type | Required? | Description                                              |
| ------------ | ------------ | --------- | -------------------------------------------------------- |
| Id           | String(Guid) | Required  | Id of the related entity                                 |
| EntityType   | String       | Required  | Entity type: Operator, Policy, PPV                       | 
| Relationship | String       | Required  | The relationship for the entity.                         |
```

# Existing Operator/Vehicle/GeoLocation contracts.

Existing contracts for these Domains will be updated appropriately. In the interim, the Characteristics associated with the contracts can be safely mapped to Characteristics on an entity, but keep in mind the information may be moved around when the information is published.

This example here is brief to just highlight the across the board changes between all commands that have changed.

````markdown
```json
{
  "ApplicationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", // Any ApplicationId gets mapped here.
  "Entity": {
    "Id": "21ft7f64-2211-4562-v3gc-tt243f66a54hn", // Existing OperatorId, VehicleId, AddressId
    "EntityType": "PPV", // If the entity is a Vehicle -> PPV, Operator, Policy, Etc
    "Ordinal": 1,
    "Characteristics": {
      "VIN": {
        // Explained below, per characteristic on previous document the fields have to be remapped See "Characteristic Name" Column on the documents below.
        "StringValue": "3N1CN7AP8CL848849"
      },
      "ProductId": {
        "StringValue": "4fa85f64-5817-4562-b3fc-2c965j66afa6"
      }
    }
  }
}
```
````

## Operator

Entity.Id = OperatorId or GUID you’ve created for this operator

Entity.EntityType = Operator

The characteristic names that are available on operator are defined below. Please note that these will be renamed and moved.

```markdown
| Characteristic Name                  | Element Type |  Description                                                                                                                                                                                                                                                                                                                                                                                            |
| ------------------------------------ | ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| DriversLicenseNumber                 | string       | Operator’s License Number                                                                                                                                                                                                                                                                                                                                                                               |
| DriversLicenseStateAbbreviation      | String       | EX ‘NC’ or ‘OH’                                                                                                                                                                                                                                                                                                                                                                                         |
| DriversLicenseStatusCode             | string       | Code identifying the status of the Operator's driver's license as rated. Note: Client data may have changed between the last time the Quote was rated and the current date. Value List (Complete): ACTV = Active (default), EXPD = Expired, INAC = Inactive, INVD = Invalid, LPMT = Learner's Permit, NLUS = Active Not Lic In US, REVK = Revoked, SUSP = Suspended, UNKN = Unknown, UNLC = Unlicensed. |
| PriorDriversLicenseNumber            | string       | Operator’s Previous License Number                                                                                                                                                                                                                                                                                                                                                                      |
| PriorDriversLicenseStateAbbreviation | string       | EX ‘NC’ or ‘OH’                                                                                                                                                                                                                                                                                                                                                                                         |
| DateOfBirth                          | Date         | Operator Date of Birth                                                                                                                                                                                                                                                                                                                                                                                  |
| MaritalStatus                        | String       | Value List (partial): Divorced, Separated, Married, Refused, Single, Unknown, Widowed                                                                                                                                                                                                                                                                                                                   |
| DoesChildResideWithDriver            | Bool         | Value List (complete): ‘true’, there are resident children under 18. ‘false’, there are no resident children under 18 (default)                                                                                                                                                                                                                                                                         |
| DoesDriverResideWithInsured          | Bool         | Value List (complete): ‘true’, driver resides with the named insured. ‘false’, driver does not reside with the named insured                                                                                                                                                                                                                                                                            |
| DriverRelationshipToInsuredCode      | string       | Value List (partial): DRD = Daughter, DRF = Father, DRFI = Fiance, DRIN = Named Insured, DRSP = Spouse. For additional values refer to the support table DRV_RLT_TO_INS_LIT                                                                                                                                                                                                                             |
| HasAutoLicense                       | Bool         | The value associated with identifying if the driver has an auto license ‘true’/’false                                                                                                                                                                                                                                                                                                                   |
| JobTitle                             | string       | Text entered by customer in occupation field                                                                                                                                                                                                                                                                                                                                                            |
| OccupationCode                       | string       | Code identifying the driver's occupation as last rated. Note: Client data may have changed between the last time the Quote was rated and the current date. Value List (partial): 0010 = Self-Employed NOC, 00A9 = Sales/Marketing, 0020 = Farmers, 0024 = Executive secretary (Degreed), 0086 = Bank tellers. For additional values refer to the support table                                          |
| Gender                               | String       | Value List (complete): Male, Female, Non-binary, Unknown                                                                                                                                                                                                                                                                                                                                                |
| DriverStatus                         | String       | Value List (complete): Active, At School, Cancelled, Deceased, Deployed, Excluded (Involuntary), Excluded (Voluntary), Non-Driver, Other Insurance                                                                                                                                                                                                                                                      |
| OccurrenceDate                       | Date         | Date of when an accident or conviction occurred                                                                                                                                                                                                                                                                                                                                                         |
| OccurrenceType                       | String       | Value List (partial): Accident, Minor, Speed                                                                                                                                                                                                                                                                                                                                                            |
| TermEffectiveDate                    | Date         | Date of a new policy term                                                                                                                                                                                                                                                                                                                                                                               |
| DriverAddDate                        | Date         | Date of when an operator was initially added to a policy                                                                                                                                                                                                                                                                                                                                                |
| ApplyPoints                          | string       | Apply Points Indicator Either Y, N, or null                                                                                                                                                                                                                                                                                                                                                             |
```

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

## Sample Contract

Extremely simple formatting of the information above in an entity format. Included is 3 entities listed here. These are purely to demonstrate the json format required to send the information.

````markdown
```json
{
      "id": "b95d17c7-e705-4722-9ae0-0986fbe4d153",
      "EntityType": "PPV",
      "Ordinal": 1,
      "Characteristics": {
          "VIN": {
              "StringValue": "1HTMSAZR66H200901"
          },
          "RetentionId": {
              "StringValue": "d92b6ffd-9d5c-41b7-9ac3-2f4e902d7838"
          },
          "VehicleId": {
              "StringValue": "d92b6ffd-9d5c-41b7-9ac3-2f4e902d7838"
          },
          "LocationType": {
              "StringValue": "Home"
          },
          "ProductId": {
              "StringValue": "Pure4.0"
          },
          "UnitType": {
              "StringValue": "Home"
          },
          "UnitNumber": {
              "StringValue": "126"
          },
          "City": {
              "StringValue": "Town"
          },
          "StateCode": {
              "StringValue": "MD"
          },
          "ZipCode5": {
              "StringValue": "12345"
          },
          "ZipCode4": {
              "StringValue": "0012"
          },
          "CassCode": {
              "StringValue": "C"
          },
          "MatchCode": {
              "StringValue": "9999"
          },
          "MatchCodeIsValidAddress": {
              "BoolValue": "true"
          },
          "RecordType": {
              "StringValue": "M",
          }
        },
      "RelatedEntities": []
  },
  {
      "id": "1748514d-f6cd-47dc-aca7-205161ac8d7d",
      "EntityType": "Operator",
      "Ordinal": 1,
      "Characteristics": {
          "DriversLicenseNumber": {
              "StringValue": "1234567890"
          },
          "DriversLicenseStateAbbreviation": {
              "StringValue": "OH"
          },
          "DriversLicenseStatusCode": {
              "StringValue": "ACTV"
          },
          "PriorDriversLicenseNumber": {
              "StringValue": "1234567890"
          },
          "PriorDriversLicenseStateAbbreviation": {
              "StringValue": "OH"
          },
          "DateOfBirth": {
              "DateTimeValue": "2022-03-15T18:12:54.011Z"
          },
          "MaritalStatus": {
              "StringValue": "Married"
          },
          "DoesChildResideWithDriver": {
              "BoolValue": "true"
          },
          "DoesDriverResideWithInsured": {
              "BoolValue": "true"
          },
          "DriverRelationshipToInsuredCode": {
              "StringValue": "DRSP"
          },
          "HasAutoLicense": {
              "BoolValue": "true"
          },
          "JobTitle": {
              "StringValue": "Chef"
          },
          "OccupationCode": {
              "StringValue": "0010"
          },
          "Gender": {
              "StringValue": "Male"
          },
          "DriverStatus": {
              "StringValue": "Active"
          },
          "OccurrenceDate": {
              "DateTimeValue": "2022-03-15T18:12:54.011Z"
          },
          "OccurrenceType": {
              "StringValue": "Minor"
          },
          "TermEffectiveDate": {
              "DateTimeValue": "2022-03-15T18:12:54.011Z"
          },
          "DriverAddDate": {
              "DateTimeValue": "2022-03-15T18:12:54.011Z"
          },
          "ResidenceType": {
              "StringValue": "Own"
          }
      },
      "RelatedEntities": []
  }
```
````
