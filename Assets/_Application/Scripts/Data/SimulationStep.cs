using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{

    public abstract class SimulationStep
    {
        public abstract bool FinalStep { get; }
        public int ChangeInResource { get; }

        public SimulationStep(int changeInResource)
        {
            ChangeInResource = changeInResource;
        }
    }

    public class MatchStep : SimulationStep
    {
        public override bool FinalStep => false;

        public int ChainLength { get; }

        public HashSet<Tile> MatchedTiles { get; }
        public List<MovedTile> MovedTiles { get; }

        public HashSet<Vector2Int> LeftEndsOfHorizontalMatches { get; }
        public HashSet<Vector2Int> BottomEndsOfVerticalMatches { get; }

        public MatchStep(int chainLength, HashSet<Tile> matchedTiles, List<MovedTile> movingTiles, HashSet<Vector2Int> leftEndsOfHorizontalMatches, HashSet<Vector2Int> bottomEndsOfVerticalMatches, int changeInResource)
            : base(changeInResource)
        {
            ChainLength = chainLength;
            MatchedTiles = matchedTiles;
            MovedTiles = movingTiles;
            LeftEndsOfHorizontalMatches = leftEndsOfHorizontalMatches;
            BottomEndsOfVerticalMatches = bottomEndsOfVerticalMatches;
        }
    }

    public class CleanUpStep : SimulationStep
    {
        public override bool FinalStep => true;

        public List<MovedTile> NewTiles { get; }
        public HashSet<Tile> InertTiles { get; }

        public CleanUpStep(List<MovedTile> newTiles, HashSet<Tile> inertTiles, int changeInResource)
            : base(changeInResource)
        {
            NewTiles = newTiles;
            InertTiles = inertTiles;
        }
    }

    public class RemovalStep : SimulationStep
    {
        public override bool FinalStep => true;

        public HashSet<Tile> RemovedTiles { get; }
        public List<MovedTile> MovedTiles { get; }
        public List<MovedTile> NewTiles { get; }

        public RemovalStep(HashSet<Tile> removedTiles, List<MovedTile> movedTiles, List<MovedTile> newTiles, int changeInResource)
            : base(changeInResource)
        {
            RemovedTiles = removedTiles;
            MovedTiles = movedTiles;
            NewTiles = newTiles;
        }
    }

    public class PermutationStep : SimulationStep
    {
        public override bool FinalStep => true;

        public List<MovedTile> MovedTiles { get; }

        public PermutationStep(List<MovedTile> movedTiles, int changeInResource)
            : base(changeInResource)
        {
            MovedTiles = movedTiles;
        }
    }

    public class RotationStep : SimulationStep
    {
        public override bool FinalStep => true;

        public Vector2 Pivot { get; }
        public RotationSense RotationSense { get; }
        public List<MovedTile> MovedTiles { get; }

        public RotationStep(Vector2 pivot, RotationSense rotationSense, List<MovedTile> movedTiles, int changeInResource)
            : base(changeInResource)
        {
            Pivot = pivot;
            RotationSense = rotationSense;
            MovedTiles = movedTiles;
        }
    }

    public class TileStateChangeStep : SimulationStep
    {
        public override bool FinalStep => true;

        public List<Tile> AffectedTiles { get; }

        public TileStateChangeStep(List<Tile> affectedTiles, int changeInResource)
            : base(changeInResource)
        {
            AffectedTiles = affectedTiles;
        }
    }

    public class PredictionStep : TileStateChangeStep
    {
        public PredictionStep(List<Tile> affectedTiles, int changeInResource) : base(affectedTiles, changeInResource) { }
    }

    public class RefillStep : TileStateChangeStep
    {
        public RefillStep(List<Tile> affectedTiles, int changeInResource) : base(affectedTiles, changeInResource) { }
    }

    public class WildcardStep : TileStateChangeStep
    {
        public WildcardStep(List<Tile> affectedTiles, int changeInResource) : base(affectedTiles, changeInResource) { }
    }
}