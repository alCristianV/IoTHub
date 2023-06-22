using AutoMapper;
using IoTHubAPI.Models;
using IoTHubAPI.Models.Dtos;

namespace IoTHubAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {
            CreateMap<User, UserForListDto>();
            CreateMap<Device, DeviceForAddDto>().ReverseMap();
            CreateMap<Device, DeviceForListDto>().ReverseMap();
            CreateMap<User, UserInfoDto>().ReverseMap();
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.DeviceId));
            CreateMap<DeviceDataField, DeviceDataFieldDto>().ReverseMap();
            CreateMap<DeviceDataField, DeviceDataFieldListDto>().ReverseMap();
            CreateMap<Models.Action, ActionDto>().ReverseMap();
            CreateMap<Models.Action, ActionListDto>().ReverseMap();
            CreateMap<ActionDto, ActionListDto>().ReverseMap();
        }
    }
}
