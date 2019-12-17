using System.Collections.Generic;
using System.Text;
using ChineseChess.View;
using ChineseChess.Control;

namespace ChineseChess.Model
{

    abstract class Pieces
    {
        // odd->RED, even->BLACK
        public int colour { get; }
        // S,C,R,H,E,A,G
        public string type { get; }
        public List<int> validMoves { get; set; }
        public string url { get; set; }

        public Pieces (int colour, string type)
        {
            this.colour = colour;
            this.type = type;
        }

        public abstract List<int> calculateValidMoves (int[] location);
    } 

    class Soldier : Pieces
    {
        public Soldier(int colour) :
            base(colour, "S")
        {
            if (colour == 1) url = "/Images/RedSoldier.png";
            else url = "/Images/BlackSoldier.png";
        }

        public override List<int> calculateValidMoves(int[] location)
        {
            // Initialize the valideMove List
            validMoves = new List<int>();

           // The piece color
            if (colour == 1) // if it is a red piece
            {
                // if cross the river
                if (location[0] > 4) // not yet cross
                {
                    if (Board.pieces[location[0] - 1, location[1]] == null || Board.pieces[location[0] - 1, location[1]].colour != colour) // possible position has no piece or has a not-black piece(eat)
                    {
                        validMoves.Add((location[0] - 1) * 10 + location[1]); // only forward
                    }
                } else if (location[0] <= 4) // it did cross
                {
                    // if the piece is not on the buttom boarder
                    if (location[0] != 0 && (Board.pieces[location[0] - 1, location[1]] == null || Board.pieces[location[0] - 1, location[1]].colour != colour))
                    {
                        validMoves.Add((location[0] - 1) * 10 + location[1]); // 1. forward
                    }
                    // if the piece is not on the left boarder
                    if (location[1] != 0 && (Board.pieces[location[0], location[1] - 1] == null || Board.pieces[location[0], location[1] - 1].colour != colour))
                    {
                        validMoves.Add(location[0] * 10 + location[1] - 1); // 2. left
                    }
                    // if the piece is not on the right boarder
                    if (location[1] != 8 && Board.pieces[location[0], location[1] + 1] == null || Board.pieces[location[0], location[1] + 1].colour != colour)
                    {
                        validMoves.Add(location[0] * 10 + location[1] + 1); // 3. right
                    }
                }
            } else // else if it is a black piece
            {
                // if cross the river
                if (location[0] < 5) // not yet cross
                {
                    if (Board.pieces[location[0] + 1, location[1]] == null || Board.pieces[location[0] + 1, location[1]].colour != colour) // possible position has no piece or has a not-black piece(eat)
                    {
                        validMoves.Add((location[0] + 1) * 10 + location[1]); // only forward
                    }
                }
                else if (location[0] >= 5) // it did cross
                {
                    // if the piece is not on the buttom boarder
                    if (location[0] != 9 && Board.pieces[location[0] + 1, location[1]] == null || Board.pieces[location[0] + 1, location[1]].colour != colour)
                    {
                        validMoves.Add((location[0] + 1) * 10 + location[1]); // 1. forward
                    }
                    // if the piece is not on the left boarder
                    if (location[1] != 0 && Board.pieces[location[0], location[1] - 1] == null || Board.pieces[location[0], location[1] - 1].colour != colour)
                    {
                        validMoves.Add(location[0] * 10 + location[1] - 1); // 2. left
                    }
                    // if the piece is not on the right boarder
                    if (location[1] != 8 && Board.pieces[location[0], location[1] + 1] == null || Board.pieces[location[0], location[1] + 1].colour != colour)
                    {
                        validMoves.Add(location[0] * 10 + location[1] + 1); // 3. right
                    }
                }
            }

            return validMoves;
        }
    }

    class Cannon : Pieces
    {
        public Cannon(int colour) :
            base(colour, "C")
        {
            if (colour == 1) url = "/Images/RedCannon.png";
            else url = "/Images/BlackCannon.png";
        }

