using AutoMapper;
using WorldWideBank.Domain;
using WorldWideBank.Dtos;

namespace WorldWideBank.Services.Mappers
{
    public class AccountMappers: Profile
    {
        public AccountMappers()
        {
            CreateMap<Account, AccountDto>();
        }
    }
}
