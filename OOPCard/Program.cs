using System;
using System.Runtime.InteropServices.JavaScript;

class Program {
    public static void Main (string[] args) 
    {  
        // Deck deck = new Deck();
        // deck.ShuffleDeck();
        // deck.PrintDeck();
        // Hand hand1 = new Hand(deck);
        // Hand hand2 = new Hand(deck);
        // Player normalPlayer = new Player(hand1, "Honest Hugo");
        // Cheater cheater = new Cheater(hand2, "Conman Connor");
        // cheater.DrawSleeveCard(11,1);
        // Console.WriteLine(cheater.RevealHand());
        // Console.WriteLine(normalPlayer.RevealHand());

        string[] playerNames = new string[3];
        playerNames[0] = "Honest Hugo";
        playerNames[1] = "Gambler Gale";
        playerNames[2] = "Some Dude";
        Console.WriteLine("What would you like to play"+"\n 1: BlackJack"+"\n 2: Texas Hold'em");
        int response = Convert.ToInt32(Console.ReadLine());
        if (response==1){
            BlackJack blackJack = new BlackJack(playerNames);
            blackJack.RunGame();
            blackJack.RestartGame();
        }
        else if (response == 2)
        {
            TexasHoldem poker = new TexasHoldem(playerNames);
            Hand someHand = new Hand(new Deck());
            Card[] rFlush = new Card[7];
            rFlush[0] = new Card(10, 1);
            rFlush[1] = new Card(11, 1);
            rFlush[2] = new Card(12, 1);
            rFlush[3] = new Card(13, 1);
            rFlush[4] = new Card(1, 1);
            rFlush[5] = someHand.GetHand()[0];
            rFlush[6] = someHand.GetHand()[1];
            
            Console.WriteLine(poker.CheckStraightFlush(rFlush)[0]);
            poker.RunGame();
            poker.RestartGame();
            
        }
    }
}

class Deck {
    private Card[] deck;
    private HashSet<Card> used = new HashSet<Card>();
    // constructor 
    public Deck()
    {
        deck = new Card[52];
        // lets generate all 52 cards and add them to the deck
        int counter = 0;

        // for all suits 0 - 3
        for (int i = 0; i < 4; i ++) {
            // for all card numbers 1 - 13
            for (int j = 1; j <= 13; j++) {
                Card c = new Card(j, i); // number, suit
                deck[counter] = c; // add card to deck
                counter++;
            }
        }
    }

    public void PrintDeck() {
        foreach (Card c in deck) {
            Console.WriteLine(c.GetName());
        }
    }

    public void ShuffleDeck()
    {
        Random shuffler= new Random();
        int index = deck.Length;
        while (index > 1)
        {
            index -= 1;
            int newIndex = shuffler.Next(index + 1);
            Card temp = deck[index];
            deck[index] = deck[newIndex];
            deck[newIndex] = temp;
        }
    }
    public Card TakeCard()
    {
        if (used.Count == 52)
        {
            //essentially, if there are no cards left, return nothing
            return null;
        }

        int index = 0;
        while (used.Contains(deck[index]))
        {
            index += 1;
        }
        //once a card is found, chuck it into used
        used.Add(deck[index]);
        return deck[index];
    }

    public Card[] CheckUsed()
    {
        Card[] inUse = new Card[52];
        //for somewhat obvious reasons, if the set is over 52 cards large, something has gone horribly wrong
        used.CopyTo(inUse);
        return inUse;
    }
}
class Card {
    private int number;
    private int suit;
	
    // constructor
    public Card(int newNum, int newSuit) {
        this.number = newNum;
        this.suit = newSuit;
    }

    public string GetName() {
        string suitString;
        string numberString;

        switch (suit) {
            case 0:
                suitString = "Diamonds";
                break;
            case 1:
                suitString = "Clubs";
                break;
            case 2:
                suitString = "Hearts";
                break;
            case 3:
                suitString = "Spades";
                break;
            default:
                suitString = "Error";
                break;
        }


        switch (number) {
            case 11:
                numberString = "Jack";
                break;
            case 12:
                numberString = "Queen";
                break;
            case 13:
                numberString = "King";
                break;
            case 1:
                numberString = "Ace";
                break;
            default:
                numberString = ""+number;
                break;
        }

        return numberString + " of " + suitString;
    }

