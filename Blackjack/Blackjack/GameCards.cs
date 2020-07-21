using System;
using System.Collections.Generic;

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

        public GameCards(int numberOfDecks)
        {
            this.numberOfDecks = numberOfDecks;
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
