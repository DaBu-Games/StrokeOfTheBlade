using UnityEngine;

public enum ElementType{Fire, Water, Earth, Wood, Null}
    
[CreateAssetMenu(fileName = "NewElement", menuName = "Elements/Element")]
public class ElementalData : ScriptableObject
{
    public int damage;
    public Material material;
    public ElementType Type;
    public ElementType WeaknessType;
}
