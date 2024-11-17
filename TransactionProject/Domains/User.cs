using System.ComponentModel.DataAnnotations;

namespace TransactionProject.Domains
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }


        public ICollection<Invoice> Invoices { get; set; }

    }
}