    public int GetScore()
    {
        //this is for blackjack
        if (number > 10)
        {
            //Jack, Queen and King are worth 10
            return 10;
        }
        else if (number == 1)
        {
            //Ace can either be 11 or 1
            return 11;
        }
        else
        {
            return number;
        }
    }

    public int GetSuit()
    {
        return suit;
    }

    public int GetNumber()
    {
        return number;
    }
}
class Hand
{
    private List<Card> hand;
    public Hand(Deck deck)
    {
        hand = new List<Card>();
        for (int count=0; count<2; count++)
        {
            Card card = deck.TakeCard();
            
            hand.Add(card);
        }
    }

    public void PrintHand()
    {
        for (int index = 0; index < hand.Count; index++)
        {
            Console.WriteLine(hand[index].GetName());
        }
    }

    public void AddCard(Card newCard)
    {
        hand.Add(newCard);
    }

    public List<Card> GetHand()
    {
        return hand;
    }
    public int[] HandScore()
    {
        int total = 0;
        //due to how Ace can have multiple values, I will store the number of aces
        int numAces = 0;
        for (int index = 0; index < hand.Count; index++)
        {
            int score = hand[index].GetScore();
            if (score == 11)
            {
                numAces += 1;
            }
            total += score;
        }

        int[] packet = new int[2];
        packet[0] = total;
        packet[1] = numAces;
        return packet;
    }
}
class Player
{
    private Hand hand;
    private string name;
    
    public Player(Hand hand, string playerName)
    {
        this.hand = hand;
        name = playerName;
    }

    public Hand GetHand()
    {
        return hand;
    }

    public void DrawCard(Deck deck)
    {
        hand.AddCard(deck.TakeCard());
    }

    public void ResetHand(Hand hand)
    {
        this.hand = hand;
    }

    public void SetName(string newName)
    {
        name = newName;
    }

    public string GetName()
    {
        return name;
    }

    public int RevealBlackjackHand()
    {
        int total = hand.HandScore()[0];
        int numAces = hand.HandScore()[1];
        while (numAces > 0 && total > 21)
        {
            //converting an ace from 11 to 1
            total -= 10;
            numAces -= 1;
        }
        return total;
    }
}
class Cheater : Player
{
    public Cheater(Hand hand, string playerName) : base(hand, playerName)
    {
        
    }

    public void DrawSleeveCard(int num, int suit)
    {
        Hand newHand = this.GetHand();
        newHand.AddCard(new Card(num, suit));
        this.ResetHand(newHand);
    }
}
abstract class Game
{
    private List<Player> players;
    protected Deck deck;
    public virtual void RestartGame()
    {
        deck = new Deck();
        deck.ShuffleDeck();
        for (int index = 0; index < players.Count; index++)
        {
            players[index].ResetHand(new Hand(deck));
        }
    }
    public void AddPlayer(Player newPlayer)
    {
        players.Add(newPlayer);
    }

    public Deck GetDeck()
    {
        return deck;
    }

    public List<Player> GetPlayers()
    {
        return players;
    }
    
    public Game(string[] playerNames, bool burn)
    {
        
        players = new List<Player>();
        deck = new Deck();
        deck.ShuffleDeck();
        if(burn){
            deck.TakeCard();
        }
        for (int index = 0; index < playerNames.Length; index++)
        {
            Hand hand = new Hand(this.GetDeck());
            this.AddPlayer(new Player(hand,playerNames[index]));
        }
    }
}
class BlackJack : Game
{
    public BlackJack(string[] playerNames) : base(playerNames,false)
    {
        
    }

