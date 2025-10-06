namespace BE
{
    public class ProductRating
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public byte Rating { get; set; }
    }

    public class ProductRatingAggregate
    {
        public decimal Average { get; set; }
        public int Count { get; set; }
    }
}
