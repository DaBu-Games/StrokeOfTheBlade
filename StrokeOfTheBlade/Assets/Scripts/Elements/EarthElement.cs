using UnityEngine;

public class EarthElement : IElement
{
    public ElementalData data { get; }
    
    public EarthElement(ElementalData elementalData) => data = elementalData;

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
