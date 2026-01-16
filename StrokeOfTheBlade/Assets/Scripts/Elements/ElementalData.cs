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
    public AudioClip OnChange;
    public AudioClip OnDestroy;
    
    [Header("Movement")]
    public float StartDelay = 100f;
    public float TargetSpeed = 40f;
    public float MinProjectileSpeed = 2f;
    public float MaxProjectileSpeed = 10f;
    public float MinDuration = 0.2f;
    public float MaxDuration = 2f;
    public float ForwardCurve = 1f;
    public int LineSegments = 5;
}
