namespace BookKeeping.App.Domain.Repositories
{
    public interface IProductRepository
    {
        decimal GetStock(string itemNo);

        void SetStock(string itemNo, decimal stock);
    }
}
