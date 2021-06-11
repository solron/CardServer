using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace BlackJackConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Variables
            bool dealerIsBust = false;
            bool playerIsBust = false;
            bool gameOver = false;
            int playerValueCards = 0;
            int dealerValueCards = 0;
            string deckID;          // Deck id 
            string returnMessage;   // Return message from card server
            List<string> playerCards = new List<string>();   // Player cards
            List<string> dealerCards = new List<string>();    // Dealer cards

            // Welcome
            Console.WriteLine("Welcome to Blackjack for CLI");

            // Create UDP socket and destination ip/port
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            int destPort = 10000;
            string destIP = "192.168.2.170";
            IPEndPoint destination = new IPEndPoint(IPAddress.Parse(destIP), destPort);

            // Create UDP listener
            const int serverPort = 5000;
            UdpClient listener = new UdpClient(serverPort);
            IPEndPoint ipGroup = new IPEndPoint(IPAddress.Any, serverPort);

            System.Console.WriteLine("UDP listening port created on: " + serverPort);

            // G A M E    L O O P
            do
            {
                // Create Deck
                deckID = SendCommand("create " + serverPort, destination, s, ipGroup, listener);

                // Shuffle the deck
                returnMessage = SendCommand("shuffle " + deckID, destination, s, ipGroup, listener);
                if (returnMessage == "OK")
                    System.Console.WriteLine("Deck " + deckID + " shuffled");

                // Give two cards to player
                returnMessage = SendCommand("draw " + deckID, destination, s, ipGroup, listener);
                playerCards.Add(returnMessage);
                returnMessage = SendCommand("draw " + deckID, destination, s, ipGroup, listener);
                playerCards.Add(returnMessage);

                // Print out player cards
                System.Console.WriteLine("- - - - - - - - - - - - - - - - - -");
                foreach (var item in playerCards)
                {
                    System.Console.Write(item + " ");
                }
                playerValueCards = TotalValue(playerCards);
                System.Console.WriteLine("  Total = " + playerValueCards);
                // Write total card value
                System.Console.WriteLine();

                // P L A Y E R   L O O P
                // Ask for more player cards and calculate total
                bool askAgain = true;
                System.Console.WriteLine("(H)it or (S)tay ");
                do
                {
                    string input = Console.ReadLine();
                    if (input.ToLower() == "h")
                    {
                        returnMessage = SendCommand("draw " + deckID, destination, s, ipGroup, listener);
                        playerCards.Add(returnMessage);
                        System.Console.Write("Player: ");
                        foreach (var item in playerCards)
                        {
                            System.Console.Write(item + " ");
                        }
                        playerValueCards = TotalValue(playerCards);
                        if (playerValueCards > 21)
                        {
                            askAgain = false;
                            playerIsBust = true;
                        }
                        System.Console.WriteLine("  Total = " + playerValueCards);
                    }
                    if (input.ToLower() == "s")
                        askAgain = false;
                } while (askAgain);

                // D E A L E R   L O O P
                // Show dealers card and dealers card logic unless player is bust
                // Draw two cards for the dealer
                returnMessage = SendCommand("draw " + deckID, destination, s, ipGroup, listener);
                dealerCards.Add(returnMessage);
                returnMessage = SendCommand("draw " + deckID, destination, s, ipGroup, listener);
                dealerCards.Add(returnMessage);

                askAgain = true;
                while (askAgain && playerValueCards < 22)
                {
                    System.Console.Write("Dealer: ");
                    foreach (var item in dealerCards)
                    {
                        System.Console.Write(item + " ");
                    }
                    dealerValueCards = TotalValue(dealerCards);
                    System.Console.WriteLine("  Total = " + dealerValueCards);

                    if (dealerValueCards < 16)
                    {
                        returnMessage = SendCommand("draw " + deckID, destination, s, ipGroup, listener);
                        dealerCards.Add(returnMessage);
                    }
                    else if (dealerValueCards > 21)
                    {
                        dealerIsBust = true;
                        askAgain = false;
                    }
                    else
                    {
                        askAgain = false;
                    }
                }

                // Check for winner
                if (playerIsBust || dealerIsBust)
                {
                    if (playerIsBust)
                        System.Console.WriteLine("Player is bust, Dealer wins!!!!");
                    if (dealerIsBust)
                        System.Console.WriteLine("Dealer is bust, players wins!!!!");
                }
                else
                {
                    if (playerValueCards > dealerValueCards)
                        System.Console.WriteLine("Player beats dealer");
                    else if (playerValueCards == dealerValueCards)
                        System.Console.WriteLine("ITS A DRAW!!");
                    else
                        System.Console.WriteLine("Dealer beats player");
                }

                // NEW GAME?
                System.Console.WriteLine();
                System.Console.Write("Another game? (Y)es or (N)o: ");
                string newGame = Console.ReadLine();
                if (newGame.ToLower() == "y")
                    gameOver = false;
                if (newGame.ToLower() == "n")
                    gameOver = true;
                // Clear the cards
                playerCards.Clear();
                playerValueCards = 0;
                playerIsBust = false;
                dealerCards.Clear();
                dealerValueCards = 0;
                dealerIsBust = false;

            } while (!gameOver);
        }
        static string SendCommand(string command, IPEndPoint destination, Socket s, IPEndPoint ipGroup, UdpClient listener)
        {
            // Send the UDP Packet
            byte[] sendbuf = Encoding.ASCII.GetBytes(command);
            s.SendTo(sendbuf, destination);

            // Receive UDP Packet
            byte[] bytes = listener.Receive(ref ipGroup);
            string received = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

            return received;
        }

        static int TotalValue(List<string> cards)
        {
            int total = 0;
            foreach (var item in cards)
            {   // Remove first char which is the suit from the value and add them into total
                int addValue = Int32.Parse(item.Remove(0, 1));
                if (addValue > 10)      // If picture card the value is 10
                    addValue = 10;
                if (addValue == 1)
                {    // Check if the card is ace
                    if (total > 10)     // If you are going bust with 11, give the ace the value of 1
                        addValue = 1;
                    else
                        addValue = 11;
                }
                total += addValue;      // Add total
            }

            return total;
        }
    }
}