        public object[] obj = new object[2];

        public object[] addColumnValidMove(int count, int i, int[] location, List<int> validMoves)
        {
            if (Board.pieces[i, location[1]] == null && count == 0)
            {
                validMoves.Add(10 * i + location[1]);
            }
            else if (Board.pieces[i, location[1]] != null)
            {
                ++count;
                if (count == 2 && Board.pieces[i, location[1]].colour != colour)
                {
                    validMoves.Add(10 * i + location[1]);
                }
            }
            obj[0] = validMoves;
            obj[1] = count;
            return obj;
        }

        public object[] addRowValidMove(int count, int j, int[] location, List<int> validMoves)
        {
            if (Board.pieces[location[0], j] == null && count == 0)
            {
                validMoves.Add(10 * location[0] + j);
            }
            else if (Board.pieces[location[0], j] != null)
            {
                ++count;
                if (count == 2 && Board.pieces[location[0], j].colour != colour)
                {
                    validMoves.Add(10 * location[0] + j);
                }
            }
            obj[0] = validMoves;
            obj[1] = count;
            return obj;
        }

        public override List<int> calculateValidMoves(int[] location)
        {
            // Initialize the valideMove List
            validMoves = new List<int>();
            // To count the rank of the piece on the route (the first/second/... enermy piece)
            int count = 0;
            // Recieve the return value array of addValidMove functions
            object[] obj = new object[2];

            // UP Column possible moves
            for (int i = location[0] - 1; i >= 0; i--)
            {
                obj = addColumnValidMove(count, i, location, validMoves);
                validMoves = (List<int>)obj[0];
                count = (int)obj[1];
            }
            count = 0;

            // DOWN Column possible moves
            for (int i = location[0] + 1; i <= 9; i++)
            {
                obj = addColumnValidMove(count, i, location, validMoves);
                validMoves = (List<int>)obj[0];
                count = (int)obj[1];
            }
            count = 0;

            // RIGHT Row possible moves
            for (int j = location[1] + 1; j <= 8; j++)
            {
                obj = addRowValidMove(count, j, location, validMoves);
                validMoves = (List<int>)obj[0];
                count = (int)obj[1];
            }
            count = 0;

            // LEFT Row possible moves
            for (int j = location[1] - 1; j >= 0; j--)
            {
                obj = addRowValidMove(count, j, location, validMoves);
                validMoves = (List<int>)obj[0];
                count = (int)obj[1];
            }
            count = 0;
            
            return validMoves;
        }
    }

    class Rook : Pieces
    {
        public Rook(int colour) :
            base(colour, "R")
        {
            if (colour == 1) url = "/Images/RedRook.png";
            else url = "/Images/BlackRook.png";
        }

        public List<int> addColumnValidMove(int i, int[] location, List<int> validMoves, out bool block)
        {
            block = false;
            if (Board.pieces[i, location[1]] != null && Board.pieces[i, location[1]].colour == colour)
            {
                block = true;
            } else if (Board.pieces[i, location[1]] == null)
            {
                validMoves.Add(10 * i + location[1]);
            } else if (Board.pieces[i, location[1]] != null && Board.pieces[i, location[1]].colour != colour)
            {
                validMoves.Add(10 * i + location[1]);
                block = true;
            }
            return validMoves;
        }

        public List<int> addRowValidMove(int j, int[] location, List<int> validMoves, out bool block)
        {
            block = false;
            if (Board.pieces[location[0], j] != null && Board.pieces[location[0], j].colour == colour)
            {
                block = true;
            } else if (Board.pieces[location[0], j] == null)
            {
                validMoves.Add(10 * location[0] + j);
            } else if (Board.pieces[location[0], j] != null && Board.pieces[location[0], j].colour != colour)
            {
                validMoves.Add(10 * location[0] + j);
                block = true;
            }
            return validMoves;
        }

