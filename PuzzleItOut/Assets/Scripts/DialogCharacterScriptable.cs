using UnityEngine;


/// <summary>
/// The dialog character scriptable represrents a character during a dialog sequence. This is a container for the information about the character
/// as a dialog object, such as thier profile image and text font.
/// </summary>

[CreateAssetMenu(fileName = "Dialog Char Object", menuName = "ScriptableObjects/DialogCharacterScriptableObject", order = 3)]
public class DialogCharacterScriptable : ScriptableObject
{
    public Sprite profile;


}
