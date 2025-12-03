using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Queries.Customers;

public class GetAllCustomersQuery : IRequest<IEnumerable<CustomerDto>>
{
}

