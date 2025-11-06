using UnityEngine;
using UnityEngine.UI;

public class WateringBarSetup : MonoBehaviour
{
    [Header("Configuración de Zonas")]
    [Range(0f, 1f)] public float greenZoneStart = 0.4f;
    [Range(0f, 1f)] public float greenZoneEnd = 0.6f;
    [Range(0f, 1f)] public float yellowZoneMargin = 0.15f;
    
    [Header("Imágenes de Zona")]
    public Image barBackground;
    public Image greenZoneImage;
    public Image yellowZoneLeftImage;
    public Image yellowZoneRightImage;
    public Image redZoneLeftImage;
    public Image redZoneRightImage;
    
    [Header("Colores")]
    public Color greenColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
    public Color yellowColor = new Color(0.9f, 0.9f, 0.2f, 0.8f);
    public Color redColor = new Color(0.9f, 0.2f, 0.2f, 0.8f);
    
    private void Start()
    {
        UpdateZones();
    }
    
    [ContextMenu("Actualizar Zonas")]
    public void UpdateZones()
    {
        if (barBackground == null)
        {
            Debug.LogError("❌ Asigna Bar Background!");
            return;
        }
        
        RectTransform barRect = barBackground.GetComponent<RectTransform>();
        float barWidth = barRect.rect.width;
        
        // HACER que todas las zonas sean hijas de la barra
        SetZoneAsChild(greenZoneImage, barRect);
        SetZoneAsChild(yellowZoneLeftImage, barRect);
        SetZoneAsChild(yellowZoneRightImage, barRect);
        SetZoneAsChild(redZoneLeftImage, barRect);
        SetZoneAsChild(redZoneRightImage, barRect);
        
        // Calcular posiciones
        float yellowStartLeft = Mathf.Max(0, greenZoneStart - yellowZoneMargin);
        float yellowEndRight = Mathf.Min(1, greenZoneEnd + yellowZoneMargin);
        
        // ZONA ROJA IZQUIERDA (0% hasta inicio amarillo)
        SetupZone(redZoneLeftImage, barRect, 0, yellowStartLeft, redColor);
        
        // ZONA AMARILLA IZQUIERDA (inicio amarillo hasta inicio verde)
        SetupZone(yellowZoneLeftImage, barRect, yellowStartLeft, greenZoneStart, yellowColor);
        
        // ZONA VERDE (centro)
        SetupZone(greenZoneImage, barRect, greenZoneStart, greenZoneEnd, greenColor);
        
        // ZONA AMARILLA DERECHA (fin verde hasta fin amarillo)
        SetupZone(yellowZoneRightImage, barRect, greenZoneEnd, yellowEndRight, yellowColor);
        
        // ZONA ROJA DERECHA (fin amarillo hasta 100%)
        SetupZone(redZoneRightImage, barRect, yellowEndRight, 1f, redColor);
        
        Debug.Log($"✓ Zonas actualizadas - Verde: {greenZoneStart * 100:F0}%-{greenZoneEnd * 100:F0}%");
    }
    
    private void SetZoneAsChild(Image zoneImage, RectTransform parent)
    {
        if (zoneImage == null) return;
        
        RectTransform zoneRect = zoneImage.GetComponent<RectTransform>();
        if (zoneRect.parent != parent)
        {
            zoneRect.SetParent(parent, false);
        }
    }
    
    private void SetupZone(Image zoneImage, RectTransform barRect, float startPercent, float endPercent, Color color)
    {
        if (zoneImage == null) return;
        
        RectTransform zoneRect = zoneImage.GetComponent<RectTransform>();
        float barWidth = barRect.rect.width;
        
        float width = (endPercent - startPercent) * barWidth;
        float startPos = startPercent * barWidth;
        
        // Anclar al lado izquierdo de la barra
        zoneRect.anchorMin = new Vector2(0, 0);
        zoneRect.anchorMax = new Vector2(0, 1);
        zoneRect.pivot = new Vector2(0, 0.5f);
        
        zoneRect.sizeDelta = new Vector2(width, 0);
        zoneRect.anchoredPosition = new Vector2(startPos, 0);
        
        zoneImage.color = color;
        
        // Ocultar si es muy pequeña
        zoneImage.gameObject.SetActive(width > 1f);
    }
}
