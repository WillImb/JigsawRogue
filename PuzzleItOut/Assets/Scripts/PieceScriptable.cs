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

    public float healingValue;
    public float combatValue;
    public float goldValue;
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

public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}
