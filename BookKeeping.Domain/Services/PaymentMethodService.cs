using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Models;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private const string CacheKey = "PaymentMethods";
        private readonly IPaymentMethodRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly object _cacheLock = new object();

        public static IPaymentMethodService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<IPaymentMethodService>();
            }
        }

        public PaymentMethodService(IPaymentMethodRepository repository, ICacheService cacheService)
        {
            this._repository = repository;
            this._cacheService = cacheService;
            NotificationCenter.PaymentMethod.Created += new PaymentMethodEventHandler(this.PaymentMethodCreated);
        }

        public IEnumerable<PaymentMethod> GetAll(long storeId)
        {
            return
                from i in this.GetCachedList(storeId)
                where !i.IsDeleted
                orderby i.Sort
                select i;
        }

        public IEnumerable<PaymentMethod> GetAllAllowedIn(long storeId, long countryId, long? countryRegionId = null)
        {
            return (
                from pm in this.GetAll(storeId)
                where pm.AllowedInFollowingCountries.Contains(countryId) && (!countryRegionId.HasValue || pm.AllowedInFollowingCountryRegions.Contains(countryRegionId.Value))
                select pm).ToList<PaymentMethod>();
        }

        public PaymentMethod Get(long storeId, long paymentMethodId)
        {
            List<PaymentMethod> cachedList = this.GetCachedList(storeId);
            PaymentMethod paymentMethod = cachedList.SingleOrDefault((PaymentMethod i) => i.Id == paymentMethodId);
            if (paymentMethod == null)
            {
                lock (this._cacheLock)
                {
                    paymentMethod = cachedList.SingleOrDefault((PaymentMethod i) => i.Id == paymentMethodId);
                    if (paymentMethod == null)
                    {
                        paymentMethod = this._repository.Get(storeId, paymentMethodId);
                        if (paymentMethod != null)
                        {
                            cachedList.Add(paymentMethod);
                        }
                    }
                }
            }
            return paymentMethod;
        }

        private void PaymentMethodCreated(PaymentMethod paymentMethod)
        {
            List<PaymentMethod> cachedList = this.GetCachedList(paymentMethod.StoreId);
            if (cachedList.Any((PaymentMethod pm) => pm.Id == paymentMethod.Id))
            {
                return;
            }
            lock (this._cacheLock)
            {
                if (cachedList.All((PaymentMethod pm) => pm.Id != paymentMethod.Id))
                {
                    cachedList.Add(paymentMethod);
                }
            }
        }

        private List<PaymentMethod> GetCachedList(long storeId)
        {
            List<PaymentMethod> list = this._cacheService.GetCacheValue<List<PaymentMethod>>("PaymentMethods-" + storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<PaymentMethod>>("PaymentMethods-" + storeId);
                    if (list == null)
                    {
                        list = this._repository.GetAll(storeId).ToList<PaymentMethod>();
                        this._cacheService.SetCacheValue("PaymentMethods-" + storeId, list);
                    }
                }
            }
            return list;
        }
    }
}