using AutoMapper;
using Domain.DTOs.User;
using Domain.Entities;

namespace Application.Mappings;

public class UserMapper
{
    public static void CreateMap(IMapperConfigurationExpression config)
    {
        config.CreateMap<User, UserDto>();
    }
}