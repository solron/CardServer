using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CardServer
{
    class Program
    {
        public static int IndexFinder(string argument, List<Deck> activeDeck)
        {
            int index = activeDeck.FindIndex(
                delegate (Deck deck)
                {
                    return deck.idNumber.Equals(argument);
                }
            );
            if (index == -1)
            {
                System.Console.WriteLine($"Deck {argument} not found!");
            }

            return index;
        }
        public static void SendUDP(string message, IPEndPoint destination, Socket s)
        {
            byte[] sendbuf = Encoding.ASCII.GetBytes(message);
            s.SendTo(sendbuf, destination);
        }

        static void Main(string[] args)
        {
            PlayingCard card = new PlayingCard();
            List<Deck> ActiveDecks = new List<Deck>();
            IPEndPoint remoteDestination;

            // UDP Server
            const int serverPort = 10000;
            UdpClient listener = new UdpClient(serverPort);
            IPEndPoint ipGroup = new IPEndPoint(IPAddress.Any, serverPort);
            bool run = true;

            // UDP Transmitter
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // Print out info
            Console.WriteLine("Card Server running on UDP port " + serverPort);

            while (run)
            {
                byte[] bytes = listener.Receive(ref ipGroup);
                string received = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                System.Console.Write(ipGroup + ": " + received + ": ");

                // Split input string into array of strings to be able to use arguments
                string[] commands = received.Split(' ');

                // Draw a card from the specified card deck
                if (commands[0] == "draw")
                {
                    if (commands.Length > 1)
                    {
                        int index = IndexFinder(commands[1], ActiveDecks);
                        remoteDestination = ActiveDecks[index].user;
                        if (index != -1 && ActiveDecks[index].CardCount() > 0)
                        {
                            card = ActiveDecks[index].DrawCard();
                            System.Console.WriteLine($"Draw from {ActiveDecks[index].idNumber} : {card.Get()}");
                            //remoteDestination = ActiveDecks[index].user;
                            SendUDP(card.Get(), remoteDestination, s);
                        }
                        else 
                        {
                            SendUDP("-1", remoteDestination, s);
                            System.Console.WriteLine($"Draw from {ActiveDecks[index].idNumber} : Deck is empty");
                        }
                    }
                }

                // Return the number of card left in the deck
                if (commands[0] == "count")
                {
                    if (commands.Length > 1)
                    {
                        int index = IndexFinder(commands[1], ActiveDecks);
                        if (index != -1)
                        {
                            int numberOfCards = ActiveDecks[index].CardCount();
                            remoteDestination = ActiveDecks[index].user;
                            SendUDP(card.Get(), remoteDestination, s);
                            System.Console.WriteLine($"Cards left in {ActiveDecks[index].idNumber}: " + numberOfCards);
                        }
                    }
                }

                // Delete specified card deck
                if (commands[0] == "delete")
                {
                    if (commands.Length > 1)
                    {
                        int index = IndexFinder(commands[1], ActiveDecks);
                        if (index != -1)
                        {
                            System.Console.WriteLine($"Deck {ActiveDecks[index].idNumber} is deleted...");
                            ActiveDecks.RemoveAt(index);
                            remoteDestination = ActiveDecks[index].user;
                            SendUDP("OK", remoteDestination, s);
                        }
                    }
                }

                // Shuffle the card deck
                if (commands[0] == "shuffle")
                {
                    if (commands.Length > 1)
                    {
                        int index = IndexFinder(commands[1], ActiveDecks);
                        if (index != -1)
                        {
                            ActiveDecks[index].ShuffleDeck();
                            System.Console.WriteLine($"Deck {ActiveDecks[index].idNumber} is shuffled");
                            remoteDestination = ActiveDecks[index].user;
                            SendUDP("OK", remoteDestination, s);
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Parameter is missing: No where to send the reply");
                    }
                }

                if (commands[0] == "reset")
                {
                    if (commands.Length > 1)
                    {
                        int index = IndexFinder(commands[1], ActiveDecks);
                        if (index != 1)
                        {
                            ActiveDecks[index].Reset();
                            System.Console.WriteLine($"Deck {ActiveDecks[index].idNumber} is reset");
                            remoteDestination = ActiveDecks[index].user;
                            SendUDP("OK", remoteDestination, s);
                        }
                    }
                }

                if (commands[0] == "create")
                {
                    if (commands.Length > 1)
                    {
                        int port = 0;
                        Int32.TryParse(commands[1], out port);
                        ipGroup.Port = port;
                        remoteDestination = new IPEndPoint(ipGroup.Address, port);
                        Deck newDeck = new Deck(remoteDestination);
                        ActiveDecks.Add(newDeck);
                        SendUDP(newDeck.GetID().ToString(), remoteDestination, s);
                        System.Console.WriteLine("ID: " + newDeck.GetID().ToString());
                    }
                }

                // List all active card decks if it less than 21 decks, else it will just print out the active deck count
                if (received == "active")
                {
                    SendUDP(ActiveDecks.Count.ToString(), ipGroup, s);
                    System.Console.WriteLine("Active Decks: " + ActiveDecks.Count.ToString());
                    if (ActiveDecks.Count < 21)
                    {
                        System.Console.WriteLine("ID - Card Count - User");
                        foreach (var item in ActiveDecks)
                        {
                            System.Console.Write(item.GetID() + " - ");
                            System.Console.Write(item.CardCount() + " - ");
                            System.Console.WriteLine(item.user.ToString());
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("To many active decks to list...");
                    }
                }

                if (received == "stats")
                {
                    System.Console.WriteLine("Cards dealt: " + Deck.cardsDealt);
                    System.Console.WriteLine("Decks created: " + Deck.decksCreated);
                    SendUDP(Deck.cardsDealt + "," + Deck.decksCreated, ipGroup, s);
                }

                // Stops the server.
                if (received == "quit")
                {
                    SendUDP("Server shutting down", ipGroup, s);
                    run = false;
                }
            }
        }
    }
}
