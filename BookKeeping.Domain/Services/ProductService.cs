using BookKeeping.Domain.InformationExtractors;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IProductInformationExtractor _productInformationExtractor;
        private readonly IStoreService _storeService;

        public static IProductService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<IProductService>();
            }
        }

        public ProductService(IProductRepository repository, IProductInformationExtractor productInformationExtractor, IStoreService storeService)
        {
            this._repository = repository;
            this._productInformationExtractor = productInformationExtractor;
            this._storeService = storeService;
        }

        public long GetStoreId(string productIdentifier)
        {
            return this._productInformationExtractor.GetStoreId(productIdentifier);
        }

        public string GetPropertyValue(string productIdentifier, string propertyAlias)
        {
            return this._productInformationExtractor.GetPropertyValue(productIdentifier, propertyAlias);
        }

        public string GetSku(string productIdentifier)
        {
            return this._productInformationExtractor.GetSku(productIdentifier);
        }

        public long? GetVatGroupId(string productIdentifier)
        {
            return this._productInformationExtractor.GetVatGroupId(productIdentifier);
        }

        public long? GetLanguageId(string productIdentifier)
        {
            return this._productInformationExtractor.GetLanguageId(productIdentifier);
        }

        public OriginalUnitPriceCollection GetOriginalUnitPrices(string productIdentifier)
        {
            return this._productInformationExtractor.GetOriginalUnitPrices(productIdentifier);
        }

        public CustomPropertyCollection GetProperties(string productIdentifier)
        {
            return this._productInformationExtractor.GetProperties(productIdentifier);
        }

        public ProductSnapshot GetSnapshot(string productIdentifier)
        {
            return this._productInformationExtractor.GetSnapshot(productIdentifier);
        }

        public decimal? GetStock(long storeId, string sku)
        {
            decimal? result = null;
            if (!string.IsNullOrEmpty(sku))
            {
                storeId = (this._storeService.Get(storeId).StockSharingStoreId ?? storeId);
                result = this._repository.GetStock(storeId, sku);
            }
            return result;
        }

        public void SetStock(long storeId, string sku, decimal? value)
        {
            if (!string.IsNullOrEmpty(sku))
            {
                storeId = (this._storeService.Get(storeId).StockSharingStoreId ?? storeId);
                this._repository.SetStock(storeId, sku, value);
            }
        }

        public bool HasAccess(long storeId, string productIdentifier)
        {
            return this._productInformationExtractor.HasAccess(storeId, productIdentifier);
        }
    }
}