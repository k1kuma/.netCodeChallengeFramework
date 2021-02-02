using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WorldWideBankSample.Dtos;
using WorldWideBankSample.Services;

namespace WorldWideBankSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IFetchCustomersQuery _fetchCustomersQuery;
        private readonly IFetchCustomerQuery _fetchCustomerQuery;
        private readonly ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand;

        public CustomersController(IFetchCustomersQuery fetchCustomersQuery, IFetchCustomerQuery fetchCustomerQuery, 
            ICreateOrUpdateCustomerCommand createOrUpdateCustomerCommand)
        {
            _fetchCustomersQuery = fetchCustomersQuery;
            _fetchCustomerQuery = fetchCustomerQuery;
            _createOrUpdateCustomerCommand = createOrUpdateCustomerCommand;
        }

        [HttpGet]
        public async Task<IEnumerable<CustomerDto>> Get()
        {
            return await _fetchCustomersQuery.Fetch();
        }

        [HttpGet("{customerId}")]
        public async Task<CustomerDto> Get(int customerId)
        {
            return await _fetchCustomerQuery.Fetch(customerId);
        }


        [HttpPut]
        public async Task<CustomerDto> Put(CustomerDto customerDto)
        {
            return await _createOrUpdateCustomerCommand.Execute(customerDto);
        }
    }
}
