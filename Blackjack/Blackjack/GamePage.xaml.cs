using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Blackjack
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        private int userBalance;
        public GamePage(int money)
        {
            userBalance = money;
            InitializeComponent();
            balance.Text = "Looks like you have $" + userBalance;
        }
    }
}