    public void RunGame()
    {
        //this represents the number of players who stood that round
        int numReady=0;
        int totalPlayers = this.GetPlayers().Count;
        while (numReady < totalPlayers)
        {
            //reset numReady
            numReady = 0;
            for (int index = 0; index < totalPlayers; index++)
            {
                Player player = this.GetPlayers()[index];
                string name = player.GetName();
                int score = player.RevealBlackjackHand();
                Console.WriteLine(name+". Your hand is "+ score);
                if (score<21){
                    Console.WriteLine("Do you want to Hit or Stand" + "\n False: Stand" + "\n True: Hit");
                    bool hit = Convert.ToBoolean(Console.ReadLine());
                    if (hit)
                    {
                        //the player draws another card
                        player.DrawCard(this.GetDeck());
                        //save player
                        this.GetPlayers()[index] = player;
                    }
                    else
                    {
                        numReady += 1;
                    }
                }
                else
                {
                    //player cannot do anything
                    Console.WriteLine("You have busted");
                }
            }
        }
        //game has ended 
        int closestScore = 0;
        string winningPlayer = "";
        for (int index = 0; index < totalPlayers; index++)
        {
            Player player = this.GetPlayers()[index];
            int score = player.RevealBlackjackHand();
            if (score <= 21 && score > closestScore)
            {
                closestScore = score;
                winningPlayer = player.GetName();
            }
        }
        Console.WriteLine(winningPlayer+" has won with a score of "+closestScore);
    }
}

class TexasHoldem : Game
{
    private Card[] flop;
    private int turns;
    public TexasHoldem(string[] playerNames) : base (playerNames,true)
    {
        flop = new Card[5];
        turns = 1;
    }
    public void RunGame()
    {
        int numFold = 0;
        int numPlayers = this.GetPlayers().Count;
        bool[] folded = new bool[numPlayers];
        //burning a card
        this.GetDeck().TakeCard();
        flop[0] = this.GetDeck().TakeCard();
        flop[1] = this.GetDeck().TakeCard();
        flop[2] = this.GetDeck().TakeCard();
        for (int index = 0; index < numPlayers; index++)
        {
            Console.WriteLine(this.GetPlayers()[index].GetName()+", this is your hand");
            this.GetPlayers()[index].GetHand().PrintHand();
            Console.WriteLine("What would you like to do"+"\n 1: Call/ Continue playing"+"\n 2: Fold/ End the match");
            //normally, there would be the option to either raise or call the bet, but that will be implemented later
            int answer=Convert.ToInt32(Console.ReadLine());
            if (answer == 2)
            {
                numFold += 1;
                folded[index] = true;
            }
        }
        //match properly starts
        while (turns < 3 && numFold < numPlayers - 1)
        {
            for (int card = 0; card < turns + 2; card++)
            {
                //display the flop
                Console.WriteLine(flop[card].GetName());
            }
            for (int index = 0; index < numPlayers; index++){
                if (folded[index] == true)
                {
                    Console.WriteLine(this.GetPlayers()[index].GetName()+" has folded, skipped");
                    continue;
                }
                Console.WriteLine(this.GetPlayers()[index].GetName() + ", this is your hand");
                this.GetPlayers()[index].GetHand().PrintHand();
                Console.WriteLine("What would you like to do"+"\n 1: Call/ Continue playing"+"\n 2: Fold/ End the match");
                //normally, there would be the option to either raise or call the bet, but that will be implemented later
                int answer=Convert.ToInt32(Console.ReadLine());
                if (answer == 2)
                {
                    numFold += 1;
                    folded[index] = true;
                    //if everybody else has folded, the last person remaining wins by default
                    if (numFold == numPlayers - 1)
                    {
                        break;
                    }
                }
            }
            turns += 1;
            //burn a card
            this.GetDeck().TakeCard();
            flop[turns + 1] = this.GetDeck().TakeCard();
        }

        if (numFold == numPlayers - 1)
        {
            for (int index = 0; index < numPlayers; index++)
            {
                if (folded[index] == false)
                {
                    Console.WriteLine(this.GetPlayers()[index].GetName()+" has won by default");
                }
            }
        }
        else
        {
            //compare the hands
            List<string> draw = new List<string>();
            for (int player = 0; player < numPlayers; player++)
            {
                Hand hand =this.GetPlayers()[player].GetHand();
                // if (CheckRoyalFlush(hand, flop))
                // {
                //     draw.Append(this.GetPlayers()[player].GetName());
                // }
            }

            if (draw.Count ==1)
            {
                Console.WriteLine(draw[0]+" has won with a Royal Flush");
            }
            else if (draw.Count>1)
            {
                //multiple people have a r Flush
                string winString = "";
                for (int index = 0; index < draw.Count-1; index++)
                {
                    winString += draw[index];
                    winString += " and ";
                }
                winString += draw[draw.Count-1];
                Console.WriteLine(winString + " has drawn with a Royal Flush");
            }
            //after this point you reset draw and continue checking the win conditions by order of priority
        }
    }

