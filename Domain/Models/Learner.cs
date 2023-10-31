namespace Domain.Models
{
#pragma warning disable CS8618
    public class Learner
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        //
        public User User { get; set; }
        public ICollection<ExamResult> ExamResults { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
