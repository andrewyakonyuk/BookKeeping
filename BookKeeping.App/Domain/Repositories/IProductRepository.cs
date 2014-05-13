using BookKeeping.App.Domain.Aggregates;
namespace BookKeeping.App.Domain.Repositories
{
    public interface IProductRepository
    {
        decimal GetStock(string itemNo);

        void SetStock(string itemNo, decimal stock);

        Maybe<Product> Get(string itemNo);

        void Save(Product product);
    }
}
