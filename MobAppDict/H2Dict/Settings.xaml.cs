using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using View.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace H2Dict
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        private readonly NavigationHelper navigationHelper;

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        private List<string> lstGender = new List<string>();

        public Settings()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.NavigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.NavigationHelper.SaveState += this.NavigationHelper_SaveState;

            lstGender.Add("male");
            lstGender.Add("female");
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ComboBoxTypeDict.ItemsSource = App.TypeDictIns.TypeDictList;
            ComboBoxTypeDict.SelectedIndex = App.TypeDictIns.GetInd();
            ComboBoxGender.ItemsSource = lstGender;
            ComboBoxGender.SelectedValue = App.TypeDictIns.Gender;
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

            if (Frame.CanGoBack)
            {
                //Frame.GoBack();
                e.Handled = true;
            }
        }

        private void ComboBoxTypeDict_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string nameDict = ComboBoxTypeDict.SelectedValue.ToString();
            int ind = ComboBoxTypeDict.SelectedIndex;
            App.TypeDictIns.SetTypeDict(ind);
            App.ChangeDict = true;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MessageDialog md = new MessageDialog("Coming soon... :)");
            md.ShowAsync();
        }

        private void ComboBoxGender_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int ind = ComboBoxGender.SelectedIndex;
            App.TypeDictIns.Gender = lstGender[ind];
            App.ChangeDict = true;
        }
    }
}
