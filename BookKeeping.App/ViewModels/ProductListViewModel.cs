using BookKeeping.Domain.Contracts;
using BookKeeping.Projections.ProductsList;
using BookKeeping.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace BookKeeping.App.ViewModels
{
    public class ProductListViewModel : ListViewModel<ProductViewModel>, IPrintable, ISaveable
    {
        private Projections.ProductsList.ProductListView _productListView;
        private ISession _session = Context.Current.GetSession();

        public ProductListViewModel()
        {
            DisplayName = T("ListOfProducts");
        }

        protected override IEnumerable<ProductViewModel> LoadItems()
        {
            _productListView = _session.Query<ProductListView>().Convert(t => t, new ProductListView());
            return GetProducts(_productListView);
        }

        protected virtual IEnumerable<ProductViewModel> GetProducts(ProductListView view)
        {
            return view.Products.Select((p, i) => new ProductViewModel
            {
                Id = p.Id.Id,
                Barcode = p.Barcode,
                IsOrderable = p.IsOrderable,
                ItemNo = p.ItemNo,
                Price = p.Price,
                Stock = p.Stock,
                Title = p.Title,
                UnitOfMeasure = p.UnitOfMeasure,
                VatRate = p.VatRate,
                HasChanges = false,
                IsValid = true
            });
        }

        protected override Func<ProductViewModel, bool> DoSearch(string searchText)
        {
            return new Func<ProductViewModel, bool>(t => string.IsNullOrWhiteSpace(searchText) ? true : t.ItemNo.IndexOf(searchText) > -1);
        }

        public void Print()
        {
            SendMessage(new MessageEnvelope(T("PrintingNotCompleted")));
            //var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            //bool? result = saveFileDialog.ShowDialog();
            //if (result == true)
            //{
            //    App.Current.Dispatcher.BeginInvoke((Action)delegate
            //    {
            //        var exporter = new PdfExporter();
            //        //var documentPaginator = new DataGridDocumentPaginator((DataGrid)PrintArea, string.Empty, new System.Windows.Size(940, 1070), new System.Windows.Thickness());
            //        exporter.Export(PrintArea, saveFileDialog.FileName);
            //        SendMessage(new MessageEnvelope(T("PrintCompleted")));
            //    });
            //}
        }

        protected override void SaveNewItems(IEnumerable<ProductViewModel> newItems)
        {
            foreach (var item in newItems)
            {
                var product = _productListView.Products.Find(t => t.Id == new ProductId(item.Id));
                if (product == null)
                {
                    var id = _session.GetId();
                    _session.Command(new CreateProduct(new ProductId(id), item.Title, item.ItemNo, item.Price, item.Stock, item.UnitOfMeasure, item.VatRate, item.Barcode));
                    item.Id = id;
                }
            }
        }

        protected override void SaveUpdatedItems(IEnumerable<ProductViewModel> updatesItems)
        {
            foreach (var item in updatesItems)
            {
                var product = _productListView.Products.Find(t => t.Id == new ProductId(item.Id));
                if (product.Barcode != item.Barcode)
                {
                    _session.Command(new ChangeProductBarcode(product.Id, item.Barcode));
                }
                if (product.IsOrderable != item.IsOrderable)
                {
                    if (item.IsOrderable)
                    {
                        _session.Command(new MakeProductOrderable(product.Id));
                    }
                    else
                    {
                        _session.Command(new MakeProductNonOrderable(product.Id, "manual edited"));
                    }
                }
                if (product.ItemNo != item.ItemNo)
                {
                    _session.Command(new ChangeProductItemNo(product.Id, item.ItemNo));
                }
                if (product.Price != item.Price)
                {
                    _session.Command(new ChangeProductPrice(product.Id, item.Price));
                }
                if (product.Stock != item.Stock)
                {
                    _session.Command(new UpdateProductStock(product.Id, item.Stock, "manual edited"));
                }
                if (product.Title != item.Title)
                {
                    _session.Command(new RenameProduct(product.Id, item.Title));
                }
                if (product.UnitOfMeasure != item.UnitOfMeasure)
                {
                    _session.Command(new ChangeProductUnitOfMeasure(product.Id, item.UnitOfMeasure));
                }
                if (product.VatRate != item.VatRate)
                {
                    _session.Command(new ChangeProductVatRate(product.Id, item.VatRate));
                }
            }
        }

        protected override void SaveDeletedItems(IEnumerable<ProductViewModel> deletedItems)
        {
            foreach (var item in deletedItems)
            {
                _session.Command(new DeleteProduct(new ProductId(item.Id)));
            }
        }

        protected override void CommitChanges()
        {
            _session.Commit();
        }
    }
}