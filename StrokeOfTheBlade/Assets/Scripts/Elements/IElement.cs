using UnityEngine;

public interface IElement
{
    ElementalData data { get; }
    void OnHit(Collider target);
    void OnDestory();
}
