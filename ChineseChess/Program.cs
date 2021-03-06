﻿using System;
using ChineseChess.Model;
using ChineseChess.View;
using ChineseChess.Control;


namespace ChineseChess
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isChosen;
            bool isMoved;
            // Initialization of chess board and pieces
            GameRules.iniGame();
            do
            {
                do
                {
                    // Choose a piece and display the valid move
                    PiecesHandler.chooseOri();
                    isChosen = true;

                } while (!isChosen);
                isChosen = false;

                do
                {
                    // Choose a destination and move the chosen piece
                    PiecesHandler.chooseDest();
                    isMoved = true;

                    // Display if there is a checked
                    DisplayMessage.displayChecked();

                } while (!isMoved);
                isMoved = false;

            } while (!GameRules.isCheckmate());

            Console.SetCursorPosition(0, 28);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nWINNER WINNER CHICKEN DINER!");
            Console.ResetColor();
        }
    }
}
