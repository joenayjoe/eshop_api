using eshop_api.Interfaces;
using eshop_api.Models;
using System;
using System.Threading.Tasks;

namespace eshop_api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly EshopContext context;
        private RepositoryBase<Category> category;
        private RepositoryBase<Product> product;
        private RepositoryBase<ShoppingCartRecord> shoppingCartRecords;
        private RepositoryBase<Customer> customer;
        private RepositoryBase<Order> order;

        private bool disposed = false;

        public UnitOfWork(EshopContext context)
        {
            this.context = context;
        }
        public IRepositoryBase<Category> Categories => category ??= new RepositoryBase<Category>(context);

        public IRepositoryBase<Product> Products => product ??= new RepositoryBase<Product>(context);

        public IRepositoryBase<ShoppingCartRecord> ShoppingCartRecords => shoppingCartRecords ??= new RepositoryBase<ShoppingCartRecord>(context);

        public IRepositoryBase<Customer> Customers => customer ??= new RepositoryBase<Customer>(context);

        public IRepositoryBase<Order> Orders => order ??= new RepositoryBase<Order>(context);

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
