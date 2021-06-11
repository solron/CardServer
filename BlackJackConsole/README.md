# BlackJack Console Version

A simple version of Blackjack to test out the card server.

## Simplified Rules

- The player receive two cards.
- The house receive two cards.
- The player plays first.
- Hit new cards until the player stops or goes above 21. If above 21 the player is bust and the house wins.
- The house plays.
- Suits doesn't matter.
- Ace can be 1 or 11.
- First Ace house gets is 11 unless it busts.
- All picture cards is 10 (jack, queen and king).

## Before the game loop

- Ask the server for a deck.
- Shuffle the deck.

## Game loop

- Deal two cards to player (Dont show).
- Deal two cards to house (Dont show. In reality the cards can be drawn when its the house turn to play)
- Show the player cards.
- Ask the player to (H) hit or (S) stay.
- Hit will give another card, and ask if the player wants another card. Continue until the player press S for Stay. Or if player goes above 21, in that case the house wins.
- Standard rules for the hoouse.
  - Show the dealer cards (or draw two cards for the dealer).
  - If the dealer has 17 or more he stays.
  - If the dealer has 16 or less he hits new cards until he get 17 or more.
  - When the dealer stands or goes bust the round is over.
- If the dealer busts the player win.
- Compare the score. The one who get 21 or closest to 21 wins. Above 21 loose. But that should be determined earlier in the loop.
