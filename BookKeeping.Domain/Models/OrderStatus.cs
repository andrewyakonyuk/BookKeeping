using BookKeeping.Domain.Notifications;
using BookKeeping.Domain.Repositories;
using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure.Dependency;
using BookKeeping.Infrastructure.Logging;

namespace BookKeeping.Domain.Models
{
    public class OrderStatus : ISortable
    {
        public long Id
        {
            get;
            set;
        }

        public long StoreId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Alias
        {
            get;
            set;
        }

        public int Sort
        {
            get;
            set;
        }

        public bool IsDeleted
        {
            get;
            set;
        }

        public OrderStatus()
        {
            this.Sort = -1;
        }

        public OrderStatus(long storeId, string name)
            : this()
        {
            this.StoreId = storeId;
            this.Name = name;
            this.Alias = name.ToCamelCase();
        }

        public void Save()
        {
            bool flag = this.Id == 0L;
            IOrderStatusRepository orderStatusRepository = DependencyResolver.Current.GetService<IOrderStatusRepository>();
            if (this.Sort == -1)
            {
                this.Sort = orderStatusRepository.GetHighestSortValue(this.StoreId) + 1;
            }
            orderStatusRepository.Save(this);
            if (flag)
            {
                NotificationCenter.OrderStatus.OnCreated(this);
            }
        }

        public bool Delete()
        {
            bool result = false;
            Store store = StoreService.Instance.Get(this.StoreId);
            if (store.DefaultOrderStatusId != this.Id)
            {
                this.IsDeleted = true;
                this.Save();
                result = true;
                NotificationCenter.OrderStatus.OnDeleted(this);
            }
            else
            {
                LoggingService.Instance.Log("Can't delete the order status " + this.Name + " because it is the default for the store " + store.Name);
            }
            return result;
        }
    }
}