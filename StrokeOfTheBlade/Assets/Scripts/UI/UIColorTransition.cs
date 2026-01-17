using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIColorTransition : MonoBehaviour
{
    [SerializeField] private Color targetColor;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float holdDuration = 0.25f;
    
    private Image _image;
    private Coroutine routine;
    
    private void Start()
    { 
        _image = GetComponent<Image>();
        _image.enabled = false;
    }
    
    public void Play()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ChangeImage());
    }

    private IEnumerator ChangeImage()
    {
        _image.enabled = true;

        Color start = _image.color;
        start.a = 0f;
        _image.color = start;
        
        yield return StartCoroutine(FadeToTarget(start, targetColor));
        
        yield return new WaitForSeconds(holdDuration);
        
        yield return StartCoroutine(FadeToTarget(targetColor, start));

        _image.color = start;
        _image.enabled = false;
    }

    private IEnumerator FadeToTarget(Color start, Color target)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            _image.color = Color.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        
        _image.color = target;
    }
}
