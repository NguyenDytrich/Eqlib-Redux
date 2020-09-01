using EqlibApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EqlibApi.Models.Db
{
    public class Checkout
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public ICollection<Item> Items { get; set; }
        [Required]
        public DateTime CheckoutDate { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public DateTime ReturnDate { get; set; }
        public ECheckoutStatus? CheckoutStatus { get; set; }
        [Required]
        public ECheckoutApproval ApprovalStatus { get; set; }

        [NotMapped]
        [Required]
        public IEnumerable<int> ItemIds { get; set; }

        public Checkout() { }
    }
}