        public override List<int> calculateValidMoves(int[] location)
        {
            // Initialize the valideMove List
            validMoves = new List<int>();
            bool block = false;

            // UP Column possible moves
            for (int i = location[0] - 1; i >= 0; i--)
            {
                validMoves = addColumnValidMove(i, location, validMoves, out block);
                if (block)
                {
                    break;
                }
            }
            block = false;

            // DOWN Column possible moves
            for (int i = location[0] + 1; i <= 9; i++)
            {
                validMoves = addColumnValidMove(i, location, validMoves, out block);
                if (block)
                {
                    break;
                }
            }
            block = false;

            // RIGHT Row possible moves
            for (int j = location[1] + 1; j <= 8; j++)
            {
                validMoves = addRowValidMove(j, location, validMoves, out block);
                if (block)
                {
                    break;
                }
            }
            block = false;

            // LEFT Row possible moves
            for (int j = location[1] - 1; j >= 0; j--)
            {
                validMoves = addRowValidMove(j, location, validMoves, out block);
                if (block)
                {
                    break;
                }
            }
            block = false;

            return validMoves;
        }
    }

    class Horse : Pieces
    {
        public Horse(int colour) :
            base(colour, "H")
        {
            if (colour == 1) url = "/Images/RedHorse.png";
            else url = "/Images/BlackHorse.png";
        }

        public override List<int> calculateValidMoves(int[] location)
        {
            // Initialize the valideMove List
            validMoves = new List<int>();

            // Cannot be on the buttom boarder or have distance of 1 from the boarder - Upward moving
            // if there is piece blocking the route forward
            if (location[0] > 1 && Board.pieces[location[0] - 1, location[1]] == null)
            {
                // Detecte the position (x-2, y+1)
                if (location[1] != 8 && (Board.pieces[location[0] - 2, location[1] + 1] == null || Board.pieces[location[0] - 2, location[1] + 1].colour != colour))
                {
                    validMoves.Add((location[0] - 2) * 10 + location[1] + 1);
                }
                // Also detecte the position (x-2, y-1)
                if (location[1] != 0 && (Board.pieces[location[0] - 2, location[1] - 1] == null || Board.pieces[location[0] - 2, location[1] - 1].colour != colour))
                {
                    validMoves.Add((location[0] - 2) * 10 + location[1] - 1);
                }
            }

            // Cannot be on the buttom boarder or have distance of 1 from the boarder - Downward moving
            // if there is piece blocking the route forward
            if (location[0] < 8 && Board.pieces[location[0] + 1, location[1]] == null)
            {
                // Detecte the position (x+2, y+1)
                if (location[1] != 8 && (Board.pieces[location[0] + 2, location[1] + 1] == null || Board.pieces[location[0] + 2, location[1] + 1].colour != colour))
                {
                    validMoves.Add((location[0] + 2) * 10 + location[1] + 1);
                }
                // Also detecte the position (x+2, y-1)
                if (location[1] != 0 && (Board.pieces[location[0] + 2, location[1] - 1] == null || Board.pieces[location[0] + 2, location[1] - 1].colour != colour))
                {
                    validMoves.Add((location[0] + 2) * 10 + location[1] - 1);
                }
            }

            // Cannot be on the buttom boarder or have distance of 1 from the boarder - Leftward moving
            // if there is piece blocking the route forward
            if (location[1] > 1 && Board.pieces[location[0], location[1] - 1] == null)
            {
                // Detecte the position (x+1, y-2)
                if (location[0] != 9 && (Board.pieces[location[0] + 1, location[1] - 2] == null || Board.pieces[location[0] + 1, location[1] - 2].colour != colour))
                {
                    validMoves.Add((location[0] + 1) * 10 + location[1] - 2);
                }
                // Also detecte the position (x-1, y-2)
                if (location[0] != 0 && (Board.pieces[location[0] - 1, location[1] - 2] == null || Board.pieces[location[0] - 1, location[1] - 2].colour != colour))
                {
                    validMoves.Add((location[0] - 1) * 10 + location[1] - 2);
                }
            }

            // Cannot be on the buttom boarder or have distance of 1 from the boarder - Rightward moving
            // if there is piece blocking the route forward
            if (location[1] < 7 && Board.pieces[location[0], location[1] + 1] == null)
            {
                // Detecte the position (x+1, y+2)
                if (location[0] != 9 && (Board.pieces[location[0] + 1, location[1] + 2] == null || Board.pieces[location[0] + 1, location[1] + 2].colour != colour))
                {
                    validMoves.Add((location[0] + 1) * 10 + location[1] + 2);
                }
                // Also detecte the position (x-1, y+2)
                if (location[0] != 0 && (Board.pieces[location[0] - 1, location[1] + 2] == null || Board.pieces[location[0] - 1, location[1] + 2].colour != colour))
                {
                    validMoves.Add((location[0] - 1) * 10 + location[1] + 2);
                }
            }

            return validMoves;
        }
    }

