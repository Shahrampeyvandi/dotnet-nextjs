using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Queries.Customers;

public class GetCustomerByIdQuery : IRequest<CustomerDto?>
{
    public int Id { get; set; }
}

