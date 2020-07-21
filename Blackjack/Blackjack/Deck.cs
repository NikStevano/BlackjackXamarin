using System;
namespace Blackjack
{
    /// <summary>
    /// Implements single deck object containing 52 card objects
    /// </summary>
    public class Deck
    {
        // Initial deck-laration, 52 cards because no jokers
        private Card[] Cards = new Card[52];

        public Deck()
        {
            InitDeck();
        }

        public Card[] GetCards()
        {
            return Cards;
        }

        // Generating Deck object, 4 suits with 13 cards each
        private void InitDeck()
        {
            int index = 0;
            foreach (String s in Card.Suits)
            {
                foreach (String v in Card.Values)
                {
                    Cards[index] = new Card(s, v);
                    index++;
                }
            }
        }
    }
}
