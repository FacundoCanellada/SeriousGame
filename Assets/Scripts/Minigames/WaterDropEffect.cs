using UnityEngine;
using System.Collections;

public class WaterDropEffect : MonoBehaviour
{
    [Header("Configuración del Efecto")]
    public float fallDuration = 1f;
    public float fallDistance = 100f;
    public AnimationCurve fallCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    
    [Header("Sprites de Gotitas")]
    public Sprite[] dropSprites;
    public float animationSpeed = 0.1f;
    
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
        
        StartCoroutine(PlayWaterEffect());
    }
    
    private IEnumerator PlayWaterEffect()
    {
        // Animar sprites si están disponibles
        StartCoroutine(AnimateDropSprites());
        
        // Animar caída
        float elapsed = 0f;
        Vector3 endPosition = startPosition + Vector3.down * fallDistance;
        
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fallDuration;
            float curveValue = fallCurve.Evaluate(progress);
            
            transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);
            
            // Fade out al final
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f - progress;
                spriteRenderer.color = color;
            }
            
            yield return null;
        }
        
        // Destruir efecto
        Destroy(gameObject);
    }
    
    private IEnumerator AnimateDropSprites()
    {
        if (dropSprites == null || dropSprites.Length == 0 || spriteRenderer == null)
            yield break;
            
        int currentFrame = 0;
        
        while (gameObject != null)
        {
            spriteRenderer.sprite = dropSprites[currentFrame % dropSprites.Length];
            currentFrame++;
            
            yield return new WaitForSeconds(animationSpeed);
        }
    }
}