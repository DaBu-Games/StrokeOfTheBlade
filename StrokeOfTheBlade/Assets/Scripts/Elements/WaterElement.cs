using UnityEngine;

public class WaterElement : IElement
{
    public ElementalData data { get; }
    
    public WaterElement(ElementalData elementalData) => data = elementalData;

    public void OnHit(Collider target)
    {
        IElement element = target.GetComponent<IElement>();

        if (element != null)
        {
            if (element.data.Type == this.data.WeaknessType)
            {
                OnDestory();
            }
        }
    }

    public void OnDestory()
    {
        
    }

}
