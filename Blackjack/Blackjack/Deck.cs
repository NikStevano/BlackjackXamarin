using System;
using System.IO;
using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Blackjack
{
    /// <summary>
    /// Implements single deck object containing 52 card objects
    /// </summary>
    public class Deck
    {
        // Initial deck-laration, 52 cards because no jokers
        private Card[] Cards = new Card[52];

        // Images for cards
        private Image[] Images = new Image[52];

        public Deck()
        {
            InitDeck();
        }

        public Card[] GetCards()
        {
            return Cards;
        }

        public Card GetCard(int CardID)
        {
            return Cards[CardID];
        }

        public Image GetCardImage(int CardID)
        {
            return Images[CardID];
        }

        // Generating Deck object, 4 suits with 13 cards each
        private void InitDeck()
        {

            SKBitmap resourceBitmap;
            // Using Skia to load bitmap from embedded resource
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(Core.CARD_SHEET_RESOURCE))
            {
                resourceBitmap = SKBitmap.Decode(stream);
            }

            for ( int i=0; i<52; i++) {
                int cardSuiteIndex = i % 4;  // card Suite
                int cardValueIndex = i % 13; // card Value, i.e "A", "10" ...
                int cardNumValue = (i+1) % 13;  // card Numeric Value
                if (cardNumValue > 10)
                    cardNumValue = 10;

                Cards[i] = new Card(i, Core.Suits[cardSuiteIndex], Core.Values[cardValueIndex], cardNumValue );

                SKBitmap card = new SKBitmap(Core.CARD_WIDTH, Core.CARD_HEIGHT);

                SKRect dest = new SKRect(0, 0, Core.CARD_WIDTH, Core.CARD_HEIGHT);
                SKRect source = new SKRect(cardValueIndex * Core.CARD_WIDTH, cardSuiteIndex * Core.CARD_HEIGHT, (cardValueIndex + 1) * Core.CARD_WIDTH, (cardSuiteIndex + 1) * Core.CARD_HEIGHT);

                // Extract sprite from sprite sheet
                using (SKCanvas canvas = new SKCanvas(card))
                {
                    canvas.DrawBitmap(resourceBitmap, source, dest);
                }
                // Convert Skia bitmap to forms image source
                Image image = new Image
                {
                    Source = (SKBitmapImageSource)card
                };
                Images[i] = image;
            }
        }
    }
}
