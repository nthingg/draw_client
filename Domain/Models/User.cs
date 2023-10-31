namespace Domain.Models
{
#pragma warning disable CS8618
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
        public bool IsBlocked { get; set; }
        //
        public Instructor? Instructor { get; set; }
        public Learner? Learner { get; set; }
    }
}
