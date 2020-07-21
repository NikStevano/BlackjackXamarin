using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms.Xaml;

namespace Blackjack
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        // Values for card sprite sheet, 4x13 and each card is 79x123
        private int userBalance;
        static int cardWidth = 79;
        static int cardHeight = 123;
        static int rows = 4;
        static int cols = 13;

        // Stores all cards and pops from top to deal
        Queue<Card> cardList;

        SKBitmap resourceBitmap;

        Random rand = new Random();

        public GamePage(int money)
        {
            userBalance = money;
            InitializeComponent();
            balance.Text = "Looks like you have $" + userBalance;

            // Preparing and shuffling cards for game
            GameCards cards = new GameCards(8);
            cardList = cards.Shuffle();
        }

        // Deals card to player when pressed
        void OnButtonClicked(object sender, EventArgs e)
        {
            // pops off queue just to test functionality
            Card card = cardList.Dequeue();

            Image cardImage = new Image
            {
                Source = card.GetImageSource()
            };

            playerStack.Children.Add(cardImage);


        }

        // TO:DO: creating game loop
        void GameStart()
        {

        }

    }
}