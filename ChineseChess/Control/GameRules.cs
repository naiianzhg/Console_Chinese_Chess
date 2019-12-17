using System;
using System.Collections.Generic;
using System.Text;
using ChineseChess.Model;
using ChineseChess.View;

namespace ChineseChess.Control
{
    class GameRules
    {
        // Initialize the Xiangqi game system
        public static void iniGame()
        {
            Board.iniChessBoard();
        }

        // return a bool[] where bool[0] is the checked situation for black, whereas bool[1] for red
        public static bool[] isChecked()
        {
            int[] redGeneralLocation, blkGeneralLocation;
            int[] enermyLocation = new int[2];
            // chked[0] = black is checked or not, chked[1] = red is checked or not
            bool[] chked = new bool[2];

            // Get the position of the general, and find out if there is a checked on the board
            blkGeneralLocation = Board.getBlkGeneralPosition();
            redGeneralLocation = Board.getRedGeneralPosition();

            // Calculate the valid moves of all the red(b) pieces
            for (int row = 0; row < Board.pieces.GetLength(0); row++)
            {
                for (int col = 0; col < Board.pieces.GetLength(1); col++)
                {
                    // once the valid moves of one red pieces contains the black general, the black is checked
                    if (Board.pieces[row, col] != null && Board.pieces[row, col].colour == 1)
                    {
                        enermyLocation[0] = row;
                        enermyLocation[1] = col;
                        if (Board.pieces[row, col].calculateValidMoves(enermyLocation).Contains(blkGeneralLocation[0] * 10 + blkGeneralLocation[1]))
                        {
                            chked[0] = true;
                            break;
                        }
                    }
                    // or once the valid moves of one black pieces contains the red general, the red is checked
                    else if (Board.pieces[row, col] != null && Board.pieces[row, col].colour == 0)
                    {
                        enermyLocation[0] = row;
                        enermyLocation[1] = col;
                        if (Board.pieces[row, col].calculateValidMoves(enermyLocation).Contains(redGeneralLocation[0] * 10 + redGeneralLocation[1]))
                        {
                            chked[1] = true;
                            break;
                        }
                    }
                }
                // Exit of the first for loop
                if (chked[0] || chked[1]) break;
            }

            return chked;
        }

        // TODO: checkmate
        public static bool isCheckmate()
        {
            return false;
        }

        // Regret move
        public static void regret()
        {
            // if the player still has chances for regret
            if (Board.regretAmount[Board.currentColour % 2] > 0)
            {
                PiecesHandler.moveTo(Board.getLastDestLocation(), Board.getLastOriLocation());
                // Reduce of regret chance by 1
                Board.regretAmount[Board.currentColour % 2]--;
                // After regret, the current colour change back
                Board.changeTurn();
            }
        }
    }
}