    class Elephant : Pieces
    {
        public Elephant(int colour) :
            base(colour, "E")
        {
            if (colour == 1) url = "/Images/RedElephant.png";
            else url = "/Images/BlackElephant.png";
        }

        // Collect the possible ↖ move
        public List<int> addTLValidMove(int[] location, List<int> validMoves)
        {
            if (Board.pieces[location[0] - 2, location[1] - 2] == null || Board.pieces[location[0] - 2, location[1] - 2].colour != colour)
                {
                validMoves.Add((location[0] - 2) * 10 + location[1] - 2);
            }
            return validMoves;
        }

        // Collect the possible ↗ move
        public List<int> addTRValidMove(int[] location, List<int> validMoves)
        {
            if (Board.pieces[location[0] - 2, location[1] + 2] == null || Board.pieces[location[0] - 2, location[1] + 2].colour != colour)
            {
                validMoves.Add((location[0] - 2) * 10 + location[1] + 2);
            }
            return validMoves;
        }

        // Collect the possible ↙ move
        public List<int> addBLValidMove(int[] location, List<int> validMoves)
        {
            if (Board.pieces[location[0] + 2, location[1] - 2] == null || Board.pieces[location[0] + 2, location[1] - 2].colour != colour)
            {
                validMoves.Add((location[0] + 2) * 10 + location[1] - 2);
            }
            return validMoves;
        }

        // Collect the possible ↘ move
        public List<int> addBRValidMove(int[] location, List<int> validMoves)
        {
            if (Board.pieces[location[0] + 2, location[1] + 2] == null || Board.pieces[location[0] + 2, location[1] + 2].colour != colour)
            {
                validMoves.Add((location[0] + 2) * 10 + location[1] + 2);
            }
            return validMoves;
        }

