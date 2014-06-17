using BookKeeping.Domain.Contracts;
using BookKeeping.Domain;
using BookKeeping.Persistance;
using BookKeeping.Persistance.AtomicStorage;
using System.Linq;

namespace BookKeeping.Projections.OrdersList
{
    public class OrdersProjection :
        IEventHandler<OrderCreated>,
        IEventHandler<OrderCompleted>,
        IEventHandler<ProductAddedToOrder>,
        IEventHandler<ProductQuantityUpdatedInOrder>,
        IEventHandler<ProductRemovedFromOrder>
    {
        private readonly IDocumentWriter<unit, OrderListView> _store;

        public OrdersProjection(IDocumentWriter<unit, OrderListView> store)
        {
            _store = store;
        }

        public void When(OrderCreated e)
        {
            _store.UpdateEnforcingNew(unit.it, prs => prs.Orders.Add(
            new OrderView
            {
                Id = e.Id,
                TotalWithVat = CurrencyAmount.Unspecifined,
                Total = CurrencyAmount.Unspecifined,
                CustomerId = e.CustomerId,
                IsCompleted = false,
                TotalQuantity = 0
            }));
        }

        public void When(OrderCompleted e)
        {
            _store.UpdateOrThrow(unit.it, list => list.Orders.Find(p => p.Id == e.Id).IsCompleted = true);
        }

        public void When(ProductAddedToOrder e)
        {
            _store.UpdateOrThrow(unit.it, list =>
            {
                list.Orders.Single(o => o.Id == e.Id).Lines.Add(new OrderLineView
                {
                    Amount = e.Amount,
                    Id = e.LineId,
                    Quantity = e.Quantity,
                    ItemNo = e.ItemNo,
                    ProductId = e.ProductId,
                    Title = e.Title,
                    VatRate = e.VatRate
                });
            });
        }

        public void When(ProductQuantityUpdatedInOrder e)
        {
            _store.UpdateOrThrow(unit.it, list =>
            {
                list.Orders.Find(o => o.Id == e.Id).Lines.Find(t => t.ProductId.Equals(e.ProductId)).Quantity = e.Quantity;
            });
        }

        public void When(ProductRemovedFromOrder e)
        {
            _store.UpdateOrThrow(unit.it, list =>
            {
                var lines = list.Orders.Find(o => o.Id == e.Id).Lines;
                var line = lines.Find(t => t.ProductId.Equals(e.ProductId));
                lines.Remove(line);
            });
        }
    }
}