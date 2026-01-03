using UnityEngine;
using UnityEngine.Serialization;

public enum ElementType{Fire, Water, Earth, Wood, Null}
    
[CreateAssetMenu(fileName = "NewElement", menuName = "Elements/Element")]
public class ElementalData : ScriptableObject
{
    public int Damage;
    public Material Material;
    public ElementType Type;
    public ElementType WeaknessType;
}
