using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Domain.Aggregates
{
    public sealed class Order : AggregateBase, IOrderState
    {
        readonly List<OrderLine> _orderLines = new List<OrderLine>();

        public IEnumerable<OrderLine> Lines { get { return _orderLines; } }

        public CurrencyAmount Total { get; private set; }

        public CurrencyAmount TotalWithVat { get; private set; }

        public VatRate VatRate { get; private set; }

        public OrderId Id { get; private set; }

        public CustomerId CustomerId { get; private set; }

        public bool IsCompleted { get; private set; }

        public decimal TotalQuantity { get { return this.Lines.Sum((OrderLine ol) => ol.Quantity); } }

        public Order(IEnumerable<IEvent> events) :
            base(events)
        {
            Total = CurrencyAmount.Unspecifined;
            TotalWithVat = CurrencyAmount.Unspecifined;
            VatRate = VatRate.Zero;
        }

        protected override void Mutate(IEvent e)
        {
            ((IOrderState)this).When((dynamic)e);
        }

        public void Create(OrderId id, CustomerId customerId, UserId userId, DateTime utc)
        {
            Apply(new OrderCreated(id, customerId, userId, utc));
        }

        public void Complete(UserId userId, DateTime utc)
        {
            Apply(new OrderCompleted(this.Id, userId, utc));
        }

        public void UpdateLineQuantity(ProductId productId, decimal quantity, UserId userId, DateTime utc)
        {
            if (_orderLines.Any(t => t.ProductId.Equals(productId)))
            {
                if (quantity <= 0)
                {
                    Apply(new ProductRemovedFromOrder(this.Id, productId, userId, utc));
                }
                else Apply(new ProductQuantityUpdatedInOrder(this.Id, productId, quantity, userId, utc));
            }
            else throw new ArgumentException();
        }

        public void AddLine(ProductId productId, string itemNo, string title, decimal quantity, CurrencyAmount amount, VatRate vatRate, UserId userId, DateTime utc)
        {
            int count = this._orderLines.Count;
            Apply(new ProductAddedToOrder(this.Id, count + 1, productId, itemNo, title, quantity, amount, vatRate, userId, utc));
        }

        public void RemoveLine(ProductId productId, UserId userId, DateTime utc)
        {
            Apply(new ProductRemovedFromOrder(this.Id, productId, userId, utc));
        }

        public void Calculate()
        {
            var currency = Currency.Uah;//todo: bug
           Total = Lines.Aggregate(new CurrencyAmount(0, currency), (seed, line) => seed = seed + line.Amount);
            TotalWithVat = Lines.Aggregate(new CurrencyAmount(0, currency), (seed, line) => seed = seed + (line.Amount + new CurrencyAmount(line.Amount.Amount * line.VatRate, currency)));
            VatRate = new VatRate((TotalWithVat - Total).Amount / 100);
        }

        void IOrderState.When(OrderCreated e)
        {
            this.Id = e.Id;
            this.CustomerId = e.CustomerId;
        }

        void IOrderState.When(OrderCompleted e)
        {
            this.IsCompleted = true;
        }

        void IOrderState.When(ProductAddedToOrder e)
        {
            var line = _orderLines.SingleOrDefault(t => t.ProductId.Equals(e.ProductId));
            if (line == null)
            {
                this._orderLines.Add(new OrderLine
                {
                    Id = e.LineId,
                    ItemNo = e.ItemNo,
                    Quantity = e.Quantity,
                    Amount = e.Amount,
                    Title = e.Title,
                    VatRate = e.VatRate
                });
            }
            else
            {
                UpdateLineQuantity(e.ProductId, e.Quantity, e.UserId, e.Utc);
            }
        }

        void IOrderState.When(ProductQuantityUpdatedInOrder e)
        {
            _orderLines.Single(t => t.ProductId.Equals(e.ProductId)).Quantity = e.Quantity;
        }

        void IOrderState.When(ProductRemovedFromOrder e)
        {
            var line = _orderLines.Find(t => t.ProductId.Equals(e.ProductId));
            if (line == null)
                return;
            _orderLines.Remove(line);
        }
    }
}