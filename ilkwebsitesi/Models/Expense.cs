namespace ilkwebsitesi.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public string? Name { get; set; }

        public string UserId { get; set; }

        public User user { get; set; }
    }
}
