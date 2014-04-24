namespace BookKeeping.Domain.Models
{
    public interface ISortable
    {
        long Id { get; }

        string Name { get; }

        int Sort { get; set; }
    }
}