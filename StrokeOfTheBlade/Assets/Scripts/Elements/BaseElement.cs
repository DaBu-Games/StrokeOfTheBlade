using UnityEngine;

public class BaseElement
{
    public ElementalData Data = null;
    
    public BaseElement(ElementalData elementalData) => Data = elementalData;

    public void OnHit(Collider target)
    {
        BaseElement element = target.GetComponent<BaseElement>();

        if (element != null)
        {
            if (element.Data.Type == this.Data.WeaknessType)
            {
                OnDestory();
            }
        }
    }

    public void OnDestory()
    {
        
    }
}
