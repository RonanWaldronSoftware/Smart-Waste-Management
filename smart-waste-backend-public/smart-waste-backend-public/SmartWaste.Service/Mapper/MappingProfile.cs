using AutoMapper;
using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects;
using SmartWaste.Model.DataTransferObjects.Account;
using SmartWaste.Model.DataTransferObjects.Damage;
using SmartWaste.Model.DataTransferObjects.Route;
using SmartWaste.Model.DataTransferObjects.Unit;
using SmartWaste.Model.DataTransferObjects.WebConfiguration;
using SmartWaste.Model.Extensions;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Account;
using SmartWaste.Model.JsonModels.Damage;
using SmartWaste.Model.JsonModels.Route;
using SmartWaste.Model.JsonModels.Unit;

namespace SmartWaste.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AlertTypeCategory, AlertTypeCategoryDto>().ReverseMap();
            CreateMap<AlertType, AlertTypeDto>().ReverseMap();
            CreateMap<ContentType, ContentTypeDto>().ReverseMap();
            CreateMap<DamageType, DamageTypeDto>().ReverseMap();
            CreateMap<DeviceType, DeviceTypeDto>().ReverseMap();
            CreateMap<DeviceInputType, DeviceInputTypeDto>().ReverseMap();
            CreateMap<MessageType, MessageTypeDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<StatusCategory, StatusCategoryDto>().ReverseMap();
            CreateMap<Status, StatusDto>().ReverseMap();
            CreateMap<UnitModelType, UnitModelTypeDto>().ReverseMap();
            CreateMap<UnitModelType, ShortUnitModelTypeDto>().ReverseMap();
            CreateMap<UnitType, UnitTypeDto>().ReverseMap();
            CreateMap<User, CreateUserInput>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, BaseUserDto>().ReverseMap();
            CreateMap<User, ShortUserDto>().ReverseMap();
            CreateMap<User, AlertUserDto>()
                .ForMember(dest => dest.Alerts, opt => opt.MapFrom(src => src.UserAlertUsers.Select(uau => uau.AlertType)));
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Language, LanguageDto>().ReverseMap();
            CreateMap<Route, RouteDetailsDto>().ReverseMap();
            CreateMap<Route, RouteDto>()
                 .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.User.Name))
                 .ForMember(dest => dest.UnitCount, opt => opt.MapFrom(src => src.RouteUnits.Count)).ReverseMap();
            CreateMap<RouteUnit, RouteUnitDetailsDto>()
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Unit.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Unit.Longitude));

            CreateMap<Unit, ShortUnitDto>().ReverseMap();
            CreateMap<Unit, UpdateUnitInput>().ReverseMap();
            CreateMap<Unit, CreateUnitInput>().ReverseMap();
            CreateMap<UnitReading, UnitReadingDto>()
                .ForMember(dest => dest.BinLevelPercentage, opt => opt.MapFrom(src => src.Unit.ComputeUnitFillLevelPercentages(src).FillLevelPercentage))
                .ForMember(dest => dest.BinLevelBcPercentage, opt => opt.MapFrom(src => src.Unit.ComputeUnitFillLevelPercentages(src).FillLevelBcPercentage))
                .ReverseMap();
            CreateMap<LatestUnitReading, UnitReadingDto>()
                .ForMember(dest => dest.BinLevelPercentage, opt => opt.MapFrom(src => src.Unit.ComputeUnitFillLevelPercentages(src).FillLevelPercentage))
                .ForMember(dest => dest.BinLevelBcPercentage, opt => opt.MapFrom(src => src.Unit.ComputeUnitFillLevelPercentages(src).FillLevelBcPercentage))
                .ReverseMap();
            CreateMap<UnitReading, ShortUnitReadingDto>()
                .ForMember(dest => dest.BatteryVoltage, opt => opt.MapFrom(src => src.BatteryVoltage.ConvertBatteryVoltage()))
                .ForMember(dest => dest.BinLevelBc, opt => opt.MapFrom(src => src.Unit.ComputeUnitFillLevelPercentages(src).FillLevelBcPercentage))
                .ForMember(dest => dest.BinLevel, opt => opt.MapFrom(src => src.Unit.ComputeUnitFillLevelPercentages(src).FillLevelPercentage));
            CreateMap<LatestUnitReading, ShortUnitReadingDto>()
                .ForMember(dest => dest.BatteryVoltage, opt => opt.MapFrom(src => src.BatteryVoltage.ConvertBatteryVoltage()))
                .ForMember(dest => dest.BinLevelBc, opt => opt.MapFrom(src => src.Unit.ComputeUnitFillLevelPercentages(src).FillLevelBcPercentage))
                .ForMember(dest => dest.BinLevel, opt => opt.MapFrom(src => src.Unit.ComputeUnitFillLevelPercentages(src).FillLevelPercentage));
            CreateMap<Unit, UnitDetailsDto>()
                .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.LatestUnitReadings.FirstOrDefault().Temperature))
                .ForMember(dest => dest.LastReadingCreateDate, opt => opt.MapFrom(src => src.LatestUnitReadings.FirstOrDefault().ReadingDate))
                .ForMember(dest => dest.BatteryVoltage, opt => opt.MapFrom(src => src.LatestUnitReadings.FirstOrDefault().BatteryVoltage.ConvertBatteryVoltage()))
                .ForMember(dest => dest.BatteryVoltage, opt => opt.MapFrom(src => src.LatestUnitReadings.FirstOrDefault().BatteryVoltage.ConvertBatteryVoltage()))
                .ReverseMap();
            CreateMap<Unit, UnitTableDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.BatteryVoltage, opt => opt.MapFrom(src => src.LatestUnitReadings.FirstOrDefault().BatteryVoltage.ConvertBatteryVoltage()))
                .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.LatestUnitReadings.FirstOrDefault().Temperature))
                .ForMember(dest => dest.LastUnitReadingCreateDate, opt => opt.MapFrom(src => src.LatestUnitReadings.FirstOrDefault().CreateDate))
                .ReverseMap();
            CreateMap<Unit, UnitDamageTableDto>()
                 .ForMember(dest => dest.ModelTypeName, opt => opt.MapFrom(src => src.UnitModelType.Name));
            CreateMap<UnitImage, UnitImageDto>().ReverseMap();

            CreateMap<Damage, DamageDetailsDto>()
                 .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.DamageHistories.FirstOrDefault().User.Name));
            CreateMap<Damage, DamageTableDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.DamageHistories.OrderBy(dh=> dh.CreateDate).FirstOrDefault().User.Name))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.DamageHistories.OrderBy(dh=> dh.CreateDate).FirstOrDefault().Image))
                .ForMember(dest => dest.AssignedUserName, opt => opt.MapFrom(src => src.DamageHistories.OrderByDescending(dh=> dh.CreateDate).FirstOrDefault().AssignedUser.Name));
            CreateMap<DamageHistory, DamageHistoryDto>();
            CreateMap<DamageHistory, UpdateDamageInput>().ReverseMap();

            CreateMap<GetUsersInput, DatatableOptions>();
            CreateMap<AlertType, AlertTypeDto>();
            CreateMap<AlertType, ShortAlertTypeDto>();
            CreateMap<UnitReading, LatestUnitReading>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<LatestUnitReading, LatestUnitReading>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
