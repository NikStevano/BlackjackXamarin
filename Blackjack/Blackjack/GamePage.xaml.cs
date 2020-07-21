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

        // Holds dealer cards
        private List<Card> dealerCards = new List<Card>();
        private List<Image> dealerCardImages = new List<Image>();
        // Holds player cards
        private List<Card> playerCards = new List<Card>();
        private List<Image> playerCardImages = new List<Image>();
        // Image for dealer hidden card
        private Image backOfCardImage;


        public GamePage(int money)
        {
            userBalance = money;
            InitializeComponent();
            balance.Text = "Looks like you have $" + userBalance;

            // Preparing and shuffling cards for game
            GameCards cards = new GameCards(8);
            cardList = cards.Shuffle();
            // Load image
            backOfCardImage = new Image
            {
                Source = cards.GetBackCardImageSource()
            };
        }

        // Deals card to player when pressed
        void OnGameStart(object sender, EventArgs e)
        {
            // Deal first cards
            Card card1 = cardList.Dequeue();
            Image card1Image = new Image
            {
                Source = card1.GetImageSource()
            };
            playerStack.Children.Add(card1Image);
            playerCards.Add(card1);
            playerCardImages.Add(card1Image);

            Card dealerCard1 = cardList.Dequeue();
            Image dealerCard1Image = new Image
            {
                Source = dealerCard1.GetImageSource()
            };
            dealerStack.Children.Add(backOfCardImage);
            dealerCards.Add(dealerCard1);
            dealerCardImages.Add(dealerCard1Image);

            // deal second card
            Card card2 = cardList.Dequeue();
            Image card2Image = new Image
            {
                Source = card2.GetImageSource()
            };
            playerStack.Children.Add(card2Image);
            playerCards.Add(card2);
            playerCardImages.Add(card2Image);

            Card dealerCard2 = cardList.Dequeue();
            Image dealerCard2Image = new Image
            {
                Source = dealerCard2.GetImageSource()
            };
            dealerStack.Children.Add(dealerCard2Image);
            dealerCards.Add(dealerCard2);
            dealerCardImages.Add(dealerCard2Image);

            startButton.IsVisible = false;
            standButton.IsVisible = true;
            hitButton.IsVisible = true;
        }

        void OnHitButtonClick(object sender, EventArgs e)
        {

        }

        void OnStandButtonClick(object sender, EventArgs e)
        {

        }
        
    }
}