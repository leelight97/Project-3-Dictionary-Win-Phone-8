using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using H2Dict.Helper;
using H2Dict.ViewModel;
using View.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace H2Dict
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //    private ListWords _lstWords = new ListWords();
        //    public ListWords LstWords
        //    {
        //        get { return _lstWords; }
        //        set { _lstWords = value; }
        //    }

        private Dict _dict;

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

        public MainPage()
        {
            this.InitializeComponent();

            _dict = new Dict();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.NavigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.NavigationHelper.SaveState += this.NavigationHelper_SaveState;

            App.TypeDictIns.SetTypeDict(App.TypeDictIns.GetInd());

            if (App.TypeDictIns.Speech.Equals(""))
                ButtonSpeech.IsEnabled = false;
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            await Dict.LoadListWords();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            if (Dict.LstWord.LstKey.Count == 0)
                this.navigationHelper.OnNavigatedTo(e);

            if (App.ChangeDict)
            {
                if (App.TypeDictIns.Speech.Equals(""))
                    ButtonSpeech.IsEnabled = false;
                else ButtonSpeech.IsEnabled = true;
                Dict.LoadListWords();
            }


            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //remove the handler before you leave!
            //this.navigationHelper.OnNavigatedFrom(e);
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string str = txtSearch.Text;
            string res = await Dict.Search(str);


            txtDisplay.Text = res;
            // Lưu lược sử. + Speech
            if (res != "N/A")
            {
                Dict.UpdateTranslatedWords(str);
                TextBlockWord.Text = txtSearch.Text;
            }
        }

        private void txtSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = getSuggestionWord(sender.Text);
            }
        }

        private void txtSearch_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = txtSearch.Text;
        }

        // Suggestion word for AutoSuggestion
        private List<string> getSuggestionWord(string key)
        {
            return Dict.GetSuggestion(key);
        }

        private void AppBarButtonHistory_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (TranslatedWords));
        }

        private void AppBarButtonFavorite_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (FavoriteWords));
        }

        private void AppBarButtonSettings_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (Settings));
        }

        private async void ButtonAddFav_OnClick(object sender, RoutedEventArgs e)
        {
            if (txtDisplay.Text.Equals("N/A"))
            {
                string nofi = "Add favorite word fail!!!";
                MessageDialog md = new MessageDialog(nofi);
                await md.ShowAsync();
            }
            else
            {
                string nofi = "Add favorite word success!!!";
                MessageDialog md = new MessageDialog(nofi);
                await md.ShowAsync();

                Dict.UpdateFavoriteWords(txtSearch.Text);
            }
        }

        private void GridDisplay_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            // If you need the clicked element:
            // Item whichOne = senderElement.DataContext as Item;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private async void ButtonSpeech_OnClick(object sender, RoutedEventArgs e)
        {
            MediaElement audioPlayer = new MediaElement();
            SpeakText(audioPlayer, TextBlockWord.Text, App.TypeDictIns.Speech, App.TypeDictIns.Gender);
            //Speech Synthesis Markup Language

            //SpeakText(audioPlayer, str);
        }

        private async void SpeakText(MediaElement audioPlayer, string tts, string lang, string gender)
        {
            string str = @"<speak version=""1.0""
             xmlns=""http://www.w3.org/2001/10/synthesis"" xml:lang=""" + lang + @""">
             <voice gender=""" + gender + @"""> " + tts + @"
                        </voice>                      
                        </speak>";

            var ttsJP = new SpeechSynthesizer();
            SpeechSynthesisStream ttsStream = await ttsJP.SynthesizeSsmlToStreamAsync(str);
            audioPlayer.SetSource(ttsStream, "");
        }

        private async void AppBarButtonStt_OnClick(object sender, RoutedEventArgs e)
        {
            SpeechRecognitionResult speechRecognitionResult;
            try
            {
                SpeechRecognizer speechRecog = new SpeechRecognizer();

                // Compile the dictation grammar
                await speechRecog.CompileConstraintsAsync();

                // Start Recognition
                speechRecognitionResult = await speechRecog.RecognizeWithUIAsync();

                var sttDialog = new Windows.UI.Popups.MessageDialog(speechRecognitionResult.Text, "Heard You said...");

                sttDialog.Commands.Add(new UICommand("Ok"));
                sttDialog.Commands.Add(new UICommand("No"));

                var res = await sttDialog.ShowAsync();

                if (res.Label == "Ok")
                {
                    string str = speechRecognitionResult.Text;
                    txtSearch.Text = str.Substring(0,str.Length - 1);
                }
                // Show Output
            }
            catch
            {
                MessageDialog nofiMss =
                    new MessageDialog("Activate Speech Recognition through settings or long press windows key." +
                                      System.Environment.NewLine + "If Activated give Speech Input");
                nofiMss.ShowAsync();
                
            }

            

        }
    }
}