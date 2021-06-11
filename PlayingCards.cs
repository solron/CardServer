using System;

class PlayingCard 
{
    public char suit;
    public int value;

    public PlayingCard(char suit, int value)
    {
        this.suit = suit;
        this.value = value;
    }
    public PlayingCard()
    {
        this.suit = 'C';
        this.value = 1;
    }

    public void Print()
    {
        System.Console.Write(suit);
        System.Console.Write(value);
    }
    public void Set(char c, int i)
    {
        suit = c;
        value = i;
    }

    public string Get() 
    {
        return suit+value.ToString();
    }
}