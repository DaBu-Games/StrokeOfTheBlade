using System.Collections.Generic;
using UnityEngine;

public class Elementmanager : MonoBehaviour
{
    [SerializeField] private List<ElementalData> elementalDatas;
    private Dictionary<ElementType, IElement> _elements = new Dictionary<ElementType, IElement>();

    private void Awake()
    {
        foreach (ElementalData data in elementalDatas)
        {
            IElement element = CreateElement(data);
            _elements.Add(data.Type, element);
        }
    }
    
    private IElement CreateElement(ElementalData data)
    {
        switch (data.Type)
        {
            case ElementType.Fire:
                return new FireElement(data);
            case ElementType.Water:
                return new WaterElement(data);
            case ElementType.Earth:
                return new EarthElement(data);
            case ElementType.Wood:
                return new WoodElement(data);
            default:
                return null;
        }
    }

    public IElement GetElement(ElementType elementType)
    {
        return _elements[elementType];
    }
}
