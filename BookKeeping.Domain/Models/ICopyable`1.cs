namespace BookKeeping.Domain.Models
{
    public interface ICopyable<T>
    {
        T Copy();
    }
}