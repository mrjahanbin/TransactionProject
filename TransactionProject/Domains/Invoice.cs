using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionProject.Domains
{
    public class Invoice
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public List<int> ProductIds { get; set; }
        [Column(TypeName = "decimal(26,2)")]
        public decimal TotalAmount { get; set; }


        [ForeignKey("UserId")]
        public User User { get; set; }
    }


}