        public override List<int> calculateValidMoves(int[] location)
        {
            // Initialize the valideMove List
            validMoves = new List<int>();

            // Elephant can only move in its own side
            // Cannot be on the buttom boarder or cross the river - ↖ moving
            // if there is piece blocking the route
            // For the Red Elephants, detecte the position (x-2, y-2)
            if (colour == 1 && location[1] != 0 && location[0] != 5 && Board.pieces[location[0] - 1, location[1] - 1] == null)
            {
                validMoves = addTLValidMove(location, validMoves);
            } else if (colour == 0 && location[1] != 0 && location[0] != 0 && Board.pieces[location[0] - 1, location[1] - 1] == null) // For the Black Elephants, detecte the position (x-2, y-2)
            {
                validMoves = addTLValidMove(location, validMoves);
            }

            // Cannot be on the buttom boarder or cross the river - ↗ moving
            // if there is piece blocking the route
            // For the Red Elephants, detecte the position (x-2, y+2)
            if (colour == 1 && location[1] != 8 && location[0] != 5 && Board.pieces[location[0] - 1, location[1] + 1] == null)
            {
                validMoves = addTRValidMove(location, validMoves);
            }
            else if (colour == 0 && location[1] != 8 && location[0] != 0 && Board.pieces[location[0] - 1, location[1] + 1] == null) // For the Black Elephants, detecte the position (x-2, y+2)
            {
                validMoves = addTRValidMove(location, validMoves);
            }

            // Cannot be on the buttom boarder or have distance of 1 from the boarder - ↙ moving
            // if there is piece blocking the route forward
            // For the Red Elephants, detecte the position (x+2, y-2)
            if (colour == 1 && location[1] != 0 && location[0] != 9 && Board.pieces[location[0] + 1, location[1] - 1] == null)
            {
                validMoves = addBLValidMove(location, validMoves);
            }
            else if (colour == 0 && location[1] != 0 && location[0] != 4 && Board.pieces[location[0] + 1, location[1] - 1] == null) // For the Black Elephants, detecte the position (x+2, y-2)
            {
                validMoves = addBLValidMove(location, validMoves);
            }

            // Cannot be on the buttom boarder or have distance of 1 from the boarder - ↘ moving
            // if there is piece blocking the route forward
            // For the Red Elephants, detecte the position (x+2, y+2)
            if (colour == 1 && location[1] != 8 && location[0] != 9 && Board.pieces[location[0] + 1, location[1] + 1] == null)
            {
                validMoves = addBRValidMove(location, validMoves);
            }
            else if (colour == 0 && location[1] != 8 && location[0] != 4 && Board.pieces[location[0] + 1, location[1] + 1] == null) // For the Black Elephants, detecte the position (x+2, y+2)
            {
                validMoves = addBRValidMove(location, validMoves);
            }

            return validMoves;
        }
    }

    class Advisor : Pieces
    {
        public Advisor(int colour) :
            base(colour, "A")
        {
            if (colour == 1) url = "/Images/RedAdvisor.png";
            else url = "/Images/BlackAdvisor.png";
        }

        public override List<int> calculateValidMoves(int[] location)
        {
            validMoves = new List<int>();

            // If the piece is on the center of the grid
            if (location[1] == 4)
            {
                // The ↖ move
                if (Board.pieces[location[0] - 1, location[1] - 1] == null || Board.pieces[location[0] - 1, location[1] -1].colour != colour)
                {
                    validMoves.Add((location[0] - 1) * 10 + location[1] - 1);
                }
                // The ↗ move
                if (Board.pieces[location[0] - 1, location[1] + 1] == null || Board.pieces[location[0] - 1, location[1] + 1].colour != colour)
                {
                    validMoves.Add((location[0] - 1) * 10 + location[1] + 1);
                }
                // The ↙ move
                if (Board.pieces[location[0] + 1, location[1] - 1] == null || Board.pieces[location[0] + 1, location[1] - 1].colour != colour)
                {
                    validMoves.Add((location[0] + 1) * 10 + location[1] - 1);
                }
                // The ↘ move
                if (Board.pieces[location[0] + 1, location[1] + 1] == null || Board.pieces[location[0] + 1, location[1] + 1].colour != colour)
                {
                    validMoves.Add((location[0] + 1) * 10 + location[1] + 1);
                }
            } else // Otherwise, the piece will be on the corner of the grid
            {
                if (colour == 1 && (Board.pieces[8, 4] == null || Board.pieces[8, 4].colour != colour))
                {
                    validMoves.Add(84);
                } else if (colour == 0 && (Board.pieces[1, 4] == null || Board.pieces[1, 4].colour != colour))
                {
                    validMoves.Add(14);
                }
            }

            return validMoves;
        }
    }

    class General : Pieces
    {
        public General(int colour) :
            base(colour, "G")
        {
            if (colour == 1) url = "/Images/RedGeneral.png";
            else url = "/Images/BlackGeneral.png";
        }

