using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionProject.DB;
using TransactionProject.Domains;

namespace TransactionProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public TransactionsController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }


        #region 001 - بدون Transaction
        [HttpPost("SimpleTransaction000")]
        public async Task<IActionResult> SimpleTransaction000(string userName, string productName, decimal price)
        {
            try
            {
                var user = new User { Name = userName };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var product = new Product { Name = productName, Price = price };

                if (product.Price <= 0)
                {
                    throw new Exception("product amount must be greater than zero.");
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return Ok("completed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"failed: {ex.Message}");
            }
        }
        #endregion

        #region 002 - حالت دستی با EF
        [HttpPost("SimpleTransaction001")]
        public async Task<IActionResult> SimpleTransaction001(string userName, string productName,decimal price)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User { Name = userName };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var product = new Product { Name = productName, Price = price };
                if (product.Price <= 0)
                {
                    throw new Exception("product amount must be greater than zero.");
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok("Transaction completed successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest($"Transaction failed: {ex.Message}");
            }

        }
        #endregion

        #region 003 با استفاده از Unit Of Work
        [HttpPost]
        public async Task<IActionResult> AddTransaction(string userName, string productName, int price)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Add User
                var user = new User { Name = userName };
                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.CommitAsync(); // ذخیره تغییرات در محدوده تراکنش

                // Add Product
                var product = new Product { Name = productName, Price = price };
                await _unitOfWork.ProductRepository.AddAsync(product);
                await _unitOfWork.CommitAsync(); // ذخیره تغییرات در محدوده تراکنش

                // Add Invoice
                var invoice = new Invoice { UserId = user.Id, ProductIds = new() { product.Id }, TotalAmount = product.Price };
                await _unitOfWork.InvoiceRepository.AddAsync(invoice);

                // تولید خطا برای تست رول‌بک
                if (invoice.TotalAmount <= 0)
                {
                    throw new Exception("Invoice amount must be greater than zero.");
                }

                await _unitOfWork.CommitTransactionAsync(); // ذخیره نهایی

                return Ok("Transaction completed successfully!");
            }
            catch
            {
                await _unitOfWork.DisposeAsync(); // رول‌بک تراکنش
                return BadRequest("Transaction failed. Rolled back changes.");
            }
        }
        #endregion

    }
}
