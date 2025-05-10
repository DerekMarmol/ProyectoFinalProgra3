using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;

namespace SuperBodega.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto)
        {
            var customer = new Customer
            {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                Phone = customerDto.Phone,
                Address = customerDto.Address
            };

            await _customerRepository.AddAsync(customer);

            customerDto.Id = customer.Id;
            return customerDto;
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer != null)
            {
                await _customerRepository.DeleteAsync(customer);
            }
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            
            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address
            });
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };
        }

        public async Task UpdateCustomerAsync(CustomerDto customerDto)
        {
            var customer = await _customerRepository.GetByIdAsync(customerDto.Id);
            if (customer != null)
            {
                customer.FirstName = customerDto.FirstName;
                customer.LastName = customerDto.LastName;
                customer.Email = customerDto.Email;
                customer.Phone = customerDto.Phone;
                customer.Address = customerDto.Address;

                await _customerRepository.UpdateAsync(customer);
            }
        }
    }
}