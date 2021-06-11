# Card Server 

Card server for various card games. The server will handle the deck, and it is up to the connected clients to use the cards how they like.<br>

The purpose for this server is to simulate card decks for card games. Most games would be better of to include this directly in the games unless you are running some sort of online poker room.<br>

A simple command line version of BlackJack is included to show how to use the server. Check the BlackJack Folder.<br>

The CardServer is MIT license. Check license.txt for license.

## Description

The server create a card deck when the create command is received. The create command require a parameter, the port number it should reply on. The cards in the deck is by default sorted in order. When a deck is created the server reply with the card deck ID. A string with 20 characters. This ID is needed for commands to draw cards, shuffle, reset and delete the deck.<br>

Format of the cards
```
H1 - Ace of hearts
H2 - two of hearts
H11 - Jack of hearts
H12 - Queen of hearts
H13 - King of hearts
S1 - Ace of spades
C1 - Ace of clubs
D1 - Ace of diamond
```

Example on creating a deck.
```
create 62971
reply from server: oMg#7!ox!CTGl65C87+@
```

Drawing a card (replies with Heart Ace)
```
draw oMg#7!ox!CTGl65C87+@
reply from server: H1
```

Shuffle the deck (replies with OK)
```
shuffle oMg#7!ox!CTGl65C87+@
reply from server: OK
```

You shuffle the deck by sending the shuffle command with the ID as argument.

The CardServer uses UDP to communicate with card clients. Every command available returns a response. So you are sure the packets have arived.<br>

## Demo

https://www.youtube.com/watch?v=PYqvcxhroXQ

[![Demo Video](https://img.youtube.com/vi/PYqvcxhroXQ/0.jpg)](https://www.youtube.com/watch?v=PYqvcxhroXQ)

## Bugs

No known bugs at the moment

## TODO

Create a better readme file<br><br>

Check if the ID is used in another active deck. Can use the IndexFinder function to do this.<br>
9,536743164062498e33 possible IDs, so the chance is small. What will happend if it create two decks with same ID. The server will use the first one.<br>

## Cards 

Deck contains
Suits = Spades, Diamonds, Hearts, Clubs<br>
Values = 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, J, Q, K, A

## Functions / Server commands

create port - Return a deck id number<br>
shuffle id - Return success or fail<br>
draw id - Return a array of cards? Or draw one at the time?<br>
count id - Return cards left in the deck.<br>
delete id - Delete the deck.<br>
reset id - Reset the deck.<br>
stats - Stats about decks and cards.<br>
active - List active decks.<br>

stats:
 - Decks created
 - Card dealt

## PlayingCard Class

Properties:<br>
 - private char suit
 - private int value

Methods:<br>
 - void PlayingCard(char, int)
 - void PlayingCard()
 - void Print()
 - void Set(char, int)

## Deck Class

Properties:<br>
 - private List<PlayingCard> _deck
 - private char[] cardValues
 - public int idNumber
 - public int decksCreated
 - public int cardsDealt

Methods:<br>
 - void Deck()
 - void InitDeck()
 - int CardCount()
 - PlayingCard DrawCard()
 - void ShuffleDeck()
 - void Reset()

## Snippets

Initialize a card and a deck
```
PlayingCard card = new PlayingCard();
Deck myDeck = new Deck();
```

How to shuffle the deck
```
myDeck.ShuffleDeck();
```

How to draw all the card until the deck is empty
```
while(myDeck.CardCount() > 0)
{
    int remainingCards = myDeck.CardCount();
    card = myDeck.DrawCard();
    Console.Write(remainingCards + ": ");
    card.Print();
}
```

How to find the right deck in the list
```
// Return -1 if deck is not found
int index = ActiveDecks.FindIndex(
    delegate(Deck deck)
    {
        return deck.idNumber.Equals(deckNumber);
    }
);
```
