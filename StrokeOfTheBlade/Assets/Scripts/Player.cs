using UnityEngine;

public class Player : MonoBehaviour, Entity
{
    [SerializeField] private UIColorTransition _colorTransition;
    
    public void OnDestroy()
    {
        //Game over
    }

    public void OnHit()
    {
        _colorTransition.Play();
    }
}
