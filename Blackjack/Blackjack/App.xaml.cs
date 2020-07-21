using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Blackjack
{
    public partial class App : Application
    {

        public static int CurrentCash { get; set; }
        public App()
        {
            InitializeComponent();

            MainPage = new StartPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
