using AutoMapper;
using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects.Account;
using SmartWaste.Model.DataTransferObjects.WebConfiguration;
using SmartWaste.Model.JsonModels.Account;

namespace SmartWaste.Account.AppSettings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Status, StatusDto>().ReverseMap();
        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<Language, LanguageDto>().ReverseMap();
        CreateMap<Country, CountryDto>().ReverseMap();
        CreateMap<User, ShortUserDto>();
        CreateMap<User, UpdateUserInput>().ReverseMap();
    }
}