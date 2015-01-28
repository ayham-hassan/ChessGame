using Chess_Project.Buisness.Models;
using Chess_Project.Buisness.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chess_Project.Buisness.Enums;

namespace Chess_Project
{
    enum Turn
    {
        Black,White
    };
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Application.ApplicationExit += Application_ApplicationExit;
        }

        
        void Board_Winner(Buisness.Enums.PieceColor winner)
        {
            string text= String.Empty;
            if(winner== Buisness.Enums.PieceColor.White)
                text+="White is winner";
            else
                text+="Black is winner";
            MessageBox.Show(text);
        }

        void Board_Checkmate(Buisness.Enums.PieceColor color, List<Piece> AvailablePiecesMovments = null)
        {
            MessageBox.Show("CheckMate!");
        }
        Turn CurrentTurn;
        PictureBox firstClicked = null;
        PictureBox secondClicked = null;
        List<PictureBox> AvailablePiecesToMove = new List<PictureBox>();
        ChessBoard Board = new ChessBoard();
        
        private void InitializeBoard()
        {
            // Black Pieces : 

            (BoardLayoutPanel.GetControlFromPosition(0, 0) as PictureBox).Image = Properties.Resources.BlackRook;
            (BoardLayoutPanel.GetControlFromPosition(1, 0) as PictureBox).Image = Properties.Resources.BlackKnight;
            (BoardLayoutPanel.GetControlFromPosition(2, 0) as PictureBox).Image = Properties.Resources.BlackBishop;
            (BoardLayoutPanel.GetControlFromPosition(3, 0) as PictureBox).Image = Properties.Resources.BlackKing;
            (BoardLayoutPanel.GetControlFromPosition(4, 0) as PictureBox).Image = Properties.Resources.BlackQueen;
            (BoardLayoutPanel.GetControlFromPosition(5, 0) as PictureBox).Image = Properties.Resources.BlackBishop;
            (BoardLayoutPanel.GetControlFromPosition(6, 0) as PictureBox).Image = Properties.Resources.BlackKnight;
            (BoardLayoutPanel.GetControlFromPosition(7, 0) as PictureBox).Image = Properties.Resources.BlackRook;
            for (int column = 0; column < 8; column++)
            {
                (BoardLayoutPanel.GetControlFromPosition(column, 1) as PictureBox).Image = Properties.Resources.BlackPawn;
            }

            // Empty Pieces : 
            for (int column = 0; column < 8; column++)
            {
                for (int row = 2; row < 5; row++)
                {
                    (BoardLayoutPanel.GetControlFromPosition(column,row) as PictureBox).Image = null;
                }
            }

            // White Pieces :
            (BoardLayoutPanel.GetControlFromPosition(0, 7) as PictureBox).Image = Properties.Resources.WhiteRook;
            (BoardLayoutPanel.GetControlFromPosition(1, 7) as PictureBox).Image = Properties.Resources.WhiteKinght;
            (BoardLayoutPanel.GetControlFromPosition(2, 7) as PictureBox).Image = Properties.Resources.WhiteBishop;
            (BoardLayoutPanel.GetControlFromPosition(3, 7) as PictureBox).Image = Properties.Resources.WhiteKing;
            (BoardLayoutPanel.GetControlFromPosition(4, 7) as PictureBox).Image = Properties.Resources.WhiteQueen;
            (BoardLayoutPanel.GetControlFromPosition(5, 7) as PictureBox).Image = Properties.Resources.WhiteBishop;
            (BoardLayoutPanel.GetControlFromPosition(6, 7) as PictureBox).Image = Properties.Resources.WhiteKinght;
            (BoardLayoutPanel.GetControlFromPosition(7, 7) as PictureBox).Image = Properties.Resources.WhiteRook;
            for (int column = 0; column < 8; column++)
            {
                (BoardLayoutPanel.GetControlFromPosition(column, 6) as PictureBox).Image = Properties.Resources.WhitePawn;
            }
            
        }

       
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PictureBox clickedPicture = sender as PictureBox;
            if (clickedPicture != null)
            {
                // ignor clicking on pieces that refer to empty piece with no color
                if ((clickedPicture.BackColor == Color.Transparent) && (clickedPicture.Image == null))
                    return;
                if (firstClicked == null)
                {
                    ColorAvailableMovments(clickedPicture);
                    if (clickedPicture.BackColor == Color.Transparent)
                        return;
                    firstClicked = clickedPicture;
                    return;
                }

                // igonr clicking on pieces that not colored with green !
                if (clickedPicture.BackColor == Color.ForestGreen)

                // detect if user click on same first clicked piece, if yes then he need to cancel this moving 
                if(firstClicked == clickedPicture)
                {
                    foreach (PictureBox control in AvailablePiecesToMove)
                    {
                        control.BackColor = Color.Transparent;
                    }
                    firstClicked.BackColor = Color.Transparent;
                    firstClicked = null;
                    return;
                }
                if (!(AvailablePiecesToMove.Contains(clickedPicture)))
                {
                    foreach (PictureBox control in AvailablePiecesToMove)
                    {
                        control.BackColor = Color.Transparent;
                    }
                    firstClicked.BackColor = Color.Transparent;
                    firstClicked = null;
                    return;
                }
                else
                {
                    MovePiece(BoardLayoutPanel.GetPositionFromControl(firstClicked), BoardLayoutPanel.GetPositionFromControl(clickedPicture));
                    firstClicked = null;
                }
                    
            }

        }
        private TableLayoutPanelCellPosition ConvertFromPostionToCellPosition(Position postion)
        {
            TableLayoutPanelCellPosition cellPosition = new TableLayoutPanelCellPosition(postion.Column-1,0);
            switch (postion.Row)
            {
                case 1: cellPosition.Row = 7;
                    break;
                case 2: cellPosition.Row = 6;
                    break;
                case 3: cellPosition.Row = 5;
                    break;
                case 4: cellPosition.Row = 4;
                    break;
                case 5: cellPosition.Row = 3;
                    break;
                case 6: cellPosition.Row = 2;
                    break;
                case 7: cellPosition.Row = 1;
                    break;
                case 8: cellPosition.Row = 0;
                    break;
            }
            return cellPosition;
        }
        private Position ConverTFromCellPositionToPostion(TableLayoutPanelCellPosition cellPostion)
        {
            Position Position = new Position(1, cellPostion.Column+1);
            switch (cellPostion.Row)
            {
                case 0: Position.Row = 8;
                    break;
                case 1: Position.Row = 7;
                    break;
                case 2: Position.Row = 6;
                    break;
                case 3: Position.Row = 5;
                    break;
                case 4: Position.Row = 4;
                    break;
                case 5: Position.Row = 3;
                    break;
                case 6: Position.Row = 2;
                    break;
                case 7: Position.Row = 1;
                    break;
            }
            return Position;
        }

        private void MovePiece(TableLayoutPanelCellPosition Position1, TableLayoutPanelCellPosition Position2)
        {
            // Move to new position and if it sucess do this :
            if (Board.MovePieceToPosition(ConverTFromCellPositionToPostion(Position1), ConverTFromCellPositionToPostion(Position2)))
            {
                (BoardLayoutPanel.GetControlFromPosition(Position2.Column, Position2.Row) as PictureBox).Image = (BoardLayoutPanel.GetControlFromPosition(Position1.Column, Position1.Row) as PictureBox).Image;
                (BoardLayoutPanel.GetControlFromPosition(Position1.Column, Position1.Row) as PictureBox).Image = null;
                (BoardLayoutPanel.GetControlFromPosition(Position1.Column, Position1.Row) as PictureBox).BackColor = Color.Transparent;
                foreach(PictureBox item in AvailablePiecesToMove)
                    item.BackColor = Color.Transparent;
                AvailablePiecesToMove.Clear();
                SwitchTurns();
            }
            else
                MessageBox.Show("this move unavailable for some reason :( .");
        }

        private void ColorAvailableMovments(PictureBox clickedPicture )
        {
            TableLayoutPanelCellPosition tableLayoutPanelCellPosition = BoardLayoutPanel.GetPositionFromControl(clickedPicture);
            Piece selectedPiece = Board.GetPiece(ConverTFromCellPositionToPostion(tableLayoutPanelCellPosition));
            if(selectedPiece==null)
            {
                MessageBox.Show("this move unavailable for some reason :( .");
                return;
            }
            if (((selectedPiece.Color == PieceColor.Black) && (CurrentTurn == Turn.White)) || ((selectedPiece.Color == PieceColor.White) && (CurrentTurn == Turn.Black)))
                return;
            clickedPicture.BackColor = Color.Yellow;

            foreach(Position item in selectedPiece.AvailabePosisions)
            {
                TableLayoutPanelCellPosition cellPosision=ConvertFromPostionToCellPosition(item);
                PictureBox PicBox = BoardLayoutPanel.GetControlFromPosition(cellPosision.Column, cellPosision.Row) as PictureBox;
                Piece PieceItem=Board.GetPiece(item);
                if (PieceItem.Color != PieceColor.Unknown)
                    PicBox.BackColor = Color.Red;
                else 
                    PicBox.BackColor = Color.ForestGreen;
                AvailablePiecesToMove.Add(PicBox);
            }

        }

        private void SwitchTurns()
        {
            if(CurrentTurn== Turn.White)
            {
                WhiteTurn.BackgroundImage = Properties.Resources.TrunDisabled;
                BlackTurn.BackgroundImage= Properties.Resources.TurnEnabled;
                CurrentTurn = Turn.Black;
            }
            else
            {
                WhiteTurn.BackgroundImage = Properties.Resources.TurnEnabled;
                BlackTurn.BackgroundImage = Properties.Resources.TrunDisabled;
                CurrentTurn = Turn.White;
            }

        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitializeBoard();
            Board = new ChessBoard();
            Board.Checkmate += Board_Checkmate;
            Board.Winner += Board_Winner;
            CurrentTurn = Turn.Black;
            SwitchTurns();
            MessageBox.Show("White play first :)");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thank you for playing our game :)");
            Application.Exit();
        }
        void Application_ApplicationExit(object sender, EventArgs e)
        {
            MessageBox.Show("Thank you for playing our game :)");
        }

        private void aboutProgrammerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutProgrammarFrm frm = new AboutProgrammarFrm();
            frm.ShowDialog();
        }

        private void aboutGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutGame frm = new AboutGame();
            frm.ShowDialog();
        }

        
    }
}
