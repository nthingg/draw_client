namespace Domain.Models
{
#pragma warning disable CS8618
    public class Order
    {
        public int Id { get; set; }
        public int LearnerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public bool IsPaid { get; set; }
        public int? TransactionId { get; set; }
        public string? PaymentMethod { get; set; }
        //
        public Learner Learner { get; set; }
        public ICollection<OrderDetail> Details { get; set; }
    }
}
