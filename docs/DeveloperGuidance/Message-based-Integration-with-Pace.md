# Front End Integration with PACE

[[_TOC_]]
## Overview
The Sales and Service front ends collects data, augments it, stores it, and then uses it for placement and rating. This document will discuss all the commands that are available, how to end them, and how to integrate with PACE



# Commands

## Set Own or Rent primary residence 

<Client> wishes to add a policy level characteristic for residence. The first step of this process is for the front ends to send raw data to the Operator domain for ResidenceType. Premium Calculation can utilize that information to calculate any policy-level characteristic.

ResidenceType is identified by translating the values from the FE in the following manner:

| Description | Value  |
|--|--|
| Owned | O |
| Rented | T |
| Unknown | R |


### Overvew

Policy Domain defines the following messages for handling frontend requests for various operations. The Domain expects UNCHANGED DATA THAT THE CUSTOMER ENTERED.

- Policy Domain receives a message to Validate, Save and Generate a refined ResidenceType Characteristic.
  - **UpdateOwnRentResidence**

### Queue

The **UpdateOwnRentResidence** message is sent to the queue: **<Client>.PremiumCalculation.Endpoint**

#### Headers
```json
{
  "NServiceBus.MessageId": "accedda4-522d-47c1-b3d7-ae59012e84e5", <-- Any GUID will do
  "NServiceBus.ContentType": "application\/json",  <-- Exactly like it is here
  "NServiceBus.EnclosedMessageTypes": "<Client>.PremiumCalcuation.Contracts.Commands.UpdateOwnRentResidene", <-- Exactly like it is here
  "NServiceBus.TimeSent": "2022-03-15 18:21:26:331441 Z" <-- Timestamp of when you send it.
}
```


#### Message Elements

| **applicationId** | Guid | Required | Application Id Created when you called Start Application | 
| --- | --- | --- | --- | 
| **policyId** | String | Optional / Required | PolicyId of the insurance application if available. Required for Service applications. Optional for Sales applications
 |
| **retentionId** | String | Optional / Required | RetentionId of the insurance application if available. Required for Service applications. Optional for Sales applications|
| **productId** | String | Optional | ProductVersion used to identify the business rules. Front-end systems do not need to send this value PremiumCalc domain to identify the product version and send it |
| **ResidenceType** | String | Required | Type of ownership for residence Value List – Own,Rent,Unknown

#### Sample Request Body

```json
{
    "ApplicationId": "7f0f8db8-fcbe-481b-a547-af1797566315",
    "PolicyId": "2000000pid",
    "RetentionId": "string",
    "ProductId": "001",
    “ResidenceType”: "string",
}
```

#### Response Info

This is a one way send. There is no response.

# Azure Service Bus Information

Below table provides the environment details.

Only DV1 is available right now (when the document is prepared). More environments will be added as provisioned.

| **DV1** | [https://gze-dombus-dv1-bns-001.servicebus.windows.net/premiumcalculation](https://gze-dombus-dv1-bns-001.servicebus.windows.net/premiumcalculation)Connection String: Endpoint=sb://gze-dombus-dv1-bns-001.servicebus.windows.net/;SharedAccessKeyName=;SharedAccessKey=;TransportType=Amqp |
| --- | --- |


Reach out to PACE System&#39;s team (Rodriguez Rizo, Pedro [PRodriguezRizo@<Client>.com](mailto:PRodriguezRizo@<Client>.com)) for credentials information.

## Security

Reach out to Livry, Pierre [PLivry@<Client>.com](mailto:PLivry@<Client>.com) or Rodriguez Rizo, Pedro [PRodriguezRizo@<Client>.com](mailto:PRodriguezRizo@<Client>.com) for those details

## Service Versioning
Not Available




### Error Handling

- It&#39;s the responsibility of the consumers to not fail when publishing the messages.

### Validations (Internal TO GTR)

Below table contains all the Business Validations and corresponding error code and message if any field fails validation

When a message fails validation, an error message is published to Error Queue with the failed validations.
| Element  | Failure Message |
| --- | --- |
| **applicationId** |  **ApplicationId** should be a valid Guid, not Empty <br> **Status Code**: 400, Messages: Missing/Incomplete data
| **policyId** | PolicyId should not be empty when RetentionId is empty <br> StatusCode: 400, Messages: Missing/Incomplete data|
| **retentionId** | RetentionId should not be empty when PolicyId is empty<BR> StatusCode: 400, Messages: Missing/Incomplete data|
| **productId** | Should not be empty <BR> StatusCode: 400, Messages: Missing/Incomplete data|
| **ResidenceType** | Should not be empty <BR> Value must be Own, Rent, or Unknown <BR> StatusCode: 400, Messages: Missing/Incomplete data|

### Code for Timestamp
```csharp
     public static string ToWireFormattedString(DateTime dateTime)
        {
            return dateTime.ToUniversalTime()
                .ToString(Format, CultureInfo.InvariantCulture);
        }
```