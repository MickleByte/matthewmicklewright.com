/*
 *          Going to Boston Dice Game
 *          5th December 2017
 *          Matthew Micklewright
 *         
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication6
{
    class Game
    {
        static void Main(string[] args)//deals with main menu
        {

            int iMenuInput = 0;
            while (iMenuInput != 9)//while the user does not enter the exit option: loop the game
            {
                switch (iMenuInput)
                {
                    case 1:
                        //vs computer
                        PlayGame(true);//passess a true of false value which represents whether it is a singleplayer or 2 player game (true = singleplayer)
                        break;
                    case 2:
                        //2 player
                        PlayGame(false);
                        break;
                    default:
                        break;

                }
                //output menu
                Console.Clear();
                Console.WriteLine("1. 1 Player (vs Computer) ");
                Console.WriteLine("2. 2 Player");
                Console.WriteLine("9. Exit");

                iMenuInput = GetMenuInput();//get user input via function GetMenuInput which validates
            }



        }

        static void PlayGame(bool bComp)//sets up the game and gets the input for the match play or score play option
        {
            Console.Clear();
            dice GameDie = new dice();//creates the dice object that is used in the game
            Player Player1 = new Player();//creates the player objects
            Player Player2 = new Player();
            if (bComp)//if the bool comp is passed as true then the user wants singleplayer therefore the computer attribute of the player2 object must be set to true
            {
                Player2.computer = true;
                Console.WriteLine("Single Player:");
            }
            else
            {
                Console.WriteLine("Two Player:");              
            } 
            Console.WriteLine("1. Match Play");//outputs menu options
            Console.WriteLine("2. Score Play");
            Console.WriteLine("9. Return to main menu");
            int iMenuInput = GetMenuInput();//get input via validation function
            switch (iMenuInput)
            {
                case 1:
                    Console.Clear();
                    MatchPlay(Player1, Player2, GameDie);
                    break;
                case 2:
                    Console.Clear();
                    ScorePlay(Player1, Player2, GameDie);
                    break;
                default:
                    Console.WriteLine("Error");
                    break;
            }

        }

        static int GetMenuInput()//Validate menu inputs (inputs must be ints 1, 2 or 9)
        {
            string Sline = Console.ReadLine();//read input and store in line
            int iValue;
            if (int.TryParse(Sline, out iValue))//if the line is an int convert it to int value else skip
            {
                if ((iValue == 1) || (iValue == 2) || (iValue == 9))//if the int value is one of the possible options
                {
                    return iValue;//if all criteria are correct return the input
                }

            }                       
            Console.WriteLine("Not a valid input, try again");//if the input was not valid returns this error message
            return GetMenuInput();//recursivley calls itself (keeps looping until it does get a correct input)

        }


        //Turn() was modified to use a list instead of a temporary int value - 6/12/17 by M Micklewright
        static private int Turn(dice GameDie, int NumberOfDice)//runs a turn (used in both match and score play) - rolls dice required number of times and outputs result to console then returns highest dice value
        { 
            List<int> rolls = new List<int>();
           
            for (int i = 0; i < NumberOfDice; i++)//rolls 3, 2 or 1 times depending on what is passed
            {
                rolls.Add(GameDie.roll()); //uses the roll function from the GameDie object to randomly generate a number 1-6 , this is addedd to the list of rolls from this turn
                Console.Write("{0}   ", rolls.Last<int>());//output value of roll most recent roll (last roll in list)                
            }
            Console.WriteLine();

            return rolls.Max<int>(); //returns the highest roll of the turn
        }


        //Score Play
        static void ScorePlay(Player Player1, Player Player2, dice GameDie)//governs the score play mode
        {

            for (int i = 1; i < 6; i++)//plays through 5 rounds
            {
                Console.WriteLine("Round {0}:", i);
                Console.ReadKey();
                Console.WriteLine();
                RoundScore(GameDie, Player1, Player2);//plays the round
                Console.WriteLine();

                Console.WriteLine("Player 1's total is now {0}", Player1.Score);//gives the total scores for player 1 and 2 (or computer if in single player)

                if (!Player2.computer)
                {
                    Console.WriteLine("Player 2's total is now {0}", Player2.Score);
                }
                else
                {
                    Console.WriteLine("The Computer's total is now {0}", Player2.Score);
                }
                Console.ReadKey();
                Console.Clear();
            }
            ScorePlayEnd(Player1, Player2);//after playing 5 rounds goes to end subroutine which decides winner etc.

        }

        static void RoundScore(dice GameDie, Player Player1, Player Player2)//governs a round of score play
        {
            int iP1Total = 0;
            int iP2Total = 0;
            for (int i = 3; i > 0; i--)//takes 3 turns with 3 dice, then 2 then 1, each time accumulating the highest value rolled in p1Total
            {
                Console.WriteLine("Player 1 rolls...");
                iP1Total += Turn(GameDie, i);
            }
            Console.WriteLine("Player 1 Scored {0}", iP1Total);//outputs player 1's score for that round
            Console.ReadKey();
            Console.WriteLine();
            for (int i = 3; i > 0; i--)//takes 3 turns with 3 dice, then 2 then 1, each time accumulating the highest value rolled in p2Total
            {
                if (!Player2.computer)//writes alternative messages for single and two player
                {
                    Console.WriteLine("Player 2 rolls...");
                }
                else
                {
                    Console.WriteLine("Computer rolls...");
                }

                iP2Total += Turn(GameDie, i);
            }
            if (!Player2.computer)//writes alternative messages for single and two player
            {
                Console.WriteLine("Player 2 Scored {0}", iP2Total);
            }
            else {
                Console.WriteLine("The computer Scored {0}", iP2Total);
            }
            
            Player1.Score += iP1Total;//adds scores from that round to player totals
            Player2.Score += iP2Total;
        }

        static void ScorePlayEnd(Player Player1, Player Player2)//decides winner of score play
        {
            if (Player1.Score == Player2.Score)//if bothed scored the same it's a draw
            {
                Console.WriteLine("Draw");

            }
            else if (Player1.Score > Player2.Score)//if player 1 scores most they win
            {
                Console.WriteLine("Player 1 Wins");

            }
            else//if player2 scores most they win
            {
                if (Player2.computer)//if in singleplayer outputs that computer has won instead
                {
                    Console.WriteLine("Computer Wins");
                }
                else
                {
                    Console.WriteLine("Player 2 Wins");
                }


            }
            Console.ReadKey();
        }


        //Match Play
        static void MatchPlay(Player Player1, Player Player2, dice GameDie)//governs the match play mode
        {
            int iRoundCounter = 1;
            while ((Player1.Score < 5) && (Player2.Score < 5))//loops until one player gets to 5 points
            {
                Console.WriteLine("Match Play");//introductory dialogue to the round
                Console.WriteLine("Round {0}:", iRoundCounter);
                Console.WriteLine();
                Console.ReadKey();
                int RoundResult = RoundMatch(GameDie, Player2.computer);//jumps to RoundMatch which runs a round of the game and returns an int value 1, 2 or 3 (1=player 2 win, 2=player 1 win, 3=draw)
                Console.WriteLine();
                if (RoundResult == 1)//if player 2 won that round
                {
                    if (!Player2.computer)//alternative outputs for singleplayer mode
                    {
                        Console.WriteLine("Player 2 wins round {0}", iRoundCounter);
                    }
                    else
                    {
                        Console.WriteLine("The computer wins round {0}", iRoundCounter);
                    }
                    Player2.Score += 1;//icrease player 2 score by one as they have won that round
                }
                else if (RoundResult == 2)//if player 1 won that round
                {
                    Console.WriteLine("Player 1 wins round {0}", iRoundCounter);//output to user who won
                    Player1.Score += 1;//increase player 1 score by one as they won the round
                }
                else {
                    Console.WriteLine("That round was a draw");//if the result is not 1 or 2 it must be 3 which represents a draw
                }

                Console.WriteLine("Current score:");//Gives an update on the current score
                Console.WriteLine("Player 1: {0}", Player1.Score);
                if (!Player2.computer)//alternative outputs if in singleplayer mode
                {
                    Console.WriteLine("Player 2: {0}", Player2.Score);
                }
                else
                {
                    Console.WriteLine("Computer: {0}", Player2.Score);
                }

                Console.WriteLine();
                iRoundCounter += 1;//increments for the next round
                Console.ReadKey();
                Console.Clear();
            }
            MatchPlayEnd(Player1, Player2);//one player has reached 5 points therefore game is over - jumps to MatchPlayEnd to check winner and output to user
            
        }

        static int RoundMatch(dice GameDie, bool comp)//governs a round of match play and returns int value of 1,2 or 3 (1=player 2 win, 2=player 1 win, 3=draw)
        {
            int iP1Total = 0;//player 1's total score for that round , compared with player 2s at the end of the round to see who won
            int iP2Total = 0;
            for (int i = 3; i > 0; i--) {//loops 3 times for player 1s section of the round - with i (number of dice rolled) decreasing from 3 by 1 each time
                Console.WriteLine("Player 1 rolls...");
                iP1Total += Turn(GameDie, i);//goes to Turn which rolls i number of times and returns the highest dice roll
            }
            Console.WriteLine("Total = {0}", iP1Total);//ouputs player 1's score for that round
            Console.ReadKey();
            Console.WriteLine();
            for (int i = 3; i > 0; i--)//loop 3 times for player 2s section of the round - with i (number of dice rolled) decreasing from 3 by 1 each time
            {
                if (comp)//alternative outputs for singleplayer mode
                {
                    Console.WriteLine("Computer rolls...");
                }
                else {
                    Console.WriteLine("Player 2 rolls...");
                }

                iP2Total += Turn(GameDie, i);//goes to Turn which rolls i number of times and returns the highest dice roll
            }
            Console.WriteLine("Total = {0}", iP2Total);//outputs player 2's total score for that round

            //determines who scored most and therefore won the round and returns 1, 2 or 3 to match play which interprets the ints to mean P2 win, P1 win or draw respectively
            if (iP2Total > iP1Total)//player 2 scores most (P2 Win)
            {
                return 1;
            }
            else if (iP2Total < iP1Total)//player 1 scores most (P1 Win)
            {
                return 2;
            }
            else//both scored the same (Draw)
            {
                return 3;
            }
            
        }

        static void MatchPlayEnd(Player Player1, Player Player2)//checks winner of match play and outputs to user
        {
            Console.WriteLine("The Final Score:");
            Console.WriteLine("P1: {0}", Player1.Score);
            if (!Player2.computer)//alternative output for singleplayer
            {
                Console.WriteLine("P2: {0}", Player2.Score);
            }
            else
            {
                Console.WriteLine("Computer: {0}", Player2.Score);
            }

            if (Player1.Score == 5)//if player 1 won
            {
                Console.WriteLine("Player 1 wins");
            }
            else//alternatively if player 2 won
            {
                if (!Player2.computer)//alternative for singleplayer
                {
                    Console.WriteLine("Player 2 wins"); ;
                }
                else
                {
                    Console.WriteLine("The computer wins"); ;
                }
            }

            Console.ReadKey();
        }
    }


    class Player//template for each player, has attributes : computer (bool) - represents if the player is human or computer (affects ouputs in singleplayer) and score
    {
        public bool computer;//defines whether or not the player is a computer or not (this affects the way things are phrased when outputing to user_
        private int iscore;//score of player
        public int Score
        {
            get
            {
                return this.iscore;
            }
            set
            {
                this.iscore = value;
            }
        }
    }


    class dice//template for dice objects, has the constant attribute sides which represents the upper limit of the random number generation in the function roll() (generates a random number between 1 and number of sides)  
    {    
        private const int sides = 6;//range that the dice goes up to
        Random rnd = new Random();
        public int roll()//returns a random value between 1 and number of sides (inclusive)
        {      
            return (rnd.Next(1, sides + 1));
        }
    }
        
        
    }
