using Unity.VisualScripting;
using UnityEngine;

public class BaseElement
{
    public ElementalData Data = null;
    
    public BaseElement(ElementalData elementalData) => Data = elementalData;

    public void OnHit(Collider target, GameObject ownProjectile)
    {
        if ((Data.CollisionLayers.value & (1 << target.gameObject.layer)) == 0)
            return; 
        
        ElementalProjectile projectile = target.GetComponent<ElementalProjectile>();

        if (projectile != null)
        {
            BaseElement element = projectile.GetElement();
            
            if (Data.Type != element.Data.WeaknessType)
            {
                Object.Destroy(ownProjectile);
            }
        }
        else
        {
            HealthBar healthBar = target.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(Data.Damage);
                Object.Destroy(ownProjectile);
            }
        }
    }
}
