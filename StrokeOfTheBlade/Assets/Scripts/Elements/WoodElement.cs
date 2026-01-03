using UnityEngine;

public class WoodElement : IElement
{
    public ElementalData data { get; }
    
    public WoodElement(ElementalData elementalData) => data = elementalData;

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
