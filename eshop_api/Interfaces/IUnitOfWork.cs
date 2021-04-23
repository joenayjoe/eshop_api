using eshop_api.Models;
using System;
using System.Threading.Tasks;

namespace eshop_api.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryBase<Category> Categories { get; }
        IRepositoryBase<Product> Products { get; }
        IRepositoryBase<Customer> Customers { get; }
        IRepositoryBase<ShoppingCartRecord> ShoppingCartRecords { get; }
        IRepositoryBase<Order> Orders { get; }

        Task SaveAsync();
    }
}
