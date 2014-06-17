using BookKeeping.Domain.Contracts;
using BookKeeping.Projections.UserList;
using BookKeeping.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System;
using BookKeeping.Domain;

namespace BookKeeping.App.ViewModels
{
    public class UserListViewModel : ListViewModel<UserViewModel>, ISaveable
    {
        private ISession _session = Context.Current.GetSession();
        private Projections.UserList.UserListView _userListView;

        public UserListViewModel()
        {
            DisplayName = T("ListOfUsers");
        }

        protected virtual IEnumerable<UserViewModel> GetUsers(UserListView view)
        {
            return view.Users.Select((u, i) => new UserViewModel
            {
                Id = u.Id.Id,
                Login = u.Login,
                Name = u.Name,
                RoleType = u.RoleType,
                HasChanges = false,
                IsValid = true
            });
        }

        protected override System.Func<UserViewModel, bool> DoSearch(string searchText)
        {
            return new System.Func<UserViewModel, bool>(t => t.Name.IndexOf(searchText) > -1 || t.RoleType.IndexOf(searchText) > -1);
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

        protected override IEnumerable<UserViewModel> LoadItems()
        {
            _userListView = _session.Query<UserListView>().Convert(t => t, new UserListView());
            return GetUsers(_userListView);
        }

        protected override void SaveNewItems(IEnumerable<UserViewModel> newItems)
        {
            foreach (var item in newItems)
            {
                var product = _userListView.Users.Find(t => t.Id == new UserId(item.Id));
                if (product == null)
                {
                    //todo:
                    //var userId = new UserId(_session.GetId());
                    //_session.Command(new CreateUser(userId, item.Name, item.Login, item.NewPassword, item.RoleType));
                    //item.Id = userId.Id;
                }
                else throw new ArgumentException();
            }
        }

        protected override void SaveDeletedItems(IEnumerable<UserViewModel> deletedItems)
        {
            foreach (var item in deletedItems)
            {
                _session.Command(new DeleteUser(new UserId(item.Id)));
            }
        }

        protected override void SaveUpdatedItems(IEnumerable<UserViewModel> updatesItems)
        {
            foreach (var item in updatesItems)
            {
                 var product = _userListView.Users.Find(t => t.Id == new UserId(item.Id));
                 if (product.Name != item.Name)
                 {
                     _session.Command(new RenameUser(product.Id, item.Name));
                 }
                 if (product.RoleType != item.RoleType)
                 {
                     _session.Command(new AssignRoleToUser(product.Id, item.RoleType));
                 }
            }
        }

        protected override void CommitChanges()
        {
            _session.Commit();
        }
    }
}
