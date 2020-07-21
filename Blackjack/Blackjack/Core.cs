using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Blackjack
{
    public class Core
    {

        // Width and height of a single card in pixels
        public static int CARD_WIDTH = 79;
        public static int CARD_HEIGHT = 123;

        // Number of decks to play
        public static int NUM_DECKS = 8;

        // Embedded resource ID
        public static string CARD_SHEET_RESOURCE = "Blackjack.cards.png";

        // Card Suits
        public static String[] Suits = new String[] { "Clubs", "Diamonds", "Hearts", "Spades" };
        // Card Values
        public static String[] Values = new String[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        // Single card deck
        private Deck deck;

        private Image backOfCardImage;

        public Core()
        {
            deck = new Deck();
            SetBackCardImage();
        }

        public Card GetCard(int cardID)
        {
            return deck.GetCard(cardID);
        }

        public Image GetCardImage(int cardID)
        {
            return deck.GetCardImage(cardID);
        }

        public Image GetBackOfCardImage()
        {
            return backOfCardImage;
        }

        // Load back of card image from sprite sheet
        private void SetBackCardImage()
        {
            SKBitmap resourceBitmap;

            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(CARD_SHEET_RESOURCE))
            {
                resourceBitmap = SKBitmap.Decode(stream);
            }

            int row = 4;
            int col = 2;
            SKBitmap card = new SKBitmap(CARD_WIDTH, CARD_HEIGHT);

            SKRect dest = new SKRect(0, 0, CARD_WIDTH, CARD_HEIGHT);
            SKRect source = new SKRect(col * CARD_WIDTH, row * CARD_HEIGHT, (col + 1) * CARD_WIDTH, (row + 1) * CARD_HEIGHT);

            using (SKCanvas canvas = new SKCanvas(card))
            {
                canvas.DrawBitmap(resourceBitmap, source, dest);
            }

            backOfCardImage = new Image
            {
                Source = (SKBitmapImageSource)card
            };

        }

        // Generates game deck by shuffling all current decks and puts them in queue randomly
        public Queue<Card> GetGameCards()
        {
            List<Card> shuffleCards = new List<Card>();
            Random rand = new Random();
            Card[] deckCards = deck.GetCards();

            for (int i = 0; i < NUM_DECKS; i++)
            {
                for (int j = 0; j < deckCards.Length; j++)
                {
                    int insertPosition = shuffleCards.Count > 1 ? rand.Next(shuffleCards.Count + 1) : 0;
                    Card card = deckCards[j];
                    shuffleCards.Insert(insertPosition, card);
                }
            }
            // Use queue instead of list for popping cards off top
            Queue<Card> readyDeck = new Queue<Card>(shuffleCards);
            /*
            foreach( Card c in shuffleCards)
            {
                readyDeck.Enqueue(c);
            }
            */
            return readyDeck;
        }

    }
}
