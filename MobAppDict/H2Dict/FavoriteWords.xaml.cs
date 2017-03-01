using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using H2Dict.ViewModel;
using View.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace H2Dict
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoriteWords : Page
    {
        private Dict _dict = new Dict();

        public Dict Dict
        {
            get { return _dict; }
            set { _dict = value; }
        }

        private readonly NavigationHelper navigationHelper;

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public FavoriteWords()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Dict.LstFavoriteWords.Count == 0)
            {
                this.navigationHelper.OnNavigatedTo(e);
            }
                
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //remove the handler before you leave!
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame == null)
            {
                Application.Current.Exit();
            }

            else if (Frame.CanGoBack)
            {
                e.Handled = true;
            }
        }


        private void SelectAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            SelectListFavoriteWords.SelectionMode = ListViewSelectionMode.Multiple;
        }

        private async void OkAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (var item in SelectListFavoriteWords.SelectedItems)
            {
                Dict.LstFavoriteWords.Remove(item.ToString());
                
            }

            SelectListFavoriteWords.ItemsSource = null;
            SelectListFavoriteWords.ItemsSource = Dict.LstFavoriteWords;
            SelectListFavoriteWords.SelectionMode = ListViewSelectionMode.None;

            await Dict.UpdateFavoriteWords();
        }

        private void SelectListFavoriteWords_OnSelectionModeChanged(object sender, RoutedEventArgs e)
        {
            if (SelectListFavoriteWords.SelectionMode != ListViewSelectionMode.Multiple)
            {
                SelectAppBarButton.Visibility = Visibility.Visible;
                OkAppBarButton.Visibility = Visibility.Collapsed;
                SelectListFavoriteWords.IsItemClickEnabled = true;
            }
            else
            {
                SelectAppBarButton.Visibility = Visibility.Collapsed;
                OkAppBarButton.Visibility = Visibility.Visible;
                SelectListFavoriteWords.IsItemClickEnabled = false;
            }
        }

        private void SelectListFavoriteWords_OnItemClick(object sender, ItemClickEventArgs e)
        {
            string word = e.ClickedItem.ToString();
            Frame.Navigate(typeof(Meaning), word);
        }

        private async void SelectListFavoriteWords_OnLoaded(object sender, RoutedEventArgs e)
        {
            SelectListFavoriteWords.ItemsSource = null;
            SelectListFavoriteWords.ItemsSource = await Dict.LoadFavoriteWords();
        }

        private void AppBarButtonSelect_OnClick(object sender, RoutedEventArgs e)
        {
            SelectListFavoriteWords.SelectionMode = ListViewSelectionMode.Multiple;
            SelectListFavoriteWords.SelectAll();
        }

        private void AppBarButtonUnselect_OnClick(object sender, RoutedEventArgs e)
        {
            SelectListFavoriteWords.SelectionMode = ListViewSelectionMode.None;
        }
    }
}
