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

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x416

namespace AutomaçãoTEL
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    
    public sealed partial class MainPage : Page
    {


        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(420, 650);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }


        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(muxc.NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is muxc.NavigationViewItem && item.Tag.ToString() == "Home")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
            ContentFrame.Navigate(typeof(AutomaçãoTEL.Views.Home));
        }

        public void ActivateBackButton()
        {
            NavView.IsBackEnabled = true;
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
            var view = Assembly.GetExecutingAssembly().GetType($"AutomaçãoTEL.Views.{clickedView}");



            if (string.IsNullOrWhiteSpace(clickedView) || view == null)
                return false;

            
            if(clickedView == "Login")
            {
                ContentFrame.Navigate(view, NavView, new EntranceNavigationTransitionInfo());
                return true;
            }
            else
            {
                ContentFrame.Navigate(view, null, new EntranceNavigationTransitionInfo());
                return true;
            }
 
        }

        private void NavView_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
        {
            var item = args.InvokedItemContainer as muxc.NavigationViewItem;
            if (item == null)
                return;
            if (args.IsSettingsInvoked == true)
            {
                NavigateToView("Config");
            }
            else
            {
                var clickedView = item.Tag.ToString();
                if (!NavigateToView(clickedView)) return;
            }
            
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void AccountBt_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavigateToView("Login");
        }

        private void NavView_PaneClosing(muxc.NavigationView sender, muxc.NavigationViewPaneClosingEventArgs args)
        {

        }
    }
}