    public int[] CheckStraightFlush(Card[] combinedHand)
    {
        //takes in the combined Hand of the player and the flop, returns a list of 2 integers. 
        //first integer represents if it is a straight flush, second integer represents the smallest number in the straight
        //flush
        int[] output = new int[2];
        int diamondCount = 0;
        int clubCount = 0;
        int heartCount = 0;
        int spadeCount = 0;
        for (int index = 0; index < 7; index++)
        {
            int suit = combinedHand[index].GetSuit();
            switch (suit)
            {
                case 0:
                    diamondCount += 1;
                    break;
                case 1:
                    clubCount += 1;
                    break;
                case 2:
                    heartCount += 1;
                    break;
                case 3:
                    spadeCount += 1;
                    break;
            }
        }
        List<Card> trimmedHand = new List<Card>();
        if (diamondCount > 4)
        {
            //the suit is in diamond
            trimmedHand = SpecialSort(combinedHand,0);
        }
        else if (clubCount > 4)
        {
            //the suit is in clubs
            trimmedHand = SpecialSort(combinedHand, 1);
        }
        else if (heartCount > 4)
        {
            //the suit is in hearts
           trimmedHand = SpecialSort(combinedHand, 2);
        }
        else if (spadeCount > 4)
        {
            //the suit is in spades
            trimmedHand = SpecialSort(combinedHand, 3);
        }
        else
        {
            //there is no flush
            output[0] = 0;
            return output;
        }

        if (CheckContinuous(trimmedHand))
        {
            output[0] = 0;
            output[1] = trimmedHand[0].GetNumber();
        }
        return output;
    }

    private List<Card> SpecialSort(Card[] hand, int suit)
    {
        List<Card> trimmedHand = new List<Card>();
        for (int index = 0; index < 7; index++)
        {
            if (hand[index].GetSuit() == suit)
            {
                //only add cards that could be part of a straight flush
                trimmedHand.Add(hand[index]);
            }
        }

        for (int index = 0; index < trimmedHand.Count; index++)
        {
            //insertion sort it
            Card checking = trimmedHand[index];
            int value = checking.GetNumber();
            int position = index;
            while (position > 0 && value < trimmedHand[position - 1].GetNumber())
            {
                trimmedHand[position] = trimmedHand[position - 1];
                position -= 1;
            }
            trimmedHand[position] = checking;
        }
        return trimmedHand;
    }
    private bool CheckContinuous(List<Card> trimmedHand)
    {
        //this breaks if there is an ace in the deck
        //should probably make a new Poker Card
        int previousValue = trimmedHand[0].GetNumber();
        for (int index = 0; index < trimmedHand.Count; index++)
        {
            if ((trimmedHand[index].GetNumber() - previousValue) > 1)
            {
                //it is not a straight flush
                return false;
            }
        }
        return true;
    }
    public override void RestartGame()
    {
        deck = new Deck();
        deck.ShuffleDeck();
        flop = new Card[5];
        turns = 1;
        //burn a card
        this.GetDeck().TakeCard();
        for (int index = 0; index < this.GetPlayers().Count; index++)
        {
            Hand hand = new Hand(deck);
            this.GetPlayers()[index].ResetHand(hand);
        }
    }
}
