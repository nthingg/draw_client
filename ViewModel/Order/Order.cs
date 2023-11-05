using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Order
{
    public class Order
    {
        public int Id { get; set; }
        public int LearnerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public bool IsPaid { get; set; }
        public string? TransactionId { get; set; }
        public int? PaymentMethod { get; set; }
        //
        //public Learner Learner { get; set; }
        public ICollection<OrderDetail> Details { get; set; }
    }
}