        public bool isBlocked(int colour, int[] location)
        {
            bool block = false;
            // Detecte if there exists a piece on the column between the two generals, if so it is blocked
            if (colour == 1)
            {
                for (int row = location[0]+1; row <= 9; row++)
                {
                    if (Board.pieces[row, location[1]] != null && Board.pieces[row, location[1]].GetType() != typeof(General))
                    {
                        block = true;
                        break;
                    }
                }
            } else
            {
                for (int row = location[0]-1; row >= 0; row--)
                {
                    if (Board.pieces[row, location[1]] != null && Board.pieces[row, location[1]].GetType() != typeof(General))
                    {
                        block = true;
                        break;
                    }
                }
            }
            return block;
        }

        public List<int> addFlyingGeneral(int colour, int[] location, List<int> validMoves)
        {
            if (colour == 1)
            {
                for (int row = 7; row <= 9; row++)
                {
                    if (Board.pieces[row, location[1]] != null && Board.pieces[row, location[1]].GetType() == typeof(General))
                    {
                        validMoves.Add(row * 10 + location[1]);
                    }
                }
            }
            else if (colour == 0)
            {
                for (int row = 3; row >= 0; row--)
                {
                    if (Board.pieces[row, location[1]] != null && Board.pieces[row, location[1]].GetType() == typeof(General))
                    {
                        validMoves.Add(row * 10 + location[1]);
                    }
                }
            }
            return validMoves;
        }

        public List<int> addUpValidMove(int colour, int[] location, List<int> validMoves)
        {
            if (colour == 1 && location[0] != 7 &&
                (Board.pieces[location[0] - 1, location[1]] == null || Board.pieces[location[0] - 1, location[1]].colour != colour))
            {
                validMoves.Add((location[0] - 1) * 10 + location[1]);
            }
            else if (colour == 0 && location[0] != 0 &&
              (Board.pieces[location[0] - 1, location[1]] == null || Board.pieces[location[0] - 1, location[1]].colour != colour))
            {
                validMoves.Add((location[0] - 1) * 10 + location[1]);
            }
            return validMoves;
        }

        public List<int> addDownValidMove(int colour,int[] location, List<int> validMoves)
        {
            if (colour == 1 && location[0] != 9 &&
                (Board.pieces[location[0] + 1, location[1]] == null || Board.pieces[location[0] + 1, location[1]].colour != colour))
            {
                validMoves.Add((location[0] + 1) * 10 + location[1]);
            }
            else if (colour == 0 && location[0] != 2 &&
              (Board.pieces[location[0] + 1, location[1]] == null || Board.pieces[location[0] + 1, location[1]].colour != colour))
            {
                validMoves.Add((location[0] + 1) * 10 + location[1]);
            }
            return validMoves;
        }

        public List<int> addLeftValidMove(int colour, int[] location, List<int> validMoves)
        {
            if (location[1] != 3 &&
                (Board.pieces[location[0], location[1] - 1] == null || Board.pieces[location[0], location[1] - 1].colour != colour))
            {
                validMoves.Add(location[0] * 10 + location[1] - 1);
            }
            return validMoves;
        }

        public List<int> addRightValidMove(int colour, int[] location, List<int> validMoves)
        {
            if (location[1] != 5 &&
                (Board.pieces[location[0], location[1] + 1] == null || Board.pieces[location[0], location[1] + 1].colour != colour))
            {
                validMoves.Add(location[0] * 10 + location[1] + 1);
            }
            return validMoves;
        }

        public override List<int> calculateValidMoves(int[] location)
        {
            validMoves = new List<int>();

            // Flying general
            if (!isBlocked(colour, location))
            {
                validMoves = addFlyingGeneral(colour, location, validMoves);
            }

            // Upward moving
            validMoves = addUpValidMove(colour, location, validMoves);

            // Downward moving
            validMoves = addDownValidMove(colour, location, validMoves);

            // Leftward moving
            validMoves = addLeftValidMove(colour, location, validMoves);

            // Rightward moving
            validMoves = addRightValidMove(colour, location, validMoves);

            return validMoves;
        }
    }
    
}
