using UnityEngine;

public enum UpgradeType { Single, Double, Triple }
public enum ValueType { Combat, Healing, Gold, WildValue }
public enum ElementType { Fire, Water, Earth, Air, Wild }

[CreateAssetMenu(menuName = "Shop/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public UpgradeType upgradeType;
    public ValueType valueType;
    public ElementType elementType;
}
