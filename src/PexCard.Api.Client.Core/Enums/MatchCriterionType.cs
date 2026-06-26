using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    /// <summary>
    /// Type of an extracted/matched field produced by attachment analysis.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MatchCriterionType
    {
        MerchantName,
        Total,
        Date,
        OrderDate,
        Last4Digits,
        Subtotal,
        AmountPaid,
        Gratuity,
        InvoiceReceiptId,
        AccountId,
        Pump,
        Gallons,
        PricePerGallon,
        FuelProduct,
        TotalTax,
        Time,
        OrderTime,
        Currency,
        CurrencySymbol,
        Mcc,
        ChargeDate,
        ChargeTime,
        ServiceProvider,
        DueDate,
        PaymentTerms,
        RemitToName,
        RemitToAddress,
        RemitToEmail,
        BankDetails,
        RemitToWebsite,
        RemitToTaxId,
        RemitToAddress1,
        RemitToAddress2,
        RemitToCity,
        RemitToState,
        RemitToPostalCode,
        RemitToContactFirstName,
        RemitToContactLastName,
        RemitToContactPhoneNumber,
        RemitToContactEmail,
        RemitToBankName,
        RemitToBankAccountNumber,
        RemitToBankRoutingNumber,
        IsFuel,
        IsForeignCurrency,
        IsAlcohol,
        IsReceipt,
        IsInvoice,
        IsHotel,
        IsAirTrainTravel,
        IsVehicleRental,
        IsRestaurant,
        IsRideShare,
        Unknown
    }
}
