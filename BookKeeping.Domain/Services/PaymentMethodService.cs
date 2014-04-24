using System;
using System.Collections.Generic;
using System.Linq;
using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Caching;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Domain.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly object _cacheLock = new object();
        private const string CacheKey = "PaymentMethods";
        private readonly IPaymentMethodRepository _repository;
        private readonly ICacheService _cacheService;

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

        public IEnumerable<BookKeeping.Domain.Models.PaymentMethod> GetAll(long storeId)
        {
            return (IEnumerable<BookKeeping.Domain.Models.PaymentMethod>)Enumerable.OrderBy<BookKeeping.Domain.Models.PaymentMethod, int>(Enumerable.Where<BookKeeping.Domain.Models.PaymentMethod>((IEnumerable<BookKeeping.Domain.Models.PaymentMethod>)this.GetCachedList(storeId), (Func<BookKeeping.Domain.Models.PaymentMethod, bool>)(i => !i.IsDeleted)), (Func<BookKeeping.Domain.Models.PaymentMethod, int>)(i => i.Sort));
        }

        public IEnumerable<BookKeeping.Domain.Models.PaymentMethod> GetAllAllowedIn(long storeId, long countryId, long? countryRegionId = null)
        {
            return (IEnumerable<BookKeeping.Domain.Models.PaymentMethod>)Enumerable.ToList<BookKeeping.Domain.Models.PaymentMethod>(Enumerable.Where<BookKeeping.Domain.Models.PaymentMethod>(this.GetAll(storeId), (Func<BookKeeping.Domain.Models.PaymentMethod, bool>)(pm =>
            {
                if (!pm.AllowedInFollowingCountries.Contains(countryId))
                    return false;
                if (countryRegionId.HasValue)
                    return pm.AllowedInFollowingCountryRegions.Contains(countryRegionId.Value);
                else
                    return true;
            })));
        }

        public BookKeeping.Domain.Models.PaymentMethod Get(long storeId, long paymentMethodId)
        {
            List<BookKeeping.Domain.Models.PaymentMethod> cachedList = this.GetCachedList(storeId);
            BookKeeping.Domain.Models.PaymentMethod paymentMethod = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.PaymentMethod>((IEnumerable<BookKeeping.Domain.Models.PaymentMethod>)cachedList, (Func<BookKeeping.Domain.Models.PaymentMethod, bool>)(i => i.Id == paymentMethodId));
            if (paymentMethod == null)
            {
                lock (this._cacheLock)
                {
                    paymentMethod = Enumerable.SingleOrDefault<BookKeeping.Domain.Models.PaymentMethod>((IEnumerable<BookKeeping.Domain.Models.PaymentMethod>)cachedList, (Func<BookKeeping.Domain.Models.PaymentMethod, bool>)(i => i.Id == paymentMethodId));
                    if (paymentMethod == null)
                    {
                        paymentMethod = this._repository.Get(storeId, paymentMethodId);
                        if (paymentMethod != null)
                            cachedList.Add(paymentMethod);
                    }
                }
            }
            return paymentMethod;
        }

        private void PaymentMethodCreated(BookKeeping.Domain.Models.PaymentMethod paymentMethod)
        {
            List<BookKeeping.Domain.Models.PaymentMethod> cachedList = this.GetCachedList(paymentMethod.StoreId);
            if (Enumerable.Any<BookKeeping.Domain.Models.PaymentMethod>((IEnumerable<BookKeeping.Domain.Models.PaymentMethod>)cachedList, (Func<BookKeeping.Domain.Models.PaymentMethod, bool>)(pm => pm.Id == paymentMethod.Id)))
                return;
            lock (this._cacheLock)
            {
                if (!Enumerable.All<BookKeeping.Domain.Models.PaymentMethod>((IEnumerable<BookKeeping.Domain.Models.PaymentMethod>)cachedList, (Func<BookKeeping.Domain.Models.PaymentMethod, bool>)(pm => pm.Id != paymentMethod.Id)))
                    return;
                cachedList.Add(paymentMethod);
            }
        }

        private List<BookKeeping.Domain.Models.PaymentMethod> GetCachedList(long storeId)
        {
            List<BookKeeping.Domain.Models.PaymentMethod> list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.PaymentMethod>>("PaymentMethods-" + (object)storeId);
            if (list == null)
            {
                lock (this._cacheLock)
                {
                    list = this._cacheService.GetCacheValue<List<BookKeeping.Domain.Models.PaymentMethod>>("PaymentMethods-" + (object)storeId);
                    if (list == null)
                    {
                        list = Enumerable.ToList<BookKeeping.Domain.Models.PaymentMethod>(this._repository.GetAll(storeId));
                        this._cacheService.SetCacheValue("PaymentMethods-" + (object)storeId, (object)list);
                    }
                }
            }
            return list;
        }
    }
}