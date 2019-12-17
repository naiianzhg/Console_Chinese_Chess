﻿using System;
using System.Collections.Generic;
using System.Text;
using ChineseChess.Model;
using ChineseChess.View;

namespace ChineseChess.Control
{
    class PiecesHandler
    {
        // Parse the input string location in int location
        public static int[] parseLocation(string s, out bool isValid)
        {
            string[] strLocation;
            int[] location = new int[2];
            int splitAmount = 0, splitIndex = 0, numAmount = 0;
            isValid = false;

            // Split the input string 'x,y' into two parts x and y and storing into int[] (int[0] = x, int[1] = y)
            // if there is only one none-digit character between two numbers as spliting character, it is valid for format
            // Obtain the (1.)amount of none-digit character, (2.)the index of the last none-digit character and (3.)the amount of numbers
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsDigit(s[i]))
                {
                    splitAmount++;
                    splitIndex = i;
                }
                else numAmount++;
            }

            // The input must has (1.)only one non-digit character, (2.)the character must be surrounded by two numbers
            // if not, it is an informat input
            if (numAmount > 1 && splitAmount == 1 && splitIndex != s.Length - 1 && splitIndex != 0)
            {
                // The player should have entered comma as the spliting character
                // In case of not miss-enter too much times, no matter what spliting char the player use, the program can process
                // Obtain the string array location
                strLocation = s.Split(s[splitIndex]);
                // Convert the string location to int location
                for (int i = 0; i < strLocation.Length; i++)
                {
                    location[i] = Convert.ToInt32(strLocation[i]);
                }

                // Check the validity of the input position, if it is not out of range the board
                // Then the choose input is valid
                if (location[0] < 0 || location[0] > 9) throw new Exception("Row index out of range");
                else if (location[1] < 0 || location[1] > 8) throw new Exception("Column index out of range");
                else isValid = true;
            }
            else throw new Exception("Informat input");

            return location;
        }

        // Receive the input original location (piece location) from the user
        public static void chooseOri()
        {
            int[] chosenOriLocation = new int[2];
            bool isValid = true;
            // In this loop the player will need to input a location until the right one is input
            do
            {
                // Ask the player to choose a piece to move
                DisplayMessage.displayAskChooseMessage();
                try
                {
                    // Convert the string location to integer location
                    // if the parseLocation did not throw any exception, formatly the input is correct
                    chosenOriLocation = parseLocation(Console.ReadLine(), out isValid);
                    // Save this chosen original location as last original location
                    Board.addLastOriLocation(chosenOriLocation);

                    // Check if the chosen Location is null
                    if (isValid && Board.pieces[chosenOriLocation[0], chosenOriLocation[1]] == null)
                    {
                        isValid = false;
                        throw new Exception("No piece can be chosen here");
                    }

                    // if there exist a piece in the chosen location
                    if (isValid && Board.pieces[chosenOriLocation[0], chosenOriLocation[1]] != null)
                    {
                        // check if the chosen piece is in the current colour, current move cannot be the piece of the opposite side
                        if (Board.pieces[chosenOriLocation[0], chosenOriLocation[1]].colour != Board.currentColour % 2)
                        {
                            isValid = false;
                            throw new Exception("This piece is not belong to you");
                        }

                        //Check if the chosen piece has any possible move
                        if (Board.pieces[chosenOriLocation[0], chosenOriLocation[1]].calculateValidMoves(chosenOriLocation).Count == 0)
                        {
                            isValid = false;
                            throw new Exception("This piece cannot move anywhere");
                        }
                    }
                }
                catch (Exception e)
                {
                    DisplayMessage.displayException(e);
                }
            } while (!isValid);
            // Clear the exception message line when the user enter a right input
            Console.SetCursorPosition(0, 26);
            DisplayMessage.clearConsoleLine();

            // Display the valid move in the console
            DisplayBoard.displayValidMove(chosenOriLocation);
        }

        // Receive the input destination location (move location) from the user
        public static void chooseDest()
        {
            List<int> validMove = Board.pieces[Board.getLastOriLocation()[0], Board.getLastOriLocation()[1]].
                calculateValidMoves(Board.getLastOriLocation());
            int[] chosenDestLocation = new int[2];
            bool isValid = true;

            // In this loop the player will need to input a location until the right one is input
            do
            {
                // Ask the player to choose a position to move to
                DisplayMessage.displayAskMoveMessage();
                try
                {
                    // Convert the string location to integer location
                    // if the parseLocation did not throw any exception, formatly the input is correct
                    chosenDestLocation = parseLocation(Console.ReadLine(), out isValid);

                    // Check if the chosenLocation is in the valid move list of the chosen piece
                    if (isValid && !validMove.Contains(chosenDestLocation[0] * 10 + chosenDestLocation[1]))
                    {
                        isValid = false;
                        throw new Exception("This move doesn't comply with the rule");
                    }

                    // If the player enter a location that is not able to avoid being checked, warn the player
                    // Assume the piece move to the chosen destination
                    moveTo(Board.getLastOriLocation(), chosenDestLocation);
                    // if is is the turn of the side being checked, the player need to confirme any dangerous move
                    if ((GameRules.isChecked()[0] && Board.currentColour % 2 == 0) ||
                        (GameRules.isChecked()[1] && Board.currentColour % 2 == 1))
                    {
                        DisplayMessage.displayMoveConfirmation();
                        // If the player does not confirm, then the input is invalid
                        if (Console.ReadLine() == "n") isValid = false;
                    }
                    // Clear the confirmation message
                    Console.SetCursorPosition(0, 28);
                    DisplayMessage.clearConsoleLine();
                    // Anyhow, move the piece back to original position to continue
                    moveTo(chosenDestLocation, Board.getLastOriLocation());

                    // Save this chosen destination location as last destination location
                    Board.addLastDestLocation(chosenDestLocation);
                }
                catch (Exception e)
                {
                    DisplayMessage.displayException(e);
                }
            } while (!isValid);
            // Clear the exception message line when the user enter a right input
            Console.SetCursorPosition(0, 26);
            DisplayMessage.clearConsoleLine();
            // clear the checked message line
            Console.SetCursorPosition(0, 27);
            DisplayMessage.clearConsoleLine();
            // Update the pieces positions and current colour in Board
            moveTo(Board.getLastOriLocation(), chosenDestLocation);

            // After moving display the regret message except for the 1 round
            DisplayMessage.displayRegretMessage();
        }

        // The moving operation
        public static void moveTo(int[] oriLocation, int[] destLocation)
        {
            // Move the chosen piece to the destination and reset the original position to null
            Pieces temp = Board.pieces[oriLocation[0], oriLocation[1]];
            Board.pieces[oriLocation[0], oriLocation[1]] = null;
            Board.pieces[destLocation[0], destLocation[1]] = temp;
        }

    }
}
