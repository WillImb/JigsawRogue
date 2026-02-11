using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combo Object", menuName = "ScriptableObjects/ComboScriptableObject", order = 2)]
public class ComboScriptable : ScriptableObject
{
    public string comboName;
    public List<PieceScriptable> comboList;
}


