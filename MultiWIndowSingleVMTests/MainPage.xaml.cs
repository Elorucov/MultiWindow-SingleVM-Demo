using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace MultiWIndowSingleVMTests {
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page {
        public static TestViewModel StaticVM = new TestViewModel { FirstName = "Elchin", LastName = "Orujov" };

        public MainPage() {
            this.InitializeComponent();
            DataContext = StaticVM;
        }

        private async void NewWindow(object sender, RoutedEventArgs e) {
            await OpenNewWindowAsync(typeof(MainPage));
        }

        public static async Task<bool> OpenNewWindowAsync(Type page) {
            var currentAV = ApplicationView.GetForCurrentView();
            Window newWindow = null;
            var newAV = CoreApplication.CreateNewView();
            bool result = false;
            await newAV.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            async () => {
                                newWindow = Window.Current;
                                var newAppView = ApplicationView.GetForCurrentView();

                                newAV.CoreWindow.Closed += (a, b) => CoreApplication.DecrementApplicationUseCount();
                                CoreApplication.IncrementApplicationUseCount();
                                newAppView.SetPreferredMinSize(new Size(360, 500));

                                newWindow.Activate();
                                await Task.Yield();
                                var frame = new Frame();
                                frame.Navigate(page);
                                newWindow.Content = frame;

                                result = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppView.Id, ViewSizePreference.Custom, currentAV.Id, ViewSizePreference.Custom);
                            });
            return result;
        }
    }
}