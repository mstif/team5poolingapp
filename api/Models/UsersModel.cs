
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Contracts;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class UsersModel 
    {
        [DisplayName("Логин")]
        public string UserName { get; set; } = null!;
        [DisplayName("Email")]
        public string Email{ get; set; }
        [DisplayName("Полное имя")]
        public string? FullName {  get; set; }
        [DisplayName("Идентификатор")]
        public string? Id { get; set; }
        [DisplayName("Роли")]
        public List<IsInRole> SecondRoles { get; set; }

        [DataType(DataType.Password)]
        public string? Password {  get; set; }
        [DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "пароль и подтверждение должны совпадать")]
        public string? ConfirmPassword { get; set; }

        public List<EntityModel?>? AllCompanies { get; set; }

        public string? Company{ get; set; }
        public long? CompanyId { get; set; }


        public bool? Approved { get; set; }


        public string? MajorRole {  get; set; }

        public string? StatusMessage { get; set; }

        public UsersModel()
        {
           
        }
        
        //public static StockModel? GetFromDTO(UserDTO? dto)
        //{
        //    if (dto == null) return null;
        //    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<StockDTO, StockModel>()).CreateMapper();
        //    return mapper.Map<StockDTO, StockModel>(dto);
        //}

        //public StockDTO ToDTO()
        //{

        //    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<StockModel, StockDTO>()).CreateMapper();
        //    return mapper.Map<StockModel, StockDTO>(this!);
        //}

        //public static List<StockModel>? GetListFromDTO(List<StockDTO> dto)
        //{
        //    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<StockDTO, StockModel>()).CreateMapper();
        //    return mapper.Map<List<StockDTO>, List<StockModel>>(dto);
        //}

    }

    public class IsInRole
    {
        public string Role { get; set; }
        public bool InRole { get; set; }
    }
}
