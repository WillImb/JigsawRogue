using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    [SerializeField] public Transform[] slots;
    public Piece[] occupied;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        occupied = new Piece[slots.Length];
    }

    public bool TryPlacePiece(Piece piece)
    {
        int slotIndex = GetClosestSlot(piece.transform.position);
        if (slotIndex == -1) return false;

        if (occupied[slotIndex] != null) return false;

        if(!IsValidPlacement(piece, slotIndex)) return false;

        PlacePiece(piece, slotIndex);
        return true;
    }

    //Works for 2x2 grid, if expanding better method needs to be found for checking valid placement
    private bool IsValidPlacement(Piece piece, int slotIndex)
    {
        switch (slotIndex)
        {
            case 0: // TL
                return CheckNeighbor(piece, 1, Direction.East, Direction.West) &&
                        CheckNeighbor(piece, 2, Direction.South, Direction.North);

            case 1: // TR
                return CheckNeighbor(piece, 0, Direction.West, Direction.East) &&
                        CheckNeighbor(piece, 3, Direction.South, Direction.North);

            case 2: // BL
                return CheckNeighbor(piece, 0, Direction.North, Direction.South) &&
                        CheckNeighbor(piece, 3, Direction.East, Direction.West);

            case 3: // BR
                return CheckNeighbor(piece, 2, Direction.West, Direction.East) &&
                        CheckNeighbor(piece, 1, Direction.North, Direction.South);

            default:
                return false;
        }
    }

    private bool CheckNeighbor(Piece piece, int neighborIndex, Direction pieceSide, Direction neighborSide)
    {
        if (occupied[neighborIndex] == null) return true; // Valid if no Neighbor

        sideType placedPieceSide = piece.GetSide(pieceSide);
        sideType neighborPieceSide = occupied[neighborIndex].GetSide(neighborSide);

        //Rules for Placing Pieces
        if (placedPieceSide == sideType.Flat)
        {
            return neighborPieceSide == sideType.In || neighborPieceSide == sideType.Flat;
        }
        if (placedPieceSide == sideType.In)
        {
            return true;
        }
        if (placedPieceSide == sideType.Out)
        {
            return neighborPieceSide == sideType.In;
        }
        return false;
    }

    void PlacePiece(Piece piece, int index)
    {
        occupied[index] = piece;
        piece.LockToSlot(slots[index]);
    }

    int GetClosestSlot(Vector3 piecePos)
    {
        float minDist = float.MaxValue;
        int bestIndex = -1;

        for (int i = 0; i < slots.Length; i++)
        {
            float dist = Vector2.Distance(piecePos, slots[i].position);

            if (dist < minDist && dist < 1.5f) // snap radius
            {
                minDist = dist;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    public void RemovePiece(Piece piece)
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i] == piece)
            {
                occupied[i] = null;
                return;
            }
        }
    }

}
