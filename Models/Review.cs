namespace Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; }= string.Empty;
        public int Rate { get; set; }
        public DateTime Date { get; set; }
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
