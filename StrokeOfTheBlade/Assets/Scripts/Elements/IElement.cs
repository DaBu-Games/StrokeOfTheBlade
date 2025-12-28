using UnityEngine;

public interface IElement
{
    int damage { get; }
    void OnHit(Collider target);
    void OnDestory();
}
