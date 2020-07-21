using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace Blackjack
{
    /// <summary>
    /// Class to prepare and randomize decks for game
    /// </summary>
    public class GameCards
    {
        // How many decks are being used
        private int numberOfDecks;
        // Represents 1 deck
        private Deck deck = new Deck();
        // Image for backend of card
        private ImageSource backOfCard;
        public GameCards(int numberOfDecks)
        {
            this.numberOfDecks = numberOfDecks;
            SetBackCardImage();
        }

        // Load back of card image from sprite sheet
        private void SetBackCardImage()
        {
            SKBitmap resourceBitmap;

            string resourceID = "Blackjack.cards.png";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                resourceBitmap = SKBitmap.Decode(stream);
            }

            int row = 4;
            int col = 2;
            SKBitmap card = new SKBitmap(Card.cardWidth, Card.cardHeight);

            SKRect dest = new SKRect(0, 0, Card.cardWidth, Card.cardHeight);
            SKRect source = new SKRect(col * Card.cardWidth, row * Card.cardHeight, (col + 1) * Card.cardWidth, (row + 1) * Card.cardHeight);

            using (SKCanvas canvas = new SKCanvas(card))
            {
                canvas.DrawBitmap(resourceBitmap, source, dest);
            }

            backOfCard = (SKBitmapImageSource)card;

        }

        // Get back of card image
        public ImageSource GetBackCardImageSource()
        {
            return backOfCard;
        }

        // Generates game deck by shuffling all current decks and puts them in queue randomly
        public Queue<Card> Shuffle()
        {
            List<Card> shuffleCards = new List<Card>();
            Random rand = new Random();
            Card[] deckCards = deck.GetCards();

            for (int i = 0; i < numberOfDecks; i++)
            {
                for (int j = 0; j < deckCards.Length; j++)
                {
                    int insertPosition = shuffleCards.Count > 1 ? rand.Next(shuffleCards.Count + 1) : 0;
                    Card card = deckCards[j];
                    shuffleCards.Insert(insertPosition, new Card(card.Suite, card.Value));
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
