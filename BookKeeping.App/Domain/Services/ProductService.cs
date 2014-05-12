using BookKeeping.App.Domain.Repositories;

namespace BookKeeping.App.Domain.Services
{
    public class ProductService : IProductService
    {
        readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public decimal? GetStock(string itemNo)
        {
            decimal? result = null;
            if (!string.IsNullOrEmpty(itemNo))
            {
                result = this._repository.GetStock(itemNo);
            }
            return result;
        }

        public void SetStock(string itemNo, decimal? value)
        {
            if (!string.IsNullOrEmpty(itemNo))
            {
                this._repository.SetStock(itemNo, value ?? 0);
            }
        }
    }
}
