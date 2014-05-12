using BookKeeping.App.Domain.Services;
using BookKeeping.App.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.App.Domain.Aggregates.Order
{
    public class Order : IEntity
    {
        public long Id { get; set; }

        public string CartNumber
        {
            get;
            set;
        }

        public string OrderNumber
        {
            get;
            set;
        }

        public DateTime DateCreated
        {
            get;
            set;
        }

        public DateTime DateModified
        {
            get;
            set;
        }

        public DateTime? DateFinalized
        {
            get;
            set;
        }

        public bool IsFinalized
        {
            get
            {
                return this.DateFinalized.HasValue;
            }
        }

        public decimal TotalQuantity
        {
            get
            {
                return this.OrderLines.Sum((OrderLine ol) => ol.Quantity);
            }
        }

        public OrderLineCollection OrderLines
        {
            get;
            set;
        }

        public VatRate VatRate
        {
            get;
            set;
        }

        public CurrencyAmount SubtotalPrice
        {
            get;
            set;
        }

        public CurrencyAmount TotalPrice
        {
            get;
            set;
        }

        public Order()
        {
            this.OrderLines = new OrderLineCollection();
            this.VatRate = Domain.VatRate.Zero;
            this.SubtotalPrice = CurrencyAmount.Unspecifined;
            this.TotalPrice = CurrencyAmount.Unspecifined;
        }

        public void Finalize(IProductService productService)
        {
            if (!this.IsFinalized)
            {
                this.DateFinalized = Current.UtcNow;
                //TODO: this.OrderNumber = store.OrderNumberPrefix + store.GetNextOrderNumber(true);
                this.RemoveItemsFromStock(this.OrderLines, 1m, productService);
            }
        }

        private void RemoveItemsFromStock(IEnumerable<OrderLine> orderLines, decimal parentOrderLineQuantity, IProductService productService)
        {
            foreach (OrderLine current in orderLines)
            {
                decimal? stock = productService.GetStock(current.ItemNo);
                if (stock.HasValue)
                {
                    productService.SetStock(current.ItemNo, stock.Value - current.Quantity * parentOrderLineQuantity);
                }
            }
        }

        public override bool Equals(object obj)
        {
            Order order = obj as Order;
            return order != null
                && (this.Id == order.Id
                && this.CartNumber == order.CartNumber
                && this.OrderNumber == order.OrderNumber
                && this.DateCreated.AddTicks(-(this.DateCreated.Ticks % 10000000L)) == order.DateCreated.AddTicks(-(order.DateCreated.Ticks % 10000000L))
                && this.DateModified.AddTicks(-(this.DateModified.Ticks % 10000000L)) == order.DateModified.AddTicks(-(order.DateModified.Ticks % 10000000L))
                && ((!this.DateFinalized.HasValue && !order.DateFinalized.HasValue) || (this.DateFinalized.HasValue && order.DateFinalized.HasValue
                && this.DateFinalized.Value.AddTicks(-(this.DateFinalized.Value.Ticks % 10000000L)) == order.DateFinalized.Value.AddTicks(-(order.DateFinalized.Value.Ticks % 10000000L))))
                && this.OrderLines.Equals(order.OrderLines)
                && this.VatRate.Equals(order.VatRate)
                && this.SubtotalPrice.Equals(order.SubtotalPrice)
                && this.TotalPrice.Equals(order.TotalPrice));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
