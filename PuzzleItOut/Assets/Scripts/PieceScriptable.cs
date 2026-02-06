using UnityEngine;

[CreateAssetMenu(fileName = "Piece Object", menuName = "ScriptableObjects/PieceScriptableObject", order = 1)]
public class PieceScriptable : ScriptableObject
{
    public string pieceName;
    public int baseDamange;
    public cardType cardType;
    public sideType north;
    public sideType east;
    public sideType south;
    public sideType west;
}

public enum cardType
{
    fire,
    water,
    air,
    earth
}

public enum sideType
{
    Flat,
    In,
    Out
}
