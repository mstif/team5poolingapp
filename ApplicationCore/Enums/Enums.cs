using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApplicationCore.Enums
{

    public enum ObjectTypes
    {
        Contragent = 1,
        Stock = 2,
        Order = 3,
        Invoice = 4,
        CargoType = 5,
        Temperature = 6,
        DeliveryContract=7,

    }
   // [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Countries
    {
       // [Display(Name = "Российская Федерация")]
        Russia = 1,
       // [Display(Name = "Армения")]
        Armenia = 2,
      //  [Display(Name = "Белоруссия")]
        Belorussia = 3,
      //  [Display(Name = "Китай")]
        China = 4,

    }
    public enum TypeUser
    {
        Administrator = 1,
        Logistic = 2,
        Seller = 3,
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ContragentType
    {
        DeliveryPoint = 1,
        LogisticCompany = 2,
        ClientCompany = 3,
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InvoiceStatus
    {
        [Display(Name = "Новая")]
        New =1,
        [Display(Name = "Отгружена")]
        Shipped = 2,
        [Display(Name = "Доставлена")]
        Delivered = 3,
        [Display(Name = "Сдана")]
        Passed = 4
           
    }


}
