using System.Collections.Generic;
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

        if(!IsValidPlacement(piece, slotIndex,true)) return false;

        PlacePiece(piece, slotIndex);
        ValidateBoard();

        return true;
    }

    //Works for 2x2 grid, if expanding better method needs to be found for checking valid placement
    private bool IsValidPlacement(Piece piece, int slotIndex,bool placing)
    {
        switch (slotIndex)
        {
            case 0: // TL

                if(CheckNeighbor(piece, 1, Direction.East, Direction.West, placing) &&
                        CheckNeighbor(piece, 2, Direction.South, Direction.North, placing)){

                    if (!placing)
                    {
                        return (occupied[1] || occupied[2]);
                    }
                    else
                    {
                        return true;

                    }
                }
                return false;

            case 1: // TR
               if(CheckNeighbor(piece, 0, Direction.West, Direction.East, placing) &&
                        CheckNeighbor(piece, 3, Direction.South, Direction.North, placing))
                {

                    if (!placing)
                    {
                        return (occupied[0] || occupied[3]);
                    }
                    else
                    {
                        return true;

                    }
                }
                return false;

            case 2: // BL
                if( CheckNeighbor(piece, 0, Direction.North, Direction.South, placing) &&
                        CheckNeighbor(piece, 3, Direction.East, Direction.West, placing))
                {

                    if (!placing)
                    {
                        return (occupied[0] || occupied[3]);
                    }
                    else
                    {
                        return true;

                    }
                }
                return false;

            case 3: // BR
                if (CheckNeighbor(piece, 2, Direction.West, Direction.East, placing) &&
                        CheckNeighbor(piece, 1, Direction.North, Direction.South, placing))
                {

                    if (!placing)
                    {
                        return (occupied[1] || occupied[2]);
                    }
                    else
                    {
                        return true;

                    }
                }
                return false;

            default:
                return false;
        }
    }

    

    private bool CheckNeighbor(Piece piece, int neighborIndex, Direction pieceSide, Direction neighborSide,bool placing)
    {
        if (occupied[neighborIndex] == null) return true; // Valid if no Neighbor

        sideType placedPieceSide = piece.GetSide(pieceSide);
        sideType neighborPieceSide = occupied[neighborIndex].GetSide(neighborSide);

        //Rules for Placing Pieces
        if (placedPieceSide == sideType.Out && neighborPieceSide != sideType.In)
        {
            return false;
           //return neighborPieceSide == sideType.In || neighborPieceSide == sideType.Flat;
        }
        if(placedPieceSide != sideType.In && neighborPieceSide == sideType.Out)
        {
            return false;
        }

        //this only runs if the player is checking the board for a valid spell
        if (!placing)
        {
            if (placedPieceSide == sideType.Out && neighborPieceSide == sideType.In)
            {
                return CheckElementNeighbor(piece, neighborIndex, placedPieceSide);
            }
            else if(placedPieceSide == sideType.In)
            {
                if(neighborPieceSide == sideType.In || neighborPieceSide == sideType.Flat)
                {
                    return false;
                }
                else
                {
                    return CheckElementNeighbor(piece, neighborIndex, placedPieceSide);
                }
            }
            else if(placedPieceSide == sideType.Flat)
            {
                return false;
            }

        }


        return true;
    }

    /// <summary>
    /// Checks to see if a valid spell is on the board, if not, then sets cast buttons to be uninteractable
    /// </summary>
    public void ValidateBoard()
    {

        if (!CheckOccupiedCount())
        {
            PanelManager.instance.DisableButtons("2,4");

            return;
        }

        for(int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i] != null)
            {
                if(IsValidPlacement(occupied[i], i, false))
                {
                    PanelManager.instance.EnableButtons("2,4");

                }
                else
                {
                    PanelManager.instance.DisableButtons("2,4");
                    return;

                }
            }
        }
    }


   

    private bool CheckElementNeighbor(Piece piece, int neighborIndex, sideType side)
    {
        cardType otherCardType = occupied[neighborIndex].pieceData.cardType;
        if (side == sideType.Out)
        {
            switch (piece.pieceData.cardType)
            {
                case cardType.fire:
                    if (otherCardType == cardType.water)
                        return false;
                break;
                case cardType.water:
                break;
                case cardType.air:
                    if (otherCardType == cardType.water)
                        return false;
                break;
                case cardType.earth:
                    if (otherCardType == cardType.fire || otherCardType == cardType.air)
                        return false;
                break;
            }
        }
        else if(side == sideType.In){
            switch (piece.pieceData.cardType)
            {
                case cardType.fire:
                    if (otherCardType == cardType.earth)
                        return false;
                break;
                case cardType.water:
                    if (otherCardType == cardType.fire || otherCardType == cardType.air)
                        return false;
                    break;
                case cardType.air:
                    if (otherCardType == cardType.earth)
                        return false;
                break;
                case cardType.earth:
                    
                break;
            }
        }


        return true;
    }

    void PlacePiece(Piece piece, int index)
    {
        
        occupied[index] = piece;
        piece.LockToSlot(slots[index]);
        //Spawn the place VFX
        VFXManager.instance.SpawnParticle(slots[index].position, 1);
        
        if (CheckOccupiedCount())
        {
            PanelManager.instance.EnableButtons("2,4");
        }
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
                DeckManager.instance.hand.Add(occupied[i].gameObject);

                occupied[i] = null;

                ValidateBoard();

                return;
            }
        }
    }

    public List<PieceScriptable> GetBoardPieces()
    {
        List<PieceScriptable> pieces = new List<PieceScriptable>();
        foreach (Piece piece in occupied)
        {
            if (piece != null)
                pieces.Add(piece.pieceData);
        }
        return pieces;
    }

    private bool CheckOccupiedCount()
    {
        int count = 0;
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i])
            {
                count++;
            }
        }
        if (count >= 2)
        {
            return true;
        }

        return false;
    }

}
