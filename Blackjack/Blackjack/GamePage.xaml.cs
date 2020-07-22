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
using Xamarin.Essentials;

namespace Blackjack
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        // Values for card sprite sheet, 4x13 and each card is 79x123
        private int userBalance;

        // Core game objects
        private Core core = new Core();

        // Stores all cards and pops from top to deal
        private Queue<Card> cardList;

        Random rand = new Random();

        private int currentBet;

        // Holds dealer cards
        private List<Card> dealerCards = new List<Card>();
        private List<Image> dealerCardImages = new List<Image>();
        private int dealerScore;

        // Holds player cards
        private List<Card> playerCards = new List<Card>();
        private List<Image> playerCardImages = new List<Image>();
        private int playerScore;

        // bool to see if player/dealer have ace for later calculations
        private bool playerHasAce = false;
        private bool dealerHasAce = false;
        private int playerAces = 0;
        private int dealerAces = 0;


        public GamePage(int money)
        {
            // Initialize initial money
            userBalance = money;
            InitializeComponent();
            balance.Text = "Looks like you have $" + userBalance;

            // TODO: fix android
            SetLayout();

            // Preparing and shuffling cards for game
            cardList = core.GetGameCards();
        }

        private void NewGame()
        {
            // Start a new game and clear current field
            playerStack.Children.Clear();
            dealerStack.Children.Clear();
            playerCards.Clear();
            dealerCards.Clear();

            playerScore = 0;
            dealerScore = 0;

            // Clear current decks and reshuffle
            cardList.Clear();
            cardList = core.GetGameCards();

            // Update UI
            bet.Text = "0";
            balance.Text = "Looks like you have $" + userBalance;
            standButton.IsVisible = false;
            hitButton.IsVisible = false;
            startButton.IsVisible = false;
            betButton.IsVisible = true;
        }

        // Setting layout for game screen
        private void SetLayout()
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            int displayWidth = (int)(mainDisplayInfo.Width / mainDisplayInfo.Density);
            int displayHeight = (int)(mainDisplayInfo.Height / mainDisplayInfo.Density);
            
            if(Device.RuntimePlatform == Device.iOS)
            {
                dealerStack.HeightRequest = displayHeight * 0.3;
                playerStack.HeightRequest = displayHeight * 0.3;
                potArea.HeightRequest = displayHeight / 0.1;
                infoArea.HeightRequest = displayHeight / 0.2;
            } else
            {
                dealerStack.VerticalOptions = LayoutOptions.StartAndExpand;
                playerStack.VerticalOptions = LayoutOptions.EndAndExpand;
            }
            

        }

        private async void OnBetClicked(object sender, EventArgs e)
        {
            // Get int value from bet and display it ... you can turn the tables on the casino and bet negative number ;)
            try
            {
                string result = await DisplayPromptAsync("Welcome", "How much do you want to bet?", initialValue: "5", keyboard: Keyboard.Numeric);
                while (Convert.ToInt32(result) > userBalance)
                {
                    result = await DisplayPromptAsync("Woah", "Lets bet what we have, no loans here", initialValue: "5", keyboard: Keyboard.Numeric);
                }

                currentBet = Convert.ToInt32(result);
                userBalance -= currentBet;
                balance.Text = "Looks like you have $" + userBalance;
                bet.Text = "BET = $" + currentBet;
                betButton.IsVisible = false;
                startButton.IsVisible = true;
            } catch(FormatException)
            {
                // ignore, let them bet again
            }
        }

        // Deals card to player when pressed
        private void OnGameStart(object sender, EventArgs e)
        {
            // Deal first cards
            Card card1 = cardList.Dequeue();
            playerStack.Children.Add(core.GetCardImage(card1.CardID));
            playerCards.Add(card1);
            playerCardImages.Add(core.GetCardImage(card1.CardID));
            Card dealerCard1 = cardList.Dequeue();
            dealerStack.Children.Add(core.GetBackOfCardImage());  // hidden card
            dealerCards.Add(dealerCard1);
            dealerCardImages.Add(core.GetCardImage(dealerCard1.CardID));

            // deal second card
            Card card2 = cardList.Dequeue();
            playerStack.Children.Add(core.GetCardImage(card2.CardID));
            playerCards.Add(card2);
            playerCardImages.Add(core.GetCardImage(card2.CardID));
            

            Card dealerCard2 = cardList.Dequeue();
            dealerStack.Children.Add(core.GetCardImage(dealerCard2.CardID));
            dealerCards.Add(dealerCard2);
            dealerCardImages.Add(core.GetCardImage(dealerCard2.CardID));

            playerScore = CalculatePlayerScore();
            dealerScore = CalculateDealerScore();

            // checking for any blackjacks
            if (playerScore == 21 && dealerScore < 21)
            {
                currentBet = (int)(currentBet * 1.5);
                DealerBust();
            }
            else if (playerScore < 21 && dealerScore == 21)
            {
                dealerStack.Children.Remove(core.GetBackOfCardImage());
                dealerStack.Children.Add(core.GetCardImage(dealerCard1.CardID));
                PlayerBust();
            }
            else if (playerScore == 21 && dealerScore == 21)
            {
                TieGame();
            }
            else
            {
                startButton.IsVisible = false;
                standButton.IsVisible = true;
                hitButton.IsVisible = true;
            }
        }

        private void OnHitButtonClick(object sender, EventArgs e)
        {
            // deal next card
            Card nextCard = cardList.Dequeue();
            playerStack.Children.Add(core.GetCardImage(nextCard.CardID));
            playerCards.Add(nextCard);
            playerCardImages.Add(core.GetCardImage(nextCard.CardID));

            // Check player score after next card
            playerScore = CalculatePlayerScore();

            if( playerScore > 21)
            {
                PlayerBust();    
            }
            else if( playerScore == 21)
            {
                // no point in having HIT
                hitButton.IsVisible = false;
            }
        }

        private void OnStandButtonClick(object sender, EventArgs e)
        {
            // Remove hit button and check dealer hand
            hitButton.IsVisible = false;
            standButton.IsVisible = false;
            dealerStack.Children.Remove(core.GetBackOfCardImage());
            dealerStack.Children.Add(dealerCardImages.ElementAt(0));
            DealerAction();
            
        }

        private void DealerHit()
        {
            Card nextCard = cardList.Dequeue();
            dealerStack.Children.Add(core.GetCardImage(nextCard.CardID));
            dealerCards.Add(nextCard);
            dealerCardImages.Add(core.GetCardImage(nextCard.CardID));

            // Check dealer score after hit
            dealerScore = CalculateDealerScore();
            if (dealerScore > 21)
                DealerBust();
            else
                DealerAction();
        }

        private void DealerAction()
        {
            if (dealerScore < 17)
            {
                DealerHit();
            }
            else if ( dealerScore < playerScore)
            {
                DealerHit();
            }
            else if (dealerScore > playerScore && dealerScore < 22)
            {
                PlayerBust();
            }
            else if ( dealerScore == playerScore)
            {
                // tie game, check number of cards
                if (dealerCards.Count > playerCards.Count)
                    DealerBust();
                else if (dealerCards.Count < playerCards.Count)
                    PlayerBust();
                else
                    TieGame();
            } 
        }

        private int CalculatePlayerScore()
        {
            int score = 0;
            int offset = 0;
            foreach( Card c in playerCards)
            {
                int s = c.NumericValue;
                if( s == 1 ) // ACE
                {
                    s += 10;
                    offset += 10;
                }
                score += s;
            }
            while( score > 21 && offset > 0)
            {
                score -= 10;
                offset -= 10;
            }
            return score;
        }


        private int CalculateDealerScore()
        {
            int score = 0;
            int offset = 0;
            foreach (Card c in dealerCards)
            {
                int s = c.NumericValue;
                if (s == 1) // ACE
                {
                    s += 10;
                    offset += 10;
                }
                score += s;
            }
            while (score > 21 && offset > 0)
            {
                score -= 10;
                offset -= 10;
            }
            return score;
        }


        // Method called when player goes bust
        private async void PlayerBust()
        {
            // Reset player score after game
            bool answer = await DisplayAlert("Game Over", "Dealer Wins", "One More", "No More");
            CheckForNewGame(answer);
        }

        // Method called when dealer goes bust
        private async void DealerBust()
        {
            // Reset player score after game
            bool answer = await DisplayAlert("Game Over", "You Win", "One More", "No More");
            userBalance += currentBet * 2;
            CheckForNewGame(answer);
        }

        // Method called when game ties
        private async void TieGame()
        {
            // Reset player score after game
            bool answer = await DisplayAlert("Game Over", "Draw", "One More", "No More");
            userBalance += currentBet;
            CheckForNewGame(answer);
        }

        private void CheckForNewGame(bool answer)
        {
            App.CurrentCash = userBalance;
            if(userBalance < 1)
            {
                DisplayAlert("Out of money", "ATM awaits", "Okay");
                Application.Current.MainPage = new StartPage();
            }
            if (answer)
            {
                NewGame();
            }
            else
            {
                Application.Current.MainPage = new StartPage();
            }

        }
    }
}