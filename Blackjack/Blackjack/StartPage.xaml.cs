using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Blackjack
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class StartPage : ContentPage
    {
        private int[] CashLookup = new int[] { 10, 50, 100, 500 };
        public StartPage()
        {
            InitializeComponent();
        }
        void OnContinueButtonClicked(object sender, EventArgs e)
        {
            if (CashPicker.SelectedIndex >= 0)
            {
                int UserCashIndex = CashPicker.SelectedIndex;
                int UserCash = CashLookup[UserCashIndex];
                App.StartingCash = UserCash;
                Application.Current.MainPage = new GamePage(UserCash);
            }
            else
            {
                DisplayAlert("Error", "Please select a cash amount", "OK");
                return;
            }
        }
    }
}
