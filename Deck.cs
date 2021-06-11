using System;
using System.Collections.Generic;
using System.Net;

namespace CardServer
{

    class Deck
    {
        private List<PlayingCard> _deck = new List<PlayingCard>();
        private char[] cardSuits = { 'H', 'C', 'D', 'S' };  //cSuits
        public string idNumber;
        public IPEndPoint user;
        public static int decksCreated;
        public static int cardsDealt;

        public Deck(IPEndPoint user)
        {
            this.idNumber = GenerateID();
            this.user = user;
            InitDeck();
            decksCreated++;
        }

        private void InitDeck()
        {
            foreach (var cardSuit in cardSuits)
            {
                for (int i = 1; i < 14; i++)
                {
                    var card = new PlayingCard(cardSuit, i);
                    _deck.Add(card);
                }
            }
        }

        public int CardCount()
        {
            return _deck.Count;
        }

        public PlayingCard DrawCard()
        {
            if (_deck.Count == 0)
                throw new Exception("No more cards!");

            var draw = _deck[0];
            _deck.RemoveAt(0);
            cardsDealt++;

            return draw;
        }

        public void ShuffleDeck()
        {
            _deck.Shuffle();
        }

        public void Reset()
        {
            _deck.Clear();
            InitDeck();
        }

        public string GetID()
        {
            return idNumber;
        }

        string GenerateChars(int min, int max, int num, char[] specials, bool withSpecials)
        {
            string returnAnswer = null;
            Random idChar = new Random();
            for (int i = 0; i < num; i++)
            {
                if (!withSpecials)
                {
                    int number = idChar.Next(min, max);
                    returnAnswer += ((char)number).ToString();
                }
                else
                {
                    int number = idChar.Next(0, specials.Length - 1);
                    returnAnswer += specials[number].ToString();
                }
            }

            return returnAnswer;
        }
        string GenerateID()
        {
            char[] specials = { '!', '@', '#', '%', '&', '/', '(', ')', '=', '?', '+', '-', '_', '*', ':', ';' };

            string myChars = GenerateChars(65, 90, 5, specials, false);
            myChars += GenerateChars(97, 122, 5, specials, false);
            myChars += GenerateChars(48, 57, 5, specials, false);
            myChars += GenerateChars(1, 1, 5, specials, true);

            return myChars.Shuffle();
        }
    }
}