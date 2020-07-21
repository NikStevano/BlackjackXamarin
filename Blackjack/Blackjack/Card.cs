using System;
using System.IO;
using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Blackjack
{
    /// <summary>
    /// Implements card object
    /// </summary>
    public class Card
    {
        // Height and Width for a single card from sprite sheet
        public static int cardWidth = 79;
        public static int cardHeight = 123;
        // Card Suits
        public static String[] Suits = new String[] { "Clubs", "Diamonds", "Hearts", "Spades" };
        // Card Values
        public static String[] Values = new String[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };


        public String Suite { get; set; }
        public String Value { get; set; }

        private ImageSource imageSource;

        public Card(String Suite, String Value)
        {
            this.Suite = Suite;
            this.Value = Value;
            SetImage();
        }

        public ImageSource GetImageSource()
        {
            return imageSource;
        }

        // Loading and setting image from sprite sheet
        private void SetImage()
        {
            SKBitmap resourceBitmap;
            // Using Skia to load bitmap from embedded resource
            string resourceID = "Blackjack.cards.png";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                resourceBitmap = SKBitmap.Decode(stream);
            }

            int row = GetOrderSuite();
            int col = GetOrderValue();
            SKBitmap card = new SKBitmap(cardWidth, cardHeight);

            SKRect dest = new SKRect(0, 0, cardWidth, cardHeight);
            SKRect source = new SKRect(col * cardWidth, row * cardHeight, (col + 1) * cardWidth, (row + 1) * cardHeight);

            // Extract sprite from sprite sheet
            using (SKCanvas canvas = new SKCanvas(card))
            {
                canvas.DrawBitmap(resourceBitmap, source, dest);
            }
            // Convert Skia bitmap to forms image source
            imageSource = (SKBitmapImageSource)card;

        }

        // Getting value of the card, 10 for picture cards
        public int GetNumericValue()
        {
            if (Value.StartsWith("A"))
                return 1;
            else if (Value.StartsWith("J") || Value.StartsWith("Q") || Value.StartsWith("K"))
                return 10;
            else
                return Int16.Parse(Value); // (int) Char.GetNumericValue(Value);  // or Value - "0"
        }

        // Getting suit of the card that loads image based on row
        public int GetOrderSuite()
        {
            if (Suite.StartsWith("Clu"))
                return 0;
            else if (Suite.StartsWith("Dia"))
                return 1;
            else if (Suite.StartsWith("Hea"))
                return 2;
            else if (Suite.StartsWith("Spa"))
                return 3;

            return 0;

        }

        // Getting value of the card that loads image based on column
        public int GetOrderValue()
        {
            if (Value.StartsWith("A"))
                return 0;
            else if (Value.StartsWith("J"))
                return 10;
            else if (Value.StartsWith("Q"))
                return 11;
            else if (Value.StartsWith("K"))
                return 12;
            else
                return Int16.Parse(Value) - 1; // (int) Char.GetNumericValue(Value);  // or Value - "0"
        }

    }
}
