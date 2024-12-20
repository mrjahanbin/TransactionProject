﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionProject.Domains
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "decimal(26,2)")]
        public decimal Price { get; set; }
    }


}
