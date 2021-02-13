using AutoMapper;
using WorldWideBank.Domain;
using WorldWideBank.Dtos;

namespace WorldWideBank.Services.Mappers
{
    public class CustomerMappers: Profile
    {
        public CustomerMappers()
        {
            CreateMap<Customer, CustomerDto>();
        }
    }
}
