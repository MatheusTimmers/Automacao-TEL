using MainSpecAn;
using MainSpecAn.Session;
using System;
using System.Reflection;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

namespace AutomaçãoTEL
{
    public sealed partial class MainPage : Page
    {
        // TestSession carrega o estado de sessão (IP, instrumento, pasta, alias)
        // e é passado como parâmetro de navegação para todas as páginas.
        private readonly TestSession _session = new TestSession();

        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(420, 650);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (muxc.NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is muxc.NavigationViewItem && item.Tag.ToString() == "Home")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
            ContentFrame.Navigate(typeof(Views.Home), _session, new EntranceNavigationTransitionInfo());
        }

        private void NavView_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private bool TryGoBack()
        {
            if (!ContentFrame.CanGoBack)
                return false;

            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == muxc.NavigationViewDisplayMode.Compact ||
                 NavView.DisplayMode == muxc.NavigationViewDisplayMode.Minimal))
                return false;

            ContentFrame.GoBack();
            NavView.IsBackEnabled = false;
            return true;
        }

        public bool NavigateToView(string clickedView)
        {
            var view = Assembly.GetExecutingAssembly()
                .GetType($"AutomaçãoTEL.Views.{clickedView}");

            if (string.IsNullOrWhiteSpace(clickedView) || view == null)
                return false;

            object parameter = clickedView == "Login" ? (object)NavView : _session;
            ContentFrame.Navigate(view, parameter, new EntranceNavigationTransitionInfo());
            return true;
        }

        private void NavView_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
        {
            var item = args.InvokedItemContainer as muxc.NavigationViewItem;
            if (item == null) return;

            if (args.IsSettingsInvoked)
                NavigateToView("Config");
            else
                NavigateToView(item.Tag.ToString());
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Falha ao carregar página: " + e.SourcePageType.FullName);
        }

        private void AccountBt_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavigateToView("Login");
        }
    }
}
