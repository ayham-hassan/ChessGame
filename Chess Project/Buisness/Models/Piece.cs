using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess_Project.Buisness.Enums;
namespace Chess_Project.Buisness.Models
{
    public class Piece
    {
        public Piece (PieceType Type,PieceColor Color,Position DefaultPosition)
        {
            this.Color = Color;
            this.Type = Type;
            this.CurrentPosision = DefaultPosition;
            this.AvailabePosisions = new List<Position>();
        }
        public PieceType Type { get; set; }
        public PieceColor Color { get; set; }
        public Position CurrentPosision { get; set; }
        public List<Position> AvailabePosisions { get; set; }

        public Piece Clone()
        {
            Piece clone = new Piece(this.Type, this.Color, new Position(this.CurrentPosision.Row, this.CurrentPosision.Column));
            foreach(var item in this.AvailabePosisions)
            {
                clone.AvailabePosisions.Add(new Position(item.Row, item.Column));
            }
            return clone;
        }
        public static bool operator ==(Piece a, Piece b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Color == b.Color && a.CurrentPosision == b.CurrentPosision && a.Type==b.Type;
        }

        public static bool operator !=(Piece a, Piece b)
        {

            // Return true if the fields match:
            return a.Color != b.Color || a.CurrentPosision != b.CurrentPosision || a.Type != b.Type;

        }
        

    }

    public class Position
    {
        int row;
        int column;
        public Position (int Row,int Column)
        {
            this.Row = Row;
            this.Column = Column;
        }
        bool ValidateRow(int Row)
        {
            if (Row >= 1 && Row <= 8)
                return true;
            else
                return false;
        }
        bool ValidateColumn(int Column)
        {
            if (Column >= 1 && Column <= 8)
                return true;
            else
                return false;
        }
        
        public int Row
        {
            get
            {
                return row;
            }
            set
            {
                if (ValidateRow(value))
                    row = value;
                else
                    throw new System.InvalidOperationException("value that will be inserted not valid");
            }
        }

        public int Column
        {
            get
            {
                return column;
            }
            set
            {
                if (ValidateColumn(value))
                    column = value;
                else
                    throw new System.InvalidOperationException("value that will be inserted not valid");
            }
        }

        public static bool operator ==(Position a, Position b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Column == b.Column && a.Row == b.Row ;
        }

        public static bool operator !=(Position a, Position b)
        {
            
            // Return true if the fields match:
            return a.Column != b.Column || a.Row != b.Row;
        }
        

    }
}
