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
        public String Suite { get; set; }
        public String Value { get; set; }
        public int NumericValue { get; set; }
        public int CardID { get; set; }

        public Card(int CardID, String Suite, String Value, int NumericValue)
        {
            this.CardID = CardID;
            this.NumericValue = NumericValue;
            this.Suite = Suite;
            this.Value = Value;
        }

    }
}
