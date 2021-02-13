using System.Threading.Tasks;
using AutoMapper;
using NHibernate;
using NHibernate.Linq;
using WorldWideBank.Domain;
using WorldWideBank.Dtos;

namespace WorldWideBank.Services
{
    public interface ICreateOrUpdateCustomerCommand
    {
        Task<CustomerDto> Execute(CustomerDto customerDto);
    }

    public class CreateOrUpdateCustomerCommand : ICreateOrUpdateCustomerCommand
    {
        private readonly ISession _session;
        private readonly IMapper _mapper;

        public CreateOrUpdateCustomerCommand(ISession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }

        public async Task<CustomerDto> Execute(CustomerDto customerDto)
        {
            var customer = await _session.Query<Customer>()
                .SingleOrDefaultAsync(x => x.CustomerId == customerDto.CustomerId) 
                           ?? new Customer { CustomerId = customerDto.CustomerId };

            customer.Name = customerDto.Name;

            await _session.SaveAsync(customer);

            return _mapper.Map<CustomerDto>(customer);
        }


    }
}
