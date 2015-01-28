using Chess_Project.Buisness.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess_Project.Buisness.Models;

namespace Chess_Project.Buisness.Logic
{
    class ChessBoard
    {
        private Piece[,] Pieces;
        public delegate void CheckmateEventHandler(PieceColor color,List<Piece> AvailablePiecesMovments=null);
        public delegate void WinnerEventHandler(PieceColor winner);
        public event CheckmateEventHandler Checkmate;
        public event WinnerEventHandler Winner;

        public ChessBoard()
        {
            Initilize();
        }
        private Piece GetEmptyPiece(Position position)
        {
            return new Piece(Enums.PieceType.Empty, Enums.PieceColor.Unknown,position);
        }
        private void Initilize()
        {
            Pieces=new Piece[8,8];

            // Initilize Empty Pieces :
            for (int column = 0; column < 8; column++)
            {
                for (int row = 2; row <= 5; row++)
                {
                    Pieces[row,column] = new Piece(Enums.PieceType.Empty, Enums.PieceColor.Unknown, new Position(row + 1, column + 1));
                }
            }

            #region Initilize White Pieces :

            // Initilize Left Rook
            Pieces[0, 0] = new Piece(Enums.PieceType.Rook, Enums.PieceColor.White, new Position(1, 1));

            // Initilize Right Rook
            Pieces[0, 7] = new Piece(Enums.PieceType.Rook, Enums.PieceColor.White, new Position(1, 8));

            // Initilize Right Knight
            Pieces[0, 6] = new Piece(Enums.PieceType.Knight, Enums.PieceColor.White, new Position(1, 7));

            // Initilize Left Knight
            Pieces[0, 1] = new Piece(Enums.PieceType.Knight, Enums.PieceColor.White, new Position(1, 2));

            // Initilize Right Bishpop
            Pieces[0, 5] = new Piece(Enums.PieceType.Bishop, Enums.PieceColor.White, new Position(1, 6));

            // Initilize Left Bishio
            Pieces[0, 2] = new Piece(Enums.PieceType.Bishop, Enums.PieceColor.White, new Position(1, 3));

            // Initilize Queen
            Pieces[0, 3] = new Piece(Enums.PieceType.King, Enums.PieceColor.White, new Position(1, 4));

            // Initilize King
            Pieces[0, 4] = new Piece(Enums.PieceType.Queen, Enums.PieceColor.White, new Position(1, 5));

            // Initilize Pawns
            for (int column = 0; column < 8; column++)
			{
                Pieces[1,column] = new Piece(Enums.PieceType.Pawn, Enums.PieceColor.White, new Position(2,column+1));
            }

            #endregion

            #region Initilize Black Pieces :

            // Initilize Left Rook
            Pieces[7, 0] = new Piece(Enums.PieceType.Rook, Enums.PieceColor.Black, new Position(8, 1));

            // Initilize Right Rook
            Pieces[7, 7] = new Piece(Enums.PieceType.Rook, Enums.PieceColor.Black, new Position(8, 8));

            // Initilize Right Knight
            Pieces[7, 6] = new Piece(Enums.PieceType.Knight, Enums.PieceColor.Black, new Position(8, 7));

            // Initilize Left Knight
            Pieces[7, 1] = new Piece(Enums.PieceType.Knight, Enums.PieceColor.Black, new Position(8, 2));

            // Initilize Right Bishpop
            Pieces[7, 5] = new Piece(Enums.PieceType.Bishop, Enums.PieceColor.Black, new Position(8, 6));

            // Initilize Left Bishio
            Pieces[7, 2] = new Piece(Enums.PieceType.Bishop, Enums.PieceColor.Black, new Position(8, 3));

            // Initilize Queen
            Pieces[7, 3] = new Piece(Enums.PieceType.King, Enums.PieceColor.Black, new Position(8, 4));

            // Initilize King
            Pieces[7, 4] = new Piece(Enums.PieceType.Queen, Enums.PieceColor.Black, new Position(8, 5));

            // Initilize Pawns
            for (int column = 0; column < 8; column++)
            {
                Pieces[6, column] = new Piece(Enums.PieceType.Pawn, Enums.PieceColor.Black, new Position(7, column + 1));
                
            }

            #endregion
            
            CalculateAvailableMovmmetsForWhite();
            
        }

