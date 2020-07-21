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
        private static int NUM_DECKS = 8;

        // Values for card sprite sheet, 4x13 and each card is 79x123
        private int userBalance;

        // Stores all cards and pops from top to deal
        Queue<Card> cardList;

        // All playing cards
        GameCards cards = new GameCards(NUM_DECKS);

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

        // Image for dealer hidden card
        private Image backOfCardImage;

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

            // Load image
            backOfCardImage = new Image
            {
                Source = cards.GetBackCardImageSource()
            };
            // TODO: fix android
            SetLayout();

            // Preparing and shuffling cards for game
            cardList = cards.Shuffle();
        }

        private void NewGame()
        {
            // Start a new game and clear current field
            playerStack.Children.Clear();
            dealerStack.Children.Clear();
            balance.Text = "Looks like you have $" + userBalance;
            // Clear current decks and reshuffle
            cardList.Clear();
            cardList = cards.Shuffle();
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
            } 
            

        }

        private async void OnBetClicked(object sender, EventArgs e)
        {
            // Get int value from bet and display it
            string result = await DisplayPromptAsync("Welcome", "How much do you want to bet?", initialValue: "5", keyboard: Keyboard.Numeric);
            while(Convert.ToInt32(result) > userBalance)
            {
                result = await DisplayPromptAsync("Woah", "Lets bet what we have, no loans here", initialValue: "5", keyboard: Keyboard.Numeric);
            }
            
            currentBet = Convert.ToInt32(result);
            userBalance -= currentBet;
            balance.Text = "Looks like you have $" + userBalance;
            bet.Text = "BET = $" + currentBet; 
            betButton.IsVisible = false;
            startButton.IsVisible = true;
        }

        // Deals card to player when pressed
        private void OnGameStart(object sender, EventArgs e)
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
            playerScore += card1.GetNumericValue();
            
            Card dealerCard1 = cardList.Dequeue();
            Image dealerCard1Image = new Image
            {
                Source = dealerCard1.GetImageSource()
            };
            dealerStack.Children.Add(backOfCardImage);
            dealerCards.Add(dealerCard1);
            dealerCardImages.Add(dealerCard1Image);
            dealerScore += dealerCard1.GetNumericValue();

            // deal second card
            Card card2 = cardList.Dequeue();
            Image card2Image = new Image
            {
                Source = card2.GetImageSource()
            };
            playerStack.Children.Add(card2Image);
            playerCards.Add(card2);
            playerCardImages.Add(card2Image);
            playerScore += card2.GetNumericValue();
            

            Card dealerCard2 = cardList.Dequeue();
            Image dealerCard2Image = new Image
            {
                Source = dealerCard2.GetImageSource()
            };
            dealerStack.Children.Add(dealerCard2Image);
            dealerCards.Add(dealerCard2);
            dealerCardImages.Add(dealerCard2Image);
            dealerScore += dealerCard2.GetNumericValue();

            // Checking if player got ace in initial deal
            if (card1.GetNumericValue() == 1 || card2.GetNumericValue() == 1)
            {
                playerHasAce = true;
                if(playerScore == 2)
                {
                    playerAces++;
                }
                playerAces++;
                playerScore += 10;
                
            }

            // Checking if dealer got ace in initial deal
            if (dealerCard1.GetNumericValue() == 1 || dealerCard2.GetNumericValue() == 1)
            {
                dealerHasAce = true;
                if(dealerScore == 2)
                {
                    dealerAces++;
                }
                dealerAces++;
                dealerScore += 10;
            }

            // checking for any blackjacks
            if(playerScore == 21 && dealerScore < 21)
            {
                currentBet = (int)(currentBet * 1.5);
                DealerBust();
            } else if( playerScore < 21 && dealerScore == 21)
            {
                dealerStack.Children.Remove(backOfCardImage);
                dealerStack.Children.Add(dealerCard1Image);
                PlayerBust();
            } else if( playerScore == 21 && dealerScore == 21)
            {
                TieGame();
            }

            startButton.IsVisible = false;
            standButton.IsVisible = true;
            hitButton.IsVisible = true;
        }

        private void OnHitButtonClick(object sender, EventArgs e)
        {
            // deal next card
            Card nextCard = cardList.Dequeue();
            Image nextCardImage = new Image
            {
                Source = nextCard.GetImageSource()
            };
            playerStack.Children.Add(nextCardImage);
            playerCards.Add(nextCard);
            playerCardImages.Add(nextCardImage);
            playerScore += nextCard.GetNumericValue();

            // Check player score after next card
            if( playerHasAce && playerScore > 21 && playerAces > 0)
            {
                playerScore -= 10;
                playerAces--;
            }
            if( playerScore > 21)
            {
                PlayerBust();    
            }
            if (nextCard.GetNumericValue() == 1 )
            {
                playerHasAce = true;
                playerAces++;
            }
        }

        private void OnStandButtonClick(object sender, EventArgs e)
        {
            // Remove hit button and check dealer hand
            hitButton.IsVisible = false;
            dealerStack.Children.Remove(backOfCardImage);
            dealerStack.Children.Add(dealerCardImages.ElementAt(0));
            CheckScore();
            
        }

        private void DealerHit()
        {
            Card nextCard = cardList.Dequeue();
            Image nextCardImage = new Image
            {
                Source = nextCard.GetImageSource()
            };
            dealerStack.Children.Add(nextCardImage);
            dealerCards.Add(nextCard);
            dealerCardImages.Add(nextCardImage);
            dealerScore += nextCard.GetNumericValue();

            // Check dealer score after hit
            if (dealerHasAce && dealerScore > 21 && dealerAces > 0)
            {
                dealerScore -= 10;
                dealerAces--;
            }
            if (nextCard.GetNumericValue() == 1)
            {
                dealerHasAce = true;
                dealerAces++;
            }
            if (dealerScore > 21)
            {
                DealerBust();
            }
            else
            {
                CheckScore();
            }
            
        }

        private void CheckScore()
        {
            if (dealerScore < 17)
            {
                DealerHit();
            }
            else if ( dealerScore < playerScore)
            {
                DealerHit();
            }
            else if( dealerScore > playerScore && dealerScore < 22)
            {
                PlayerBust();
            }
            else if( dealerScore == playerScore)
            {
                TieGame();
            } 
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
            playerScore = 0;
            dealerScore = 0;
            bet.Text = "0";
            App.CurrentCash = userBalance;
            if (answer)
            {
                NewGame();
            }
            else
            {
                // TODO: Application.Current.Quit();
            }

        }
    }
}