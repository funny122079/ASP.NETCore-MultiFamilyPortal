using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using MultiFamilyPortal.Collections;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class FilterableUsers : ReactiveObject, IDisposable
    {
        private CompositeDisposable _disposables { get; }
        private IEnumerable<UserAccountResponse> _allUsers;
        private bool _disposedValue;

        private ObservableRangeCollection<UserAccountResponse> _filtered { get; }

        public FilterableUsers()
        {
            _disposables = new ();
            _filtered = new();
            SearchCommand = ReactiveCommand.Create<string>(OnSearchExecuted)
                .DisposeWith(_disposables);
            this.WhenAnyValue(x => x.Query)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .InvokeCommand(SearchCommand)
                .DisposeWith(_disposables);
        }

        private bool ContainsQuery(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return value.Contains(Query, StringComparison.OrdinalIgnoreCase);
        }

        [Reactive]
        public string Query { get; set; }

        private ReactiveCommand<string, Unit> SearchCommand { get; }

        public IEnumerable<UserAccountResponse> Users => _filtered;

        public void UpdateUsers(IEnumerable<UserAccountResponse> users)
        {
            _allUsers = users;
            OnSearchExecuted(Query);
        }

        private void OnSearchExecuted(string query)
        {
            if (string.IsNullOrEmpty(query?.Trim()))
            {
                _filtered.ReplaceRange(_allUsers);
                return;
            }

            _filtered.ReplaceRange(_allUsers.Where(x => ContainsQuery(x.LastName) ||
                    ContainsQuery(x.FirstName) ||
                    ContainsQuery($"{x.FirstName} {x.LastName}") ||
                    ContainsQuery(x.Email) ||
                    ContainsQuery(x.Phone)));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                    _disposables.Clear();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
