using BookKeeping.Domain.Services;
using BookKeeping.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace BookKeeping.Domain.Aggregates
{
    public class Order : IEntity
    {
        readonly IOrderCalculator _calculator;
        readonly IProductService _productService;
        const string OrderNumberPrefix = "order no.";

        public long Id { get; set; }

        public string OrderNumber { get; set; }

        public DateTime DateCreated { get; protected set; }

        public DateTime DateModified { get; set; }

        public DateTime? DateFinalized { get; protected set; }

        public bool IsFinalized { get { return DateFinalized.HasValue; } }

        public decimal TotalQuantity
        {
            get
            {
                return this.OrderLines.Sum((OrderLine ol) => ol.Quantity);
            }
        }

        public OrderLineCollection OrderLines { get; protected set; }

        public VatRate VatRate { get; protected set; }

        public CurrencyAmount TotalPriceInclVat { get; protected set; }

        public CurrencyAmount TotalPrice { get; protected set; }

        public Order(IOrderCalculator calculator, IProductService productService)
        {
            Contract.Requires<ArgumentNullException>(calculator != null);
            Contract.Requires<ArgumentNullException>(productService != null);

            _calculator = calculator;
            _productService = productService;

            this.OrderNumber = string.Empty;
            this.OrderLines = new OrderLineCollection();
            this.VatRate = Domain.VatRate.Zero;
            this.TotalPriceInclVat = CurrencyAmount.Unspecifined;
            this.TotalPrice = CurrencyAmount.Unspecifined;
            this.DateCreated = Current.UtcNow;
        }

        public void Calculate()
        {
            var result = _calculator.CalculateOrder(this);
            TotalPrice = result.TotalPrice;
            TotalPriceInclVat = result.TotalPriceInclVat;
            VatRate = result.VatRate;
        }

        public void Complete()
        {
            if (!this.IsFinalized)
            {
                Calculate();
                this.DateFinalized = Current.UtcNow;
                this.OrderNumber = string.Format("{0}{1:##########}", OrderNumberPrefix, Id);
                RemoveItemsFromStock(this.OrderLines, 1m, _productService);
            }
        }

        private static void RemoveItemsFromStock(IEnumerable<OrderLine> orderLines, decimal parentOrderLineQuantity, IProductService productService)
        {
            foreach (OrderLine current in orderLines)
            {
                decimal? stock = productService.GetStock(current.ProductId);
                if (stock.HasValue)
                {
                    productService.SetStock(current.ProductId, stock.Value - current.Quantity * parentOrderLineQuantity);
                }
            }
        }

        public override bool Equals(object obj)
        {
            Order order = obj as Order;
            return order != null
                && (this.Id == order.Id
                && this.OrderNumber == order.OrderNumber
                && this.DateCreated.AddTicks(-(this.DateCreated.Ticks % 10000000L)) == order.DateCreated.AddTicks(-(order.DateCreated.Ticks % 10000000L))
                && this.DateModified.AddTicks(-(this.DateModified.Ticks % 10000000L)) == order.DateModified.AddTicks(-(order.DateModified.Ticks % 10000000L))
                && ((!this.DateFinalized.HasValue && !order.DateFinalized.HasValue) || (this.DateFinalized.HasValue && order.DateFinalized.HasValue
                && this.DateFinalized.Value.AddTicks(-(this.DateFinalized.Value.Ticks % 10000000L)) == order.DateFinalized.Value.AddTicks(-(order.DateFinalized.Value.Ticks % 10000000L))))
                && this.OrderLines.Equals(order.OrderLines)
                && this.VatRate.Equals(order.VatRate)
                && this.TotalPriceInclVat.Equals(order.TotalPriceInclVat)
                && this.TotalPrice.Equals(order.TotalPrice));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
