using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [SerializeField] private Transform[] slots;
    private Piece[] occupied;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        occupied = new Piece[slots.Length];
    }

    public bool TryPlacePiece(Piece piece)
    {
        int slotIndex = GetClosestSlot(piece.transform.position);
        if (slotIndex == -1) return false;

        if (occupied[slotIndex] != null) return false;

        PlacePiece(piece, slotIndex);
        return true;
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
