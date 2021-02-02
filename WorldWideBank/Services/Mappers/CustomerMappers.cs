using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WorldWideBankSample.Domain;
using WorldWideBankSample.Dtos;

namespace WorldWideBankSample.Services.Mappers
{
    public class CustomerMappers: Profile
    {
        public CustomerMappers()
        {
            CreateMap<Customer, CustomerDto>();
        }
    }
}
