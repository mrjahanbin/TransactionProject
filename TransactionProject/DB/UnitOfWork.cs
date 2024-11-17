using Microsoft.EntityFrameworkCore.Storage;
using TransactionProject.Domains;

namespace TransactionProject.DB
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;

        // Constructors for the UnitOfWork class.
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private Repository<Product> _productRepository;
        private Repository<User> _userRepository;
        private Repository<Invoice> _invoiceRepository;

        // Repositories initialization via lazy loading.
        public IRepository<Product> ProductRepository => _productRepository ??= new Repository<Product>(_context);
        public IRepository<User> UserRepository => _userRepository ??= new Repository<User>(_context);
        public IRepository<Invoice> InvoiceRepository => _invoiceRepository ??= new Repository<Invoice>(_context);

        // Dispose method to release resources (including the context and transaction).
        public void Dispose()
        {
            _transaction?.Dispose();  // Dispose transaction if exists.
            _context.Dispose();  // Dispose of DbContext.
        }

        // Method to begin a new transaction.
        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("A transaction is already started.");

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        // Method to commit the transaction.
        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();  // Save changes to the database.
        }

        // Method to rollback the transaction.
        public async Task RollbackAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction has been started.");

            await _transaction.RollbackAsync();  // Rollback the transaction if something went wrong.
            _transaction.Dispose();  // Dispose of the transaction after rollback.
        }

        public async Task DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(); // رول‌بک تراکنش
                await _transaction.DisposeAsync();  // آزادسازی منابع تراکنش
                _transaction = null;
            }
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction has been started.");

            await _context.SaveChangesAsync();  // Save changes to the database.
            await _transaction.CommitAsync();  // Commit the transaction.
        }
    }
}
