using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Elementmanager : MonoBehaviour
{
    [SerializeField] private List<ElementalData> elementalDatas;
    private Dictionary<ElementType, BaseElement> _elements = new Dictionary<ElementType, BaseElement>();

    private void Awake()
    {
        foreach (ElementalData data in elementalDatas)
        {
            BaseElement element = new BaseElement(data);
            _elements.Add(data.Type, element);
        }
    }

    public BaseElement GetElement(ElementType elementType)
    {
        return _elements[elementType];
    }

    public BaseElement GetRandomElement()
    {
        return _elements.ElementAt( Random.Range(0, _elements.Count) ).Value;
    }
}
