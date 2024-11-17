using TransactionProject.Domains;

namespace TransactionProject.DB
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> ProductRepository { get; }
        IRepository<Invoice> InvoiceRepository { get; }
        IRepository<User> UserRepository { get; }
        Task CommitAsync();
        Task CommitTransactionAsync();
        Task BeginTransactionAsync();
        Task RollbackAsync();
        Task DisposeAsync();

    }


}
