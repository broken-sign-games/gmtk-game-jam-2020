using System;
using UnityEngine;

namespace GMTK2020.Data
{
    public sealed class Tile : IEquatable<Tile>
    {
        public Guid ID { get; } = Guid.NewGuid();

        public int Color { get; }

        public bool Inert { get; private set; }

        private bool _marked;
        public bool Marked {
            get => _marked;
            set
            {
                _marked = value && !Inert;
            }
        }

        public bool Wildcard { get; private set; } = false;

        public Vector2Int Position { get; set; }

        public Tile(int color)
            : this(color, new Vector2Int(-1, -1)) { }

        public Tile(int color, Vector2Int position)
        {
            Color = color;
            Position = position;
        }

        public Tile(Tile other)
        {
            Color = other.Color;
            Inert = other.Inert;
            Marked = other.Marked;
            Position = other.Position;
            Wildcard = other.Wildcard;
        }

        public void MakeInert()
        {
            Inert = true;
            Marked = false;
        }

        public void MakeWildcard()
        {
            Wildcard = true;
        }

        public void Refill()
        {
            Inert = false;
        }

        public override string ToString()
            => $"Tile: {Color} at {Position} ({(Inert ? "Inert" : Marked ? "Marked" : "Unmarked")}{(Wildcard ? ", Wildcard" : "")})";

        #region Equality pattern
        public override bool Equals(object other)
        {
            return Equals(other as Tile);
        }

        public bool Equals(Tile other)
        {
            // If parameter is null, return false.
            if (other is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (GetType() != other.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return Color == other.Color
                && Position == other.Position
                && Marked == other.Marked
                && Wildcard == other.Wildcard
                && Inert == other.Inert;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = hash * 7 + Color.GetHashCode();
            hash = hash * 7 + Position.GetHashCode();
            hash = hash * 7 + Marked.GetHashCode();
            hash = hash * 7 + Inert.GetHashCode();
            hash = hash * 7 + Wildcard.GetHashCode();
            return hash;
        }

        public static bool operator ==(Tile lhs, Tile rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                // null == null = true.
                return rhs is null;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Tile lhs, Tile rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
    }
}