        private Piece[,] GetCopyofBoard()
        {
            Piece[,] tempBoard = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    tempBoard[i, j] = Pieces[i, j].Clone();
                }
            }
            return tempBoard;
        }

        public Piece GetPiece(Position position)
        {
            foreach (var item in Pieces)
            {
                if (item.CurrentPosision == position)
                {
                    return item;

                }
            }
            return null;
        }
        public bool MovePieceToPosition(Position currentPosition , Position newPosition)
        {
            Piece selectedPiece = GetPiece(currentPosition);
            
            if (selectedPiece == null)
                throw new SystemException("piece for current posistion does not found");
            if (selectedPiece.Color == Enums.PieceColor.Unknown)
                return false;
            if(selectedPiece.AvailabePosisions.Exists(position => newPosition.Row== position.Row && newPosition.Column== position.Column))
            {
                //  Moving piece :
                Position oldPosition = new Position(selectedPiece.CurrentPosision.Row, selectedPiece.CurrentPosision.Column);
                selectedPiece.CurrentPosision = newPosition;
                Pieces[oldPosition.Row - 1, oldPosition.Column - 1] = GetEmptyPiece(oldPosition);
                Pieces[newPosition.Row - 1, newPosition.Column - 1] = selectedPiece.Clone();

                // Calculate Available Movments for the opposite player :

                if(selectedPiece.Color== Enums.PieceColor.White)
                {
                    // Check if checkmate on black and 
                    // if not then Calculate Available Movments For Black Normally 
                    // if black is checkmate  Detect posible movments to remove the checkmate and if not then White is the winner
                    if(!(IsBlackKingCheckMate(Pieces)))
                        CalculateAvailableMovmmetsForBlack();
                    else
                    {
                        List<Piece> posibleSolutions = GetPosibleSolutionForCheckmate(PieceColor.Black);
                        if((posibleSolutions==null) && (Winner!=null))
                                Winner(PieceColor.White);
                        else if((posibleSolutions!=null) && (Checkmate!=null))
                        {
                            foreach(var item in Pieces)
                            {
                                if (posibleSolutions.Exists(x => x.Type == item.Type && x.Color == item.Color && x.CurrentPosision == item.CurrentPosision))
                                    item.AvailabePosisions = posibleSolutions.Find(x => x.Type == item.Type && x.Color == item.Color && x.CurrentPosision == item.CurrentPosision).AvailabePosisions;
                            }
                            Checkmate(selectedPiece.Color,posibleSolutions);
                        }
                            

                            
                            
                    }
                }
                    
                else
                {
                    // Check if checkmate on white and 
                    // if not then Calculate Available Movments For White Normally 
                    // if white is checkmate  Detect posible movments to remove the checkmate and if not then Black is the winner
                    if(!(IsWhiteKingCheckMate(Pieces)))
                        CalculateAvailableMovmmetsForWhite();
                    else
                    {
                        List<Piece> posibleSolutions = GetPosibleSolutionForCheckmate(PieceColor.White);
                        if ((posibleSolutions == null) && (Winner != null))
                            Winner(PieceColor.Black);
                        else if ((posibleSolutions != null) && (Checkmate != null))
                        {
                            foreach (var item in Pieces)
                            {
                                if (posibleSolutions.Exists(x => x.Type == item.Type && x.Color == item.Color && x.CurrentPosision == item.CurrentPosision))
                                    item.AvailabePosisions = posibleSolutions.Find(x => x.Type == item.Type && x.Color == item.Color && x.CurrentPosision == item.CurrentPosision).AvailabePosisions;
                            }
                            Checkmate(selectedPiece.Color, posibleSolutions);
                        }

                    }

                }
                    

                return true;
            }
            return false;
        }

        private List<Piece> GetPosibleSolutionForCheckmate(PieceColor pieceColor)
        {
            List<Piece> AvailablePiecesToMove=new List<Piece>();
            
            // Copy the board //and examin all avaialable movments for all pieces for selected color
            Piece[,] TempBoard=CalculateAvailableMovmmets(GetCopyofBoard(),pieceColor);

            //  Simulate every posible move for every piece 
            foreach(Piece item  in TempBoard)
            {
                // Examin piece for same selected color
                if(item.Color== pieceColor)
                {
                    Piece CopiedItem = item.Clone();
                    CopiedItem.AvailabePosisions.Clear();
                    // Examin every move 
                    foreach(Position position in item.AvailabePosisions)
                    {
                        // Moving item to new postion
                        Piece CopiedWherePostion= TempBoard[position.Row - 1, position.Column - 1].Clone();
                        TempBoard[item.CurrentPosision.Row - 1, item.CurrentPosision.Column - 1] = GetEmptyPiece(item.CurrentPosision);
                        TempBoard[position.Row - 1, position.Column - 1] = item.Clone();

                        // Check for checkmate
                        //     if  no checkmate  then add this new position
                        if ((pieceColor== PieceColor.White) && (!(IsWhiteKingCheckMate(TempBoard))))
                        {
                            CopiedItem.AvailabePosisions.Add(position);
                        }
                            
                        if ((pieceColor== PieceColor.Black) && (!(IsBlackKingCheckMate(TempBoard))))
                        {
                            CopiedItem.AvailabePosisions.Add(position);
                        }
                        
                        // Undo Moving 
                        TempBoard[CopiedItem.CurrentPosision.Row - 1, CopiedItem.CurrentPosision.Column - 1] = CopiedItem;
                        TempBoard[CopiedWherePostion.CurrentPosision.Row - 1, CopiedWherePostion.CurrentPosision.Column - 1] = CopiedWherePostion;

                    }
                    // Detect if this piece have available movments 
                    if (TempBoard[item.CurrentPosision.Row - 1, item.CurrentPosision.Column - 1].AvailabePosisions.Count > 0)
                    {
                        AvailablePiecesToMove.Add(TempBoard[item.CurrentPosision.Row - 1, item.CurrentPosision.Column - 1].Clone());
                    }
                }
            }
            
            if(AvailablePiecesToMove.Count==0)
                return null;
            return AvailablePiecesToMove;
        }
        private void CalculateAvailableMovmmetsForWhite()
        {
            // foreach white piece on boad do this : 
            foreach (var piece in Pieces)
            {
                // Clear all available positions
                piece.AvailabePosisions.Clear();
                if(piece.Color== Enums.PieceColor.White)
                {
                    switch (piece.Type)
                    {
                        case Chess_Project.Buisness.Enums.PieceType.Empty:
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.King:
                            piece.AvailabePosisions = AvailableMovmentsForKing(piece);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Queen:
                            piece.AvailabePosisions = AvailableMovmentsForQueen(piece);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Rook:
                            piece.AvailabePosisions = AvailableMovmentsForRook(piece);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Bishop:
                            piece.AvailabePosisions = AvailableMovmentsForBishp(piece);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Pawn:
                            piece.AvailabePosisions = AvailableMovmentsForPawn(piece);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Knight:
                            piece.AvailabePosisions = AvailableMovmentsForKnight(piece);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private Piece[,] CalculateAvailableMovmmets(Piece[,] Board,PieceColor color)
        {
            // foreach  piece on boad do this : 
            foreach (var piece in Board)
            {
                // Clear all available positions
                piece.AvailabePosisions.Clear();
                if (piece.Color == color)
                {
                    switch (piece.Type)
                    {
                        case Chess_Project.Buisness.Enums.PieceType.Empty:
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.King:
                            piece.AvailabePosisions = AvailableMovmentsForKing(piece);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Queen:
                            piece.AvailabePosisions = AvailableMovmentsForQueen(piece,false);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Rook:
                            piece.AvailabePosisions = AvailableMovmentsForRook(piece,false);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Bishop:
                            piece.AvailabePosisions = AvailableMovmentsForBishp(piece,false);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Pawn:
                            piece.AvailabePosisions = AvailableMovmentsForPawn(piece,false);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Knight:
                            piece.AvailabePosisions = AvailableMovmentsForKnight(piece,false);
                            break;
                        default:
                            break;
                    }
                }
            }
            return Board;
        }
        private void CalculateAvailableMovmmetsForBlack()
        {
            // foreach white piece on boad do this : 
            foreach (var piece in Pieces)
            {
                // Clear all available positions
                piece.AvailabePosisions.Clear();
                if (piece.Color == Enums.PieceColor.Black)
                {
                    switch (piece.Type)
                    {
                        case Chess_Project.Buisness.Enums.PieceType.Empty:
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.King:
                            piece.AvailabePosisions = AvailableMovmentsForKing(piece);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Queen:
                            piece.AvailabePosisions = AvailableMovmentsForQueen(piece);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Rook:
                            piece.AvailabePosisions = AvailableMovmentsForRook(piece);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Bishop:
                            piece.AvailabePosisions = AvailableMovmentsForBishp(piece);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Pawn:
                            piece.AvailabePosisions = AvailableMovmentsForPawn(piece);
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Knight:
                            piece.AvailabePosisions = AvailableMovmentsForKnight(piece);
                            break;
                        default:
                            break;
                    }
                }
            }

        }

        // Check movment availabilty by removing selected piece and detect after removing it, is there checkmate status 
        private bool CheckMovmentAvailablityForCheckMate(Piece piece)
        {
            // Detect if selected piece is empty
            if((piece.Color== Enums.PieceColor.Unknown) || (piece.Type== Enums.PieceType.Empty))
                return true;

            // Detect if after moving this piece to new position the king of the same color is Checkmate : 
            Piece[,] TempPieces = GetCopyofBoard();
            // Search for selected piece in temp board and when you find it, make it empty
            foreach(var item in TempPieces)
            {
                if((item.Color == piece.Color) && (item.CurrentPosision==piece.CurrentPosision) && (item.Type== piece.Type))
                {
                    item.Type = Enums.PieceType.Empty;
                    item.Color = Enums.PieceColor.Unknown;
                    break;
                }

            }

            if (piece.Color == Enums.PieceColor.White)
                return (!IsWhiteKingCheckMate(TempPieces));
            else
                return (!(IsBlackKingCheckMate(TempPieces)));

        }

        // Check movment availabilty by moving selected piece into new postion and detect after removing it, is there checkmate status 
        private bool CheckMovmentAvailablityForCheckMate(Piece piece,Position newPostion)
        {
            // Detect if selected piece is empty
            if ((piece.Color == Enums.PieceColor.Unknown) || (piece.Type == Enums.PieceType.Empty))
                return true;
            
            // Detect if after moving this piece to new position the king of the same color is Checkmate : 
            Piece[,] TempPieces = GetCopyofBoard();
            // Search for selected piece in temp board and when you find it, make it empty after make new distination equals to it
            bool  firstSucess=false;
            bool secondSucess = false;
            foreach (var item in TempPieces)
            {
                if ((item.Color == piece.Color) && (item.CurrentPosision == piece.CurrentPosision) && (item.Type == piece.Type))
                {
                    item.Type = Enums.PieceType.Empty;
                    item.Color = Enums.PieceColor.Unknown;
                    firstSucess = true;
                }
                if (item.CurrentPosision == newPostion)
                {
                    item.Type = piece.Type;
                    item.Color = piece.Color;
                    secondSucess = true;
                }
                if (firstSucess && secondSucess)
                    break;

            }

            if (piece.Color == Enums.PieceColor.White)
                return (!IsWhiteKingCheckMate(TempPieces));
            else
                return (!(IsBlackKingCheckMate(TempPieces)));

        }

        #region Available Movments for all types of piecess
        private List<Position> AvailableMovmentsForKing(Piece King)
        {
            List<Position> AvailableMovments = new List<Position>();
            
            // Detect if king can move up
            if(King.CurrentPosision.Row!=8)
            {
                // if the piece above the king not same color of king then check for checkmate if the king move above 
                if(Pieces[King.CurrentPosision.Row,King.CurrentPosision.Column-1].Color != King.Color)
                {
                    if (CheckMovmentAvailablityForCheckMate(King, new Position(King.CurrentPosision.Row + 1, King.CurrentPosision.Column)))
                        AvailableMovments.Add(new Position(King.CurrentPosision.Row + 1, King.CurrentPosision.Column));
                }
            }
            
            // Detect if king can move down
            if (King.CurrentPosision.Row != 1)
            {
                // if the piece down the king not same color of king then check for checkmate if the king move above 
                if (Pieces[King.CurrentPosision.Row-2, King.CurrentPosision.Column-1].Color != King.Color)
                {
                    if (CheckMovmentAvailablityForCheckMate(King, new Position(King.CurrentPosision.Row-1, King.CurrentPosision.Column)))
                        AvailableMovments.Add(new Position(King.CurrentPosision.Row-1, King.CurrentPosision.Column));
                }
            }

            // Detect if king can move left
            if (King.CurrentPosision.Column != 1)
            {
                
                if (Pieces[King.CurrentPosision.Row - 1, King.CurrentPosision.Column - 2].Color != King.Color)
                {
                    if (CheckMovmentAvailablityForCheckMate(King, new Position(King.CurrentPosision.Row, King.CurrentPosision.Column-1)))
                        AvailableMovments.Add(new Position(King.CurrentPosision.Row, King.CurrentPosision.Column-1));
                }
            }

            // Detect if king can move right
            if (King.CurrentPosision.Column != 8)
            {
                if (Pieces[King.CurrentPosision.Row - 1, King.CurrentPosision.Column].Color != King.Color)
                {
                    if (CheckMovmentAvailablityForCheckMate(King, new Position(King.CurrentPosision.Row, King.CurrentPosision.Column+1)))
                        AvailableMovments.Add(new Position(King.CurrentPosision.Row, King.CurrentPosision.Column +1));
                }
            }

            // Detect if king can move up left
            if ((King.CurrentPosision.Row!=8)&&(King.CurrentPosision.Column != 1))
            {

                if (Pieces[King.CurrentPosision.Row, King.CurrentPosision.Column - 2].Color != King.Color)
                {
                    if (CheckMovmentAvailablityForCheckMate(King, new Position(King.CurrentPosision.Row+1, King.CurrentPosision.Column - 1)))
                        AvailableMovments.Add(new Position(King.CurrentPosision.Row+1, King.CurrentPosision.Column - 1));
                }
            }

            // Detect if king can move up right
            if ((King.CurrentPosision.Row != 8) && (King.CurrentPosision.Column != 8))
            {

                if (Pieces[King.CurrentPosision.Row, King.CurrentPosision.Column].Color != King.Color)
                {
                    if (CheckMovmentAvailablityForCheckMate(King, new Position(King.CurrentPosision.Row + 1, King.CurrentPosision.Column + 1)))
                        AvailableMovments.Add(new Position(King.CurrentPosision.Row + 1, King.CurrentPosision.Column + 1));
                }
            }

            // Detect if king can move down left
            if ((King.CurrentPosision.Row != 1) && (King.CurrentPosision.Column != 1))
            {

                if (Pieces[King.CurrentPosision.Row-2, King.CurrentPosision.Column - 2].Color != King.Color)
                {
                    if (CheckMovmentAvailablityForCheckMate(King, new Position(King.CurrentPosision.Row - 1, King.CurrentPosision.Column - 1)))
                        AvailableMovments.Add(new Position(King.CurrentPosision.Row - 1, King.CurrentPosision.Column - 1));
                }
            }

            // Detect if king can move down right
            if ((King.CurrentPosision.Row != 1) && (King.CurrentPosision.Column != 8))
            {

                if (Pieces[King.CurrentPosision.Row-2, King.CurrentPosision.Column].Color != King.Color)
                {
                    if (CheckMovmentAvailablityForCheckMate(King, new Position(King.CurrentPosision.Row - 1, King.CurrentPosision.Column + 1)))
                        AvailableMovments.Add(new Position(King.CurrentPosision.Row - 1, King.CurrentPosision.Column + 1));
                }
            }
            return AvailableMovments;
            
        }

        private List<Position> AvailableMovmentsForKnight(Piece Knight, bool EnableCheckCheckmate=true)
        {
            List<Position> AvailableMovments = new List<Position>();
            // if moving this piece is not available because it can cause checkmate then return
            if(EnableCheckCheckmate)
                if (!(CheckMovmentAvailablityForCheckMate(Knight)))
                    return AvailableMovments;

            #region Moving throgh 2 Rows and 1 Column

            // Detect if Knight can move up left and then detect if the piece color differ from current color then Add it as available
            if ((Knight.CurrentPosision.Row <= 6) && (Knight.CurrentPosision.Column >= 2))
            {
                if ((Pieces[Knight.CurrentPosision.Row + 1, Knight.CurrentPosision.Column - 2].Color != Knight.Color))
                    AvailableMovments.Add(new Position(Knight.CurrentPosision.Row+2,Knight.CurrentPosision.Column-1));
            }

            // Detect if Knight can move up right and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row <= 6) && (Knight.CurrentPosision.Column <= 7))
            {
                if ((Pieces[Knight.CurrentPosision.Row + 1, Knight.CurrentPosision.Column].Color != Knight.Color))
                    AvailableMovments.Add(new Position(Knight.CurrentPosision.Row+2,Knight.CurrentPosision.Column+1));
            }


            // Detect if Knight can move down left and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row >= 3) && (Knight.CurrentPosision.Column >= 2))
            {
                if ((Pieces[Knight.CurrentPosision.Row - 3, Knight.CurrentPosision.Column - 2].Color != Knight.Color))
                    AvailableMovments.Add(new Position(Knight.CurrentPosision.Row-2,Knight.CurrentPosision.Column-1));
            }

            // Detect if Knight can move down right and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row >= 3) && (Knight.CurrentPosision.Column <= 7))
            {
                if ((Pieces[Knight.CurrentPosision.Row - 3, Knight.CurrentPosision.Column].Color != Knight.Color))
                    AvailableMovments.Add(new Position(Knight.CurrentPosision.Row -2, Knight.CurrentPosision.Column + 1));
            }

            #endregion

            #region Moving throgh 1 Row and 2 Columns

            // Detect if Knight can move up left and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row <= 7) && (Knight.CurrentPosision.Column >= 3))
            {
                if ((Pieces[Knight.CurrentPosision.Row, Knight.CurrentPosision.Column - 3].Color != Knight.Color))
                    AvailableMovments.Add(new Position(Knight.CurrentPosision.Row + 1, Knight.CurrentPosision.Column - 2));
            }

            // Detect if Knight can move up right and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row <= 7) && (Knight.CurrentPosision.Column <= 6))
            {
                if ((Pieces[Knight.CurrentPosision.Row, Knight.CurrentPosision.Column + 1].Color != Knight.Color))
                    AvailableMovments.Add(new Position(Knight.CurrentPosision.Row +1, Knight.CurrentPosision.Column +2));
            }


            // Detect if Knight can move down left and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row >= 2) && (Knight.CurrentPosision.Column >= 3))
            {
                if ((Pieces[Knight.CurrentPosision.Row - 2, Knight.CurrentPosision.Column - 3].Color != Knight.Color))
                    AvailableMovments.Add(new Position(Knight.CurrentPosision.Row - 1, Knight.CurrentPosision.Column - 2));
            }

            // Detect if Knight can move down right and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row >= 2) && (Knight.CurrentPosision.Column <= 6))
            {
                if ((Pieces[Knight.CurrentPosision.Row - 2, Knight.CurrentPosision.Column].Color != Knight.Color))
                    AvailableMovments.Add(new Position(Knight.CurrentPosision.Row - 1, Knight.CurrentPosision.Column + 2));
            }

            #endregion

            return AvailableMovments;
        }

        private List<Position> AvailableMovmentsForBishp(Piece Queen, bool EnableCheckCheckmate = true)
        {
            bool BreakAllLoops = false;
            int Counter = 0;
            List<Position> AvailableMovments = new List<Position>();
            // if moving this piece is not available because it can cause checkmate then return
            if(EnableCheckCheckmate)
                if (!(CheckMovmentAvailablityForCheckMate(Queen)))
                    return AvailableMovments;
            #region Detect Moving to Down Left


            if ((Queen.CurrentPosision.Row > 1) && (Queen.CurrentPosision.Column > 1))
            {
                BreakAllLoops = false;
                Counter = Queen.CurrentPosision.Column - 2;
                for (int IteratedRow = Queen.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                {
                    for (int IteratedColumn =Counter; IteratedColumn >= 0; IteratedColumn--)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Pieces[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            Counter--;
                            break;
                        }

                        // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            BreakAllLoops = true;
                            break;
                        }

                        // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            BreakAllLoops = true;
                            break;
                        }

                        // if Available position and the queen in same color then break only
                        else if (((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                        {
                            BreakAllLoops = true;
                            break;
                        }

                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Down Right


            if ((Queen.CurrentPosision.Row > 1) && (Queen.CurrentPosision.Column < 8))
            {
                BreakAllLoops = false;
                Counter = Queen.CurrentPosision.Column;
                for (int IteratedRow = Queen.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                {
                    for (int IteratedColumn = Counter; IteratedColumn <= 7; IteratedColumn++)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Pieces[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            Counter++;
                            break;
                        }

                        // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            BreakAllLoops = true;
                            break;
                        }

                        // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            BreakAllLoops = true;
                            break;
                        }

                        // if Available position and the queen in same color then break only
                        else if (((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                        {
                            BreakAllLoops = true;
                            break;
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Top Right


            if ((Queen.CurrentPosision.Row < 8) && (Queen.CurrentPosision.Column < 8))
            {
                BreakAllLoops = false;
                Counter = Queen.CurrentPosision.Column;
                for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    for (int IteratedColumn = Counter; IteratedColumn <= 7; IteratedColumn++)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Pieces[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            Counter++;
                            break;
                        }

                        // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            BreakAllLoops = true;
                            break;
                        }

                        // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            BreakAllLoops = true;
                            break;
                        }

                        // if Available position and the queen in same color then break only
                        else if (((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                        {
                            BreakAllLoops = true;
                            break;
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Top Left

            if ((Queen.CurrentPosision.Row < 8) && (Queen.CurrentPosision.Column > 1))
            {
                BreakAllLoops = false;
                Counter=Queen.CurrentPosision.Column - 2;
                for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    for (int IteratedColumn = Counter ; IteratedColumn >= 0; IteratedColumn--)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Pieces[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            Counter--;
                            break;
                        }

                        // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            BreakAllLoops = true;
                            break;
                        }

                        // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                            BreakAllLoops = true;
                            break;
                        }

                        // if Available position and the queen in same color then break only
                        else if (((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                        {
                            BreakAllLoops = true;
                            break;
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion
            return AvailableMovments;
        }

        private List<Position> AvailableMovmentsForRook(Piece Queen, bool EnableCheckCheckmate = true)
        {
            List<Position> AvailableMovments = new List<Position>();
            // if moving this piece is not available because it can cause checkmate then return
            if(EnableCheckCheckmate)
                if (!(CheckMovmentAvailablityForCheckMate(Queen)))
                    return AvailableMovments;
            #region Detect Moving to Left
            if (Queen.CurrentPosision.Column > 1)
            {
                for (int IteratedColumn = Queen.CurrentPosision.Column - 2; IteratedColumn >= 0; IteratedColumn--)
                {
                    // if Availabe Posision is empty
                    if (Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.Empty)
                    {
                        AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn + 1));
                        continue;
                    }


                    // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                    else if ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                    {
                        AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn + 1));
                        break;
                    }

                    // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                    else if ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                    {
                        AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn + 1));
                        break;
                    }

                    // if Available position and the queen in same color then break only
                    else if (((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                    {
                        break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Right
            if (Queen.CurrentPosision.Column < 8)
            {
                for (int IteratedColumn = Queen.CurrentPosision.Column; IteratedColumn <= 7; IteratedColumn++)
                {
                    // if Availabe Posision is empty
                    if (Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.Empty)
                    {
                        AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn + 1));
                        continue;
                    }

                    // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                    else if ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                    {
                        AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn + 1));
                        break;
                    }

                    // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                    else if ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                    {
                        AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn + 1));
                        break;
                    }

                    // if Available position and the queen in same color then break only
                    else if (((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                    {
                        break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Top
            if (Queen.CurrentPosision.Row < 8)
            {
                for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    // if Availabe Posision is empty
                    if (Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Type == Enums.PieceType.Empty)
                    {
                        AvailableMovments.Add(new Position(IteratedRow + 1, Queen.CurrentPosision.Column));
                        continue;
                    }

                    // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                    else if ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                    {
                        AvailableMovments.Add(new Position(IteratedRow + 1, Queen.CurrentPosision.Column));
                        break;
                    }

                    // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                    else if ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                    {
                        AvailableMovments.Add(new Position(IteratedRow + 1, Queen.CurrentPosision.Column));
                        break;
                    }

                    // if Available position and the queen in same color then break only
                    else if (((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                    {
                        break;
                    }

                }
            }
            #endregion

            #region Detect Moving to Down
            if (Queen.CurrentPosision.Row > 1)
            {
                for (int IteratedRow = Queen.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                {
                    // if Availabe Posision is empty
                    if (Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Type == Enums.PieceType.Empty)
                    {
                        AvailableMovments.Add(new Position(IteratedRow + 1, Queen.CurrentPosision.Column));
                        continue;
                    }

                    // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                    else if ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                    {
                        AvailableMovments.Add(new Position(IteratedRow + 1, Queen.CurrentPosision.Column));
                        break;
                    }

                    // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                    else if ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                    {
                        AvailableMovments.Add(new Position(IteratedRow + 1, Queen.CurrentPosision.Column));
                        break;
                    }

                    // if Available position and the queen in same color then break only
                    else if (((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                    {
                        break;
                    }
                }
            }
            #endregion

            return AvailableMovments;
        }

        private List<Position> AvailableMovmentsForQueen(Piece Queen, bool EnableCheckCheckmate = true)
        {
            bool BreakAllLoops = false;
            int Counter = 0;
            List<Position> AvailableMovments=new List<Position>();
            // if moving this piece is not available because it can cause checkmate then return
            if(EnableCheckCheckmate)
                if(!(CheckMovmentAvailablityForCheckMate(Queen)))
                    return AvailableMovments;
            
                #region Detect Moving to Left
                if (Queen.CurrentPosision.Column > 1)
                {
                    for (int IteratedColumn = Queen.CurrentPosision.Column - 2; IteratedColumn >= 0; IteratedColumn--)
                    {
                        // if Availabe Posision is empty
                        if (Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                           AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn+1));
                           continue;
                        }
                            

                        // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                        else if ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                        {
                            AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn + 1));
                            break;
                        }

                        // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                        else if ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                        {
                            AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn + 1));
                            break;
                        }

                        // if Available position and the queen in same color then break only
                        else if (((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                        {
                            break;
                        }
                    }
                }
                #endregion

                #region Detect Moving to Right
                if (Queen.CurrentPosision.Column < 8)
                {
                    for (int IteratedColumn = Queen.CurrentPosision.Column; IteratedColumn <= 7; IteratedColumn++)
                    {
                        // if Availabe Posision is empty
                        if (Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn + 1));
                            continue;
                        }

                        // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                        else if ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                        {
                            AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn + 1));
                            break;
                        }

                        // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                        else if ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                        {
                            AvailableMovments.Add(new Position(Queen.CurrentPosision.Row, IteratedColumn + 1));
                            break;
                        }

                        // if Available position and the queen in same color then break only
                        else if (((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                        {
                            break;
                        }
                    }
                }
                #endregion

                #region Detect Moving to Top
                if (Queen.CurrentPosision.Row < 8)
                {
                    for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                    {
                        // if Availabe Posision is empty
                        if (Pieces[IteratedRow,Queen.CurrentPosision.Column-1].Type == Enums.PieceType.Empty)
                        {
                            AvailableMovments.Add(new Position(IteratedRow+1, Queen.CurrentPosision.Column));
                            continue;
                        }

                        // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, Queen.CurrentPosision.Column));
                            break;
                        }

                        // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, Queen.CurrentPosision.Column));
                            break;
                        }

                        // if Available position and the queen in same color then break only
                        else if (((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                        {
                            break;
                        }
                        
                    }
                }
                #endregion

                #region Detect Moving to Down
                if (Queen.CurrentPosision.Row > 1)
                {
                    for (int IteratedRow = Queen.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                    {
                        // if Availabe Posision is empty
                        if (Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Type == Enums.PieceType.Empty)
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, Queen.CurrentPosision.Column));
                            continue;
                        }

                        // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, Queen.CurrentPosision.Column));
                            break;
                        }

                        // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                        else if ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                        {
                            AvailableMovments.Add(new Position(IteratedRow + 1, Queen.CurrentPosision.Column));
                            break;
                        }

                        // if Available position and the queen in same color then break only
                        else if (((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                        {
                            break;
                        }
                    }
                }
                #endregion

                #region Detect Moving to Down Left


                if ((Queen.CurrentPosision.Row > 1) && (Queen.CurrentPosision.Column > 1))
                {
                    BreakAllLoops = false;
                    Counter = Queen.CurrentPosision.Column - 2;
                    for (int IteratedRow = Queen.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                    {
                        for (int IteratedColumn = Counter; IteratedColumn >= 0; IteratedColumn--)
                        {
                            // if Availabe Posision is empty then break interion loop and continue row loop 
                            if (Pieces[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn+1));
                                Counter--;
                                break;
                            }

                            // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                            else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                                BreakAllLoops = true;
                                break;
                            }

                            // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                            else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                                BreakAllLoops = true;
                                break;
                            }

                            // if Available position and the queen in same color then break only
                            else if (((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                            {
                                BreakAllLoops = true;
                                break;
                            }

                        }
                        if (BreakAllLoops)
                            break;
                    }
                }
                #endregion

                #region Detect Moving to Down Right


                if ((Queen.CurrentPosision.Row > 1) && (Queen.CurrentPosision.Column < 8))
                {
                    BreakAllLoops = false;
                    Counter = Queen.CurrentPosision.Column;
                    for (int IteratedRow = Queen.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                    {
                        for (int IteratedColumn = Counter; IteratedColumn <= 7; IteratedColumn++)
                        {
                            // if Availabe Posision is empty then break interion loop and continue row loop 
                            if (Pieces[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                                Counter++;
                                break;
                            }

                            // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                            else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                                BreakAllLoops = true;
                                break;
                            }

                            // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                            else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                                BreakAllLoops = true;
                                break;
                            }

                            // if Available position and the queen in same color then break only
                            else if (((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                        if (BreakAllLoops)
                            break;
                    }
                }
                #endregion

                #region Detect Moving to Top Right


                if ((Queen.CurrentPosision.Row < 8) && (Queen.CurrentPosision.Column < 8))
                {
                    BreakAllLoops = false;
                    Counter = Queen.CurrentPosision.Column;
                    for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                    {
                        for (int IteratedColumn = Counter; IteratedColumn <= 7; IteratedColumn++)
                        {
                            // if Availabe Posision is empty then break interion loop and continue row loop 
                            if (Pieces[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                                Counter++;
                                break;
                            }

                            // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                            else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                                BreakAllLoops = true;
                                break;
                            }

                            // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                            else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                                BreakAllLoops = true;
                                break;
                            }

                            // if Available position and the queen in same color then break only
                            else if (((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                        if (BreakAllLoops)
                            break;
                    }
                }
                #endregion

                #region Detect Moving to Top Left

                if ((Queen.CurrentPosision.Row < 8) && (Queen.CurrentPosision.Column > 1))
                {
                    BreakAllLoops = false;
                    Counter = Queen.CurrentPosision.Column - 2;
                    for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                    {
                        for (int IteratedColumn = Counter; IteratedColumn >= 0; IteratedColumn--)
                        {
                            // if Availabe Posision is empty then break interion loop and continue row loop 
                            if (Pieces[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                                Counter--;
                                break;
                            }

                            // if Availabe Position is to black and  current queen is white then  available movment stop here for moving to left
                            else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.White))
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                                BreakAllLoops = true;
                                break;
                            }

                            // if Availabe Position is to white and  current queen is black then  available movment stop here for moving to left
                            else if ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.Black))
                            {
                                AvailableMovments.Add(new Position(IteratedRow + 1, IteratedColumn + 1));
                                BreakAllLoops = true;
                                break;
                            }

                            // if Available position and the queen in same color then break only
                            else if (((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White) && (Queen.Color == Enums.PieceColor.White)) || ((Pieces[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black) && (Queen.Color == Enums.PieceColor.Black)))
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                        if (BreakAllLoops)
                            break;
                    }
                }
                #endregion

                return AvailableMovments;

        }

        private List<Position> AvailableMovmentsForPawn(Piece Pawn, bool EnableCheckCheckmate = true)
        {
            List<Position> AvailableMovments=new List<Position>();
            // if moving this piece is not available because it can cause checkmate then return
            if(EnableCheckCheckmate)
                if (!(CheckMovmentAvailablityForCheckMate(Pawn)))
                    return AvailableMovments;

            if((Pawn.Color== Enums.PieceColor.White) && (Pawn.CurrentPosision.Row!=8))
            {


                // Detect if pawn can move up
                // if there is nothing above the pawn then verify that this moving cannot make checkmate for white king :
                if(Pieces[Pawn.CurrentPosision.Row,Pawn.CurrentPosision.Column-1].Type == Enums.PieceType.Empty)
                {
                        AvailableMovments.Add(new Position(Pawn.CurrentPosision.Row+1,Pawn.CurrentPosision.Column));
                        // if this pawn has not moved yet  then it can move up 2 pieces
                        if (Pawn.CurrentPosision.Row == 2)
                        {
                            if (Pieces[Pawn.CurrentPosision.Row+1, Pawn.CurrentPosision.Column - 1].Type == Enums.PieceType.Empty)
                                AvailableMovments.Add(new Position(4, Pawn.CurrentPosision.Column));
                        }

                }
                // Detect if pawn can move up left
                // if pawn not in column 1 then verify if there is an opposite piece then detect availability for moving
                if(Pawn.CurrentPosision.Column!=1)
                {
                    if(Pieces[Pawn.CurrentPosision.Row,Pawn.CurrentPosision.Column-2].Color== Enums.PieceColor.Black)
                    {
                            AvailableMovments.Add(new Position(Pawn.CurrentPosision.Row+1,Pawn.CurrentPosision.Column-1));
                    }
                }

                // Detect if pawn can move up right
                // if pawn not in column 8 then verify if there is an opposite piece then detect availability for moving
                if (Pawn.CurrentPosision.Column != 8)
                {
                    if (Pieces[Pawn.CurrentPosision.Row, Pawn.CurrentPosision.Column].Color == Enums.PieceColor.Black)
                    {
                            AvailableMovments.Add(new Position(Pawn.CurrentPosision.Row + 1, Pawn.CurrentPosision.Column + 1));
                    }
                }

 
            }
            else if((Pawn.Color== Enums.PieceColor.Black)  && (Pawn.CurrentPosision.Row!=1))
            {
                // Detect if pawn can move down
                // if there is nothing down the pawn then verify that this moving cannot make checkmate for white king :
                if (Pieces[Pawn.CurrentPosision.Row-2, Pawn.CurrentPosision.Column - 1].Type == Enums.PieceType.Empty)
                {
                        AvailableMovments.Add(new Position(Pawn.CurrentPosision.Row - 1, Pawn.CurrentPosision.Column));
                        // if this pawn has not moved yet  then it can move down 2 pieces
                        if (Pawn.CurrentPosision.Row == 7)
                        {
                            if (Pieces[Pawn.CurrentPosision.Row -3, Pawn.CurrentPosision.Column - 1].Type == Enums.PieceType.Empty)
                                AvailableMovments.Add(new Position(5, Pawn.CurrentPosision.Column));
                        }
                }
                // Detect if pawn can move down left
                // if pawn not in column 1 then verify if there is an opposite piece then detect availability for moving
                if (Pawn.CurrentPosision.Column != 1)
                {
                    if (Pieces[Pawn.CurrentPosision.Row-2, Pawn.CurrentPosision.Column - 2].Color == Enums.PieceColor.White)
                    {
                            AvailableMovments.Add(new Position(Pawn.CurrentPosision.Row - 1, Pawn.CurrentPosision.Column - 1));
                    }
                }

                // Detect if pawn can move down right
                // if pawn not in column 8 then verify if there is an opposite piece then detect availability for moving
                if (Pawn.CurrentPosision.Column != 8)
                {
                    if (Pieces[Pawn.CurrentPosision.Row-2, Pawn.CurrentPosision.Column].Color == Enums.PieceColor.White)
                    {
                            AvailableMovments.Add(new Position(Pawn.CurrentPosision.Row - 1, Pawn.CurrentPosision.Column + 1));
                    }
                }

                
            }
            return AvailableMovments;
        }

        #endregion

        # region Verify CheckMate for White
        private bool IsWhiteKingCheckMate( Piece[,] Board)
        {
           
            // Get white king piece :

            //Piece King =null;
            //foreach (Piece item in Board)
            //{
            //    if ((item.Type == Enums.PieceType.King) && (item.Color == Enums.PieceColor.White))
            //        King = item;
            //}
            //if (King == null)
            //    throw new SystemException("white king does not exists in board !");

            // Browse each piece on board and detect if this piece  make check mate
            foreach (Piece item in Board)
            {
                if(item.Color== Enums.PieceColor.Black)
                {
                    switch (item.Type)
                    {
                        case Chess_Project.Buisness.Enums.PieceType.Empty:
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.King:
                            // King Cannot make direct check mate !
                            //if (AvailableTempMovmentsForBlackKing(item,Board))
                            //    return true;
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Queen:
                            if (IsCheckMateToWhiteByBlackQueen(item, Board))
                                return true;
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Rook:
                            if (IsCheckMateToWhiteByBlackRook(item, Board))
                                return true;
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Bishop:
                            if (IsCheckMateToWhiteByBlackBishop(item, Board))
                                return true;
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Pawn:
                            if (IsCheckMateToWhiteByBlackPawn(item, Board))
                                return true;
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Knight:
                            if (IsCheckMateToWhiteByBlackKnight(item, Board))
                                return true;
                            break;
                        default:
                            break;
                    }
                }
            }
            return false;
        }

        private bool IsCheckMateToWhiteByBlackKnight(Piece Knight, Piece[,] Board)
        {
            #region Moving throgh 2 Rows and 1 Column

            // Detect if Knight can move up left and then detect if the white king on that position
            if((Knight.CurrentPosision.Row<= 6) && (Knight.CurrentPosision.Column >=2 ))
            {
                if ((Board[Knight.CurrentPosision.Row + 1, Knight.CurrentPosision.Column - 2].Color == Enums.PieceColor.White) && (Board[Knight.CurrentPosision.Row + 1, Knight.CurrentPosision.Column - 2].Type == Enums.PieceType.King))
                    return true;
            }

            // Detect if Knight can move up right and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row <= 6) && (Knight.CurrentPosision.Column <=7))
            {
                if ((Board[Knight.CurrentPosision.Row + 1, Knight.CurrentPosision.Column].Color == Enums.PieceColor.White) && (Board[Knight.CurrentPosision.Row + 1, Knight.CurrentPosision.Column].Type == Enums.PieceType.King))
                    return true;
            }


            // Detect if Knight can move down left and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row >= 3) && (Knight.CurrentPosision.Column >= 2))
            {
                if ((Board[Knight.CurrentPosision.Row -3, Knight.CurrentPosision.Column - 2].Color == Enums.PieceColor.White) && (Board[Knight.CurrentPosision.Row -3, Knight.CurrentPosision.Column - 2].Type == Enums.PieceType.King))
                    return true;
            }

            // Detect if Knight can move down right and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row >= 3) && (Knight.CurrentPosision.Column <= 7))
            {
                if ((Board[Knight.CurrentPosision.Row -3, Knight.CurrentPosision.Column].Color == Enums.PieceColor.White) && (Board[Knight.CurrentPosision.Row -3, Knight.CurrentPosision.Column].Type == Enums.PieceType.King))
                    return true;
            }

            #endregion

            #region Moving throgh 1 Row and 2 Columns

            // Detect if Knight can move up left and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row <= 7) && (Knight.CurrentPosision.Column >= 3))
            {
                if ((Board[Knight.CurrentPosision.Row, Knight.CurrentPosision.Column - 3].Color == Enums.PieceColor.White) && (Board[Knight.CurrentPosision.Row, Knight.CurrentPosision.Column - 3].Type == Enums.PieceType.King))
                    return true;
            }

            // Detect if Knight can move up right and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row <= 7) && (Knight.CurrentPosision.Column <= 6))
            {
                if ((Board[Knight.CurrentPosision.Row, Knight.CurrentPosision.Column+1].Color == Enums.PieceColor.White) && (Board[Knight.CurrentPosision.Row, Knight.CurrentPosision.Column+1].Type == Enums.PieceType.King))
                    return true;
            }


            // Detect if Knight can move down left and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row >= 2) && (Knight.CurrentPosision.Column >= 3))
            {
                if ((Board[Knight.CurrentPosision.Row - 2, Knight.CurrentPosision.Column - 3].Color == Enums.PieceColor.White) && (Board[Knight.CurrentPosision.Row - 2, Knight.CurrentPosision.Column - 3].Type == Enums.PieceType.King))
                    return true;
            }

            // Detect if Knight can move down right and then detect if the white king on that position
            if ((Knight.CurrentPosision.Row >= 2) && (Knight.CurrentPosision.Column <= 6))
            {
                if ((Board[Knight.CurrentPosision.Row - 2, Knight.CurrentPosision.Column].Color == Enums.PieceColor.White) && (Board[Knight.CurrentPosision.Row - 2, Knight.CurrentPosision.Column+1].Type == Enums.PieceType.King))
                    return true;
            }

            #endregion

            return false;
        }

        private bool IsCheckMateToWhiteByBlackPawn(Piece Pawn, Piece[,] Board)
        {
            // Detect if pawn on first row 
            if (Pawn.CurrentPosision.Row == 1)
                return false;
            // Detect if current position of pawn in first column then detect if the white king on down right
            if ((Pawn.CurrentPosision.Column==1))
            {
                if ((Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column].Type == Enums.PieceType.King) && (Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column].Color == Enums.PieceColor.White))
                    return true;
                return false;
            }
            // Detect if current posision of pawn in last column then detect if the white king on down left
            else if ((Pawn.CurrentPosision.Column == 8))
            {
                if ((Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column-2].Type == Enums.PieceType.King) && (Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column-2].Color == Enums.PieceColor.White))
                    return true;
                return false;
            }
            // Detect if the white king on down left or down right
            else
            {
                // Down left
                if ((Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column - 2].Type == Enums.PieceType.King) && (Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column-2].Color == Enums.PieceColor.White))
                    return true;

                // Down Right
                if ((Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column].Type == Enums.PieceType.King) && (Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column].Color == Enums.PieceColor.White))
                    return true;
                return false;
            }
        }

        private bool IsCheckMateToWhiteByBlackBishop(Piece Bishop, Piece[,] Board)
        {
            bool BreakAllLoops = false;
            int Counter = 0;
            #region Detect Moving to Down Left

            if ((Bishop.CurrentPosision.Row > 1) && (Bishop.CurrentPosision.Column > 1))
            {
                BreakAllLoops = false;
                Counter = Bishop.CurrentPosision.Column - 2;
                for (int IteratedRow = Bishop.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                {
                    for (int IteratedColumn =Counter; IteratedColumn >= 0; IteratedColumn--)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter--;
                            break;
                        }
                            

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to white 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King) 
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Down Right


            if ((Bishop.CurrentPosision.Row > 1) && (Bishop.CurrentPosision.Column < 8))
            {
                BreakAllLoops = false;
                Counter = Bishop.CurrentPosision.Column;
                for (int IteratedRow = Bishop.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                {
                    for (int IteratedColumn = Counter; IteratedColumn <= 7; IteratedColumn++)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter++;
                            break;
                        }

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to white 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Top Right


            if ((Bishop.CurrentPosision.Row < 8) && (Bishop.CurrentPosision.Column < 8))
            {
                BreakAllLoops = false;
                Counter = Bishop.CurrentPosision.Column;
                for (int IteratedRow = Bishop.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    for (int IteratedColumn = Counter; IteratedColumn <= 7; IteratedColumn++)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter++;
                            break;
                        }

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to white 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Top Left

            if ((Bishop.CurrentPosision.Row < 8) && (Bishop.CurrentPosision.Column > 1))
            {
                BreakAllLoops = false;
                Counter = Bishop.CurrentPosision.Column - 2;
                for (int IteratedRow = Bishop.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    for (int IteratedColumn =Counter; IteratedColumn >= 0; IteratedColumn--)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter--;
                            break;
                        }

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to white 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            return false;
        }

        private bool IsCheckMateToWhiteByBlackRook(Piece Rook, Piece[,] Board)
        {
            #region Detect Moving to Left
            if (Rook.CurrentPosision.Column > 1)
            {
                for (int IteratedColumn = Rook.CurrentPosision.Column - 2; IteratedColumn >= 0; IteratedColumn--)
                {
                    // if Availabe Posision is empty
                    if (Board[Rook.CurrentPosision.Row-1, IteratedColumn].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black
                    else if (Board[Rook.CurrentPosision.Row-1, IteratedColumn].Color == Enums.PieceColor.Black)
                        break;
                    // if Available postsion is to white
                    else if (Board[Rook.CurrentPosision.Row-1, IteratedColumn].Color == Enums.PieceColor.White)
                    {
                        // Check if the item is king :
                        if (Board[Rook.CurrentPosision.Row-1, IteratedColumn].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Right
            if (Rook.CurrentPosision.Column < 8)
            {
                for (int IteratedColumn = Rook.CurrentPosision.Column; IteratedColumn <= 7; IteratedColumn++)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[Rook.CurrentPosision.Row-1, IteratedColumn].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black then break
                    else if (Board[Rook.CurrentPosision.Row-1, IteratedColumn].Color == Enums.PieceColor.Black)
                        break;
                    // if Available postsion is to white 
                    else if (Board[Rook.CurrentPosision.Row-1, IteratedColumn].Color == Enums.PieceColor.White)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[Rook.CurrentPosision.Row-1, IteratedColumn].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Top
            if (Rook.CurrentPosision.Row < 8)
            {
                for (int IteratedRow = Rook.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[IteratedRow, Rook.CurrentPosision.Column-1].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black then break
                    else if (Board[IteratedRow, Rook.CurrentPosision.Column-1].Color == Enums.PieceColor.Black)
                        break;
                    // if Available postsion is to white 
                    else if (Board[IteratedRow, Rook.CurrentPosision.Column-1].Color == Enums.PieceColor.White)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[IteratedRow, Rook.CurrentPosision.Column-1].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Down
            if (Rook.CurrentPosision.Row > 1)
            {
                for (int IteratedRow = Rook.CurrentPosision.Row-2; IteratedRow >= 0; IteratedRow--)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[IteratedRow, Rook.CurrentPosision.Column-1].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black then break
                    else if (Board[IteratedRow, Rook.CurrentPosision.Column-1].Color == Enums.PieceColor.Black)
                        break;
                    // if Available postsion is to white 
                    else if (Board[IteratedRow, Rook.CurrentPosision.Column-1].Color == Enums.PieceColor.White)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[IteratedRow, Rook.CurrentPosision.Column-1].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            return false;
        }

        private bool IsCheckMateToWhiteByBlackQueen(Piece Queen, Piece[,] Board)
        {
            bool BreakAllLoops=false;
            int Counter = 0;
            
            #region Detect Moving to Left
            if (Queen.CurrentPosision.Column > 1)
            {
                for (int IteratedColumn = Queen.CurrentPosision.Column - 2; IteratedColumn >= 0; IteratedColumn--)
                {
                    // if Availabe Posision is empty
                    if (Board[Queen.CurrentPosision.Row-1, IteratedColumn].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black
                    else if (Board[Queen.CurrentPosision.Row-1, IteratedColumn].Color == Enums.PieceColor.Black)
                        break;
                    // if Available postsion is to white
                    else if (Board[Queen.CurrentPosision.Row-1, IteratedColumn].Color == Enums.PieceColor.White)
                    {
                        // Check if the item is king :
                        if (Board[Queen.CurrentPosision.Row-1, IteratedColumn].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Right
            if (Queen.CurrentPosision.Column < 8)
            {
                for (int IteratedColumn = Queen.CurrentPosision.Column; IteratedColumn <= 7; IteratedColumn++)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[Queen.CurrentPosision.Row-1, IteratedColumn].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black then break
                    else if (Board[Queen.CurrentPosision.Row-1, IteratedColumn].Color == Enums.PieceColor.Black)
                        break;
                    // if Available postsion is to white 
                    else if (Board[Queen.CurrentPosision.Row-1, IteratedColumn].Color == Enums.PieceColor.White)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[Queen.CurrentPosision.Row-1, IteratedColumn].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Top
            if (Queen.CurrentPosision.Row < 8)
            {
                for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[IteratedRow, Queen.CurrentPosision.Column-1].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black then break
                    else if (Board[IteratedRow, Queen.CurrentPosision.Column-1].Color == Enums.PieceColor.Black)
                        break;
                    // if Available postsion is to white 
                    else if (Board[IteratedRow, Queen.CurrentPosision.Column-1].Color == Enums.PieceColor.White)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[IteratedRow, Queen.CurrentPosision.Column-1].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Down
            if (Queen.CurrentPosision.Row > 1)
            {
                for (int IteratedRow = Queen.CurrentPosision.Row-2; IteratedRow >= 0; IteratedRow--)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[IteratedRow, Queen.CurrentPosision.Column-1].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black then break
                    else if (Board[IteratedRow, Queen.CurrentPosision.Column-1].Color == Enums.PieceColor.Black)
                        break;
                    // if Available postsion is to white 
                    else if (Board[IteratedRow, Queen.CurrentPosision.Column-1].Color == Enums.PieceColor.White)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[IteratedRow, Queen.CurrentPosision.Column-1].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Down Left


            if ((Queen.CurrentPosision.Row > 1) && (Queen.CurrentPosision.Column>1))
            {
                BreakAllLoops=false;
                Counter = Queen.CurrentPosision.Column - 2;
                for (int IteratedRow = Queen.CurrentPosision.Row-2; IteratedRow >= 0; IteratedRow--)
                {
                    for (int IteratedColumn = Counter; IteratedColumn >= 0; IteratedColumn--)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter--;
                            break;
                        }

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow,IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            BreakAllLoops=true;
                            break;
                        }
                            

                        // if Available postsion is to white 
                        else if (Board[IteratedRow,IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow,IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops=true;
                                break;
                            }
                        }
                    }
                    if(BreakAllLoops)
                        break;
                }
            }
        #endregion

            #region Detect Moving to Down Right


            if ((Queen.CurrentPosision.Row > 1) && (Queen.CurrentPosision.Column < 8))
            {
                BreakAllLoops=false;
                Counter = Queen.CurrentPosision.Column;
                for (int IteratedRow = Queen.CurrentPosision.Row-2; IteratedRow >= 0; IteratedRow--)
                {
                    for (int IteratedColumn =Counter; IteratedColumn <= 7; IteratedColumn++)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter++;
                            break;
                        }

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow,IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            BreakAllLoops=true;
                            break;
                        }
                            

                        // if Available postsion is to white 
                        else if (Board[IteratedRow,IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow,IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops=true;
                                break;
                            }
                        }
                    }
                    if(BreakAllLoops)
                        break;
                }
            }
        #endregion

            #region Detect Moving to Top Right


            if ((Queen.CurrentPosision.Row < 8) && (Queen.CurrentPosision.Column < 8))
            {
                BreakAllLoops=false;
                Counter = Queen.CurrentPosision.Column;
                for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    for (int IteratedColumn = Counter; IteratedColumn <= 7; IteratedColumn++)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter++;
                            break;
                        }

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow,IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            BreakAllLoops=true;
                            break;
                        }
                            

                        // if Available postsion is to white 
                        else if (Board[IteratedRow,IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow,IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops=true;
                                break;
                            }
                        }
                    }
                    if(BreakAllLoops)
                        break;
                }
            }
        #endregion

            #region Detect Moving to Top Left

            if ((Queen.CurrentPosision.Row < 8) && (Queen.CurrentPosision.Column > 1))
            {
                BreakAllLoops=false;
                Counter = Queen.CurrentPosision.Column - 2;
                for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    for (int IteratedColumn = Counter; IteratedColumn >= 0; IteratedColumn--)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter--;
                            break;
                        }

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow,IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            BreakAllLoops=true;
                            break;
                        }
                            

                        // if Available postsion is to white 
                        else if (Board[IteratedRow,IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow,IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops=true;
                                break;
                            }
                        }
                    }
                    if(BreakAllLoops)
                        break;
                }
            }
        #endregion
 
            return false;
        }

        #endregion

        # region Verify CheckMate for Black
        private bool IsBlackKingCheckMate(Piece[,] Board)
        {
            
            // Get Black king piece :

            //Piece King =null;
            //foreach (Piece item in Board)
            //{
            //    if ((item.Type == Enums.PieceType.King) && (item.Color == Enums.PieceColor.Black))
            //        King = item;
            //}
            //if (King == null)
            //    throw new SystemException("Black king does not exists in board !");

            // Browse each piece on board and detect if this piece  make check mate
            foreach (Piece item in Board)
            {
                if (item.Color == Enums.PieceColor.White)
                {
                    switch (item.Type)
                    {
                        case Chess_Project.Buisness.Enums.PieceType.Empty:
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.King:
                            // King Cannot make direct check mate !
                            //if (AvailableTempMovmentsForBlackKing(item,Board))
                            //    return true;
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Queen:
                            if (IsCheckMateToBlackByWhiteQueen(item, Board))
                                return true;
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Rook:
                            if (IsCheckMateToBlackByWhiteRook(item, Board))
                                return true;
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Bishop:
                            if (IsCheckMateToBlackByWhiteBishop(item, Board))
                                return true;
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Pawn:
                            if (IsCheckMateToBlackByWhitePawn(item, Board))
                                return true;
                            break;
                        case Chess_Project.Buisness.Enums.PieceType.Knight:
                            if (IsCheckMateToBlackByWhiteKnight(item, Board))
                                return true;
                            break;
                        default:
                            break;
                    }
                }
            }
            return false;
        }

        private bool IsCheckMateToBlackByWhiteKnight(Piece Knight, Piece[,] Board)
        {
            #region Moving throgh 2 Rows and 1 Column

            // Detect if Knight can move up left and then detect if the Black king on that position
            if ((Knight.CurrentPosision.Row <= 6) && (Knight.CurrentPosision.Column >= 2))
            {
                if ((Board[Knight.CurrentPosision.Row + 1, Knight.CurrentPosision.Column - 2].Color == Enums.PieceColor.Black) && (Board[Knight.CurrentPosision.Row + 1, Knight.CurrentPosision.Column - 2].Type == Enums.PieceType.King))
                    return true;
            }

            // Detect if Knight can move up right and then detect if the Black king on that position
            if ((Knight.CurrentPosision.Row <= 6) && (Knight.CurrentPosision.Column <= 7))
            {
                if ((Board[Knight.CurrentPosision.Row + 1, Knight.CurrentPosision.Column].Color == Enums.PieceColor.Black) && (Board[Knight.CurrentPosision.Row + 1, Knight.CurrentPosision.Column].Type == Enums.PieceType.King))
                    return true;
            }


            // Detect if Knight can move down left and then detect if the Black king on that position
            if ((Knight.CurrentPosision.Row >= 3) && (Knight.CurrentPosision.Column >= 2))
            {
                if ((Board[Knight.CurrentPosision.Row - 3, Knight.CurrentPosision.Column - 2].Color == Enums.PieceColor.Black) && (Board[Knight.CurrentPosision.Row - 3, Knight.CurrentPosision.Column - 2].Type == Enums.PieceType.King))
                    return true;
            }

            // Detect if Knight can move down right and then detect if the Black king on that position
            if ((Knight.CurrentPosision.Row >= 3) && (Knight.CurrentPosision.Column <= 7))
            {
                if ((Board[Knight.CurrentPosision.Row - 3, Knight.CurrentPosision.Column].Color == Enums.PieceColor.Black) && (Board[Knight.CurrentPosision.Row - 3, Knight.CurrentPosision.Column].Type == Enums.PieceType.King))
                    return true;
            }

            #endregion

            #region Moving throgh 1 Row and 2 Columns

            // Detect if Knight can move up left and then detect if the Black king on that position
            if ((Knight.CurrentPosision.Row <= 7) && (Knight.CurrentPosision.Column >= 3))
            {
                if ((Board[Knight.CurrentPosision.Row, Knight.CurrentPosision.Column - 3].Color == Enums.PieceColor.Black) && (Board[Knight.CurrentPosision.Row, Knight.CurrentPosision.Column - 3].Type == Enums.PieceType.King))
                    return true;
            }

            // Detect if Knight can move up right and then detect if the Black king on that position
            if ((Knight.CurrentPosision.Row <= 7) && (Knight.CurrentPosision.Column <= 6))
            {
                if ((Board[Knight.CurrentPosision.Row, Knight.CurrentPosision.Column + 1].Color == Enums.PieceColor.Black) && (Board[Knight.CurrentPosision.Row, Knight.CurrentPosision.Column + 1].Type == Enums.PieceType.King))
                    return true;
            }


            // Detect if Knight can move down left and then detect if the Black king on that position
            if ((Knight.CurrentPosision.Row >= 2) && (Knight.CurrentPosision.Column >= 3))
            {
                if ((Board[Knight.CurrentPosision.Row - 2, Knight.CurrentPosision.Column - 3].Color == Enums.PieceColor.Black) && (Board[Knight.CurrentPosision.Row - 2, Knight.CurrentPosision.Column - 3].Type == Enums.PieceType.King))
                    return true;
            }

            // Detect if Knight can move down right and then detect if the Black king on that position
            if ((Knight.CurrentPosision.Row >= 2) && (Knight.CurrentPosision.Column <= 6))
            {
                if ((Board[Knight.CurrentPosision.Row - 2, Knight.CurrentPosision.Column].Color == Enums.PieceColor.Black) && (Board[Knight.CurrentPosision.Row - 2, Knight.CurrentPosision.Column + 1].Type == Enums.PieceType.King))
                    return true;
            }

            #endregion

            return false;
        }

        private bool IsCheckMateToBlackByWhitePawn(Piece Pawn, Piece[,] Board)
        {
            // Detect if pawn on first row 
            if (Pawn.CurrentPosision.Row == 1)
                return false;
            // Detect if current position of pawn in first column then detect if the Black king on down right
            if ((Pawn.CurrentPosision.Column == 1))
            {
                if ((Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column].Type == Enums.PieceType.King) && (Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column].Color == Enums.PieceColor.Black))
                    return true;
                return false;
            }
            // Detect if current posision of pawn in last column then detect if the Black king on down left
            else if ((Pawn.CurrentPosision.Column == 8))
            {
                if ((Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column - 2].Type == Enums.PieceType.King) && (Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column - 2].Color == Enums.PieceColor.Black))
                    return true;
                return false;
            }
            // Detect if the Black king on down left or down right
            else
            {
                // Down left
                if ((Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column - 2].Type == Enums.PieceType.King) && (Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column - 2].Color == Enums.PieceColor.Black))
                    return true;

                // Down Right
                if ((Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column].Type == Enums.PieceType.King) && (Board[Pawn.CurrentPosision.Row - 2, Pawn.CurrentPosision.Column].Color == Enums.PieceColor.Black))
                    return true;
                return false;
            }
        }

        private bool IsCheckMateToBlackByWhiteBishop(Piece Bishop, Piece[,] Board)
        {
            bool BreakAllLoops = false;
            int Counter = 0;
            #region Detect Moving to Down Left

            if ((Bishop.CurrentPosision.Row > 1) && (Bishop.CurrentPosision.Column > 1))
            {
                BreakAllLoops = false;
                Counter = Bishop.CurrentPosision.Column - 2;
                for (int IteratedRow = Bishop.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                {
                    for (int IteratedColumn = Counter; IteratedColumn >= 0; IteratedColumn--)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter--;
                            break;
                        }

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to Black 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Down Right


            if ((Bishop.CurrentPosision.Row > 1) && (Bishop.CurrentPosision.Column < 8))
            {
                BreakAllLoops = false;
                Counter = Bishop.CurrentPosision.Column;
                for (int IteratedRow = Bishop.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                {
                    for (int IteratedColumn = Counter; IteratedColumn <= 7; IteratedColumn++)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter++;
                            break;
                        }

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to Black 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Top Right


            if ((Bishop.CurrentPosision.Row < 8) && (Bishop.CurrentPosision.Column < 8))
            {
                BreakAllLoops = false;
                Counter = Bishop.CurrentPosision.Column;
                for (int IteratedRow = Bishop.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    for (int IteratedColumn = Counter; IteratedColumn <= 7; IteratedColumn++)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter++;
                            break;
                        }

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to Black 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Top Left

            if ((Bishop.CurrentPosision.Row < 8) && (Bishop.CurrentPosision.Column > 1))
            {
                BreakAllLoops = false;
                Counter = Bishop.CurrentPosision.Column - 2;
                for (int IteratedRow = Bishop.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    for (int IteratedColumn = Counter; IteratedColumn >= 0; IteratedColumn--)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter--;
                            break;
                        }

                        // if Availabe Position is to black then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to Black 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            return false;
        }

        private bool IsCheckMateToBlackByWhiteRook(Piece Rook, Piece[,] Board)
        {
            #region Detect Moving to Left
            if (Rook.CurrentPosision.Column > 1)
            {
                for (int IteratedColumn = Rook.CurrentPosision.Column - 2; IteratedColumn >= 0; IteratedColumn--)
                {
                    // if Availabe Posision is empty
                    if (Board[Rook.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black
                    else if (Board[Rook.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White)
                        break;
                    // if Available postsion is to Black
                    else if (Board[Rook.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black)
                    {
                        // Check if the item is king :
                        if (Board[Rook.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Right
            if (Rook.CurrentPosision.Column < 8)
            {
                for (int IteratedColumn = Rook.CurrentPosision.Column; IteratedColumn <= 7; IteratedColumn++)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[Rook.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black then break
                    else if (Board[Rook.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White)
                        break;
                    // if Available postsion is to Black 
                    else if (Board[Rook.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[Rook.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Top
            if (Rook.CurrentPosision.Row < 8)
            {
                for (int IteratedRow = Rook.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[IteratedRow, Rook.CurrentPosision.Column - 1].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black then break
                    else if (Board[IteratedRow, Rook.CurrentPosision.Column - 1].Color == Enums.PieceColor.White)
                        break;
                    // if Available postsion is to Black 
                    else if (Board[IteratedRow, Rook.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[IteratedRow, Rook.CurrentPosision.Column - 1].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Down
            if (Rook.CurrentPosision.Row > 1)
            {
                for (int IteratedRow = Rook.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[IteratedRow, Rook.CurrentPosision.Column - 1].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to black then break
                    else if (Board[IteratedRow, Rook.CurrentPosision.Column - 1].Color == Enums.PieceColor.White)
                        break;
                    // if Available postsion is to Black 
                    else if (Board[IteratedRow, Rook.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[IteratedRow, Rook.CurrentPosision.Column - 1].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            return false;
        }

        private bool IsCheckMateToBlackByWhiteQueen(Piece Queen, Piece[,] Board)
        {
            bool BreakAllLoops = false;
            int Counter = 0;

            #region Detect Moving to Left
            if (Queen.CurrentPosision.Column > 1)
            {
                for (int IteratedColumn = Queen.CurrentPosision.Column - 2; IteratedColumn >= 0; IteratedColumn--)
                {
                    // if Availabe Posision is empty
                    if (Board[Queen.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to white
                    else if (Board[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White)
                        break;
                    // if Available postsion is to Black
                    else if (Board[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black)
                    {
                        // Check if the item is king :
                        if (Board[Queen.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Right
            if (Queen.CurrentPosision.Column < 8)
            {
                for (int IteratedColumn = Queen.CurrentPosision.Column; IteratedColumn <= 7; IteratedColumn++)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[Queen.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to white then break
                    else if (Board[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.White)
                        break;
                    // if Available postsion is to Black 
                    else if (Board[Queen.CurrentPosision.Row - 1, IteratedColumn].Color == Enums.PieceColor.Black)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[Queen.CurrentPosision.Row - 1, IteratedColumn].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Top
            if (Queen.CurrentPosision.Row < 8)
            {
                for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[IteratedRow, Queen.CurrentPosision.Column - 1].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to white then break
                    else if (Board[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.White)
                        break;
                    // if Available postsion is to Black 
                    else if (Board[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[IteratedRow, Queen.CurrentPosision.Column - 1].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Down
            if (Queen.CurrentPosision.Row > 1)
            {
                for (int IteratedRow = Queen.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                {
                    // if Availabe Posision is empty then continue
                    if (Board[IteratedRow, Queen.CurrentPosision.Column - 1].Type == Enums.PieceType.Empty)
                        continue;

                    // if Availabe Position is to white then break
                    else if (Board[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.White)
                        break;
                    // if Available postsion is to Black 
                    else if (Board[IteratedRow, Queen.CurrentPosision.Column - 1].Color == Enums.PieceColor.Black)
                    {
                        // Check if the item is king then there is CheckMate
                        if (Board[IteratedRow, Queen.CurrentPosision.Column - 1].Type == Enums.PieceType.King)
                            return true;
                        // If the item is not king then stop looping.
                        else
                            break;
                    }
                }
            }
            #endregion

            #region Detect Moving to Down Left


            if ((Queen.CurrentPosision.Row > 1) && (Queen.CurrentPosision.Column > 1))
            {
                BreakAllLoops = false;
                Counter = Queen.CurrentPosision.Column - 2;
                for (int IteratedRow = Queen.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                {
                    for (int IteratedColumn =Counter; IteratedColumn >= 0; IteratedColumn--)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter--;
                            break;
                        }

                        // if Availabe Position is to white then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to Black 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Down Right


            if ((Queen.CurrentPosision.Row > 1) && (Queen.CurrentPosision.Column < 8))
            {
                BreakAllLoops = false;
                Counter = Queen.CurrentPosision.Column;
                for (int IteratedRow = Queen.CurrentPosision.Row - 2; IteratedRow >= 0; IteratedRow--)
                {
                    for (int IteratedColumn = Counter; IteratedColumn <= 7; IteratedColumn++)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter++;
                            break;
                        }

                        // if Availabe Position is to white then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to Black 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Top Right


            if ((Queen.CurrentPosision.Row < 8) && (Queen.CurrentPosision.Column < 8))
            {
                BreakAllLoops = false;
                Counter = Queen.CurrentPosision.Column;
                for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    for (int IteratedColumn = Counter; IteratedColumn <= 7; IteratedColumn++)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter++;
                            break;
                        }

                        // if Availabe Position is to white then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to Black 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            #region Detect Moving to Top Left

            if ((Queen.CurrentPosision.Row < 8) && (Queen.CurrentPosision.Column > 1))
            {
                BreakAllLoops = false;
                Counter = Queen.CurrentPosision.Column - 2;
                for (int IteratedRow = Queen.CurrentPosision.Row; IteratedRow <= 7; IteratedRow++)
                {
                    for (int IteratedColumn = Counter; IteratedColumn >= 0; IteratedColumn--)
                    {
                        // if Availabe Posision is empty then break interion loop and continue row loop 
                        if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.Empty)
                        {
                            Counter--;
                            break;
                        }

                        // if Availabe Position is to white then break all loops
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.White)
                        {
                            BreakAllLoops = true;
                            break;
                        }


                        // if Available postsion is to Black 
                        else if (Board[IteratedRow, IteratedColumn].Color == Enums.PieceColor.Black)
                        {
                            // Check if the item is king then there is CheckMate
                            if (Board[IteratedRow, IteratedColumn].Type == Enums.PieceType.King)
                                return true;
                            // If the item is not king then stop looping.
                            else
                            {
                                BreakAllLoops = true;
                                break;
                            }
                        }
                    }
                    if (BreakAllLoops)
                        break;
                }
            }
            #endregion

            return false;
        }

        #endregion

    }
}
