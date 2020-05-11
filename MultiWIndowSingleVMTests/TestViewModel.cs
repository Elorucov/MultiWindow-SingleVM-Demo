using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiWIndowSingleVMTests {
    public class TestViewModel : BaseViewModel {
        private string _firstName;
        private string _lastName;
        private ThreadSafeObservableCollection<TestViewModel> _testCollection;
        private TestViewModel _selected;
        private RelayCommand _add;
        private RelayCommand _backgroundAdd;
        private RelayCommand _move;
        private RelayCommand _backgroundMove;
        private RelayCommand _deleteLast;
        private RelayCommand _clear;

        public string FirstName { get { return _firstName; } set { _firstName = value; OnPropertyChanged(); } }
        public string LastName { get { return _lastName; } set { _lastName = value; OnPropertyChanged(); } }
        public ThreadSafeObservableCollection<TestViewModel> TestCollection { get { return _testCollection; } set { _testCollection = value; OnPropertyChanged(); } }
        public TestViewModel Selected { get { return _selected; } set { _selected = value; OnPropertyChanged(); } }
        public RelayCommand Add { get { return _add; } set { _add = value; OnPropertyChanged(); } }
        public RelayCommand BackgroundAdd { get { return _backgroundAdd; } set { _backgroundAdd = value; OnPropertyChanged(); } }
        public RelayCommand Move { get { return _move; } set { _move = value; OnPropertyChanged(); } }
        public RelayCommand BackgroundMove { get { return _backgroundMove; } set { _backgroundMove = value; OnPropertyChanged(); } }
        public RelayCommand DeleteLast { get { return _deleteLast; } set { _deleteLast = value; OnPropertyChanged(); } }
        public RelayCommand Clear { get { return _clear; } set { _clear = value; OnPropertyChanged(); } }

        public TestViewModel(bool dontCreateCollection = false) {
            Add = new RelayCommand((k) => {
                TestCollection.Add(new TestViewModel(true) { FirstName = "Added item", LastName = new Random().Next(1, 999).ToString() });
            });
            BackgroundAdd = new RelayCommand((k) => {
                Thread t = new Thread(() => {
                    TestCollection.Add(new TestViewModel(true) { FirstName = "Added item in another thread!", LastName = new Random().Next(1, 999).ToString() });
                });
                t.IsBackground = true;
                t.Start();
            });
            Move = new RelayCommand((k) => {
                TestCollection.FastMove(TestCollection.Count - 1, 0);
            });
            BackgroundMove = new RelayCommand((k) => {
                //Thread t = new Thread(() => {
                //    TestCollection.FastMove(TestCollection.Count - 1, 0);
                //});
                //t.IsBackground = true;
                //t.Start();
                MoveBkg();
            });
            DeleteLast = new RelayCommand((k) => {
                TestCollection.Remove(TestCollection.Last());
            });
            Clear = new RelayCommand((k) => {
                TestCollection.Clear();
            });

            if(dontCreateCollection) return;
            TestCollection = new ThreadSafeObservableCollection<TestViewModel>();
            while (TestCollection.Count < 7) {
                TestCollection.Add(new TestViewModel(true) { FirstName = "Item", LastName = TestCollection.Count.ToString() });
            }
        }

        private void MoveBkg() {
            Task.Factory.StartNew(() => {
                TestCollection.FastMove(TestCollection.Count - 1, 0);
            }).ConfigureAwait(false);
        }
    }
}