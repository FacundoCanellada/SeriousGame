using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlantWateringGame : MonoBehaviour
{
    [Header("Estados de la Planta")]
    public Sprite[] plantGrowthSprites; // 0=semilla, 1=brote, 2=adulta
    public Sprite plantDeadSprite; // Planta seca
    public Image plantImage;
    
    [Header("Sistema de Barra de Riego")]
    public GameObject wateringBarPanel; // Panel que contiene toda la barra
    public Image barBackground;
    public Image barFillRed; // Zona roja
    public Image barFillYellow; // Zona amarilla
    public Image barFillGreen; // Zona verde
    public Image indicatorImage; // El indicador que se mueve
    public RectTransform indicatorTransform;
    
    [Header("Configuración de Zonas")]
    [Range(0f, 1f)] public float greenZoneStart = 0.4f;
    [Range(0f, 1f)] public float greenZoneEnd = 0.6f;
    [Range(0f, 1f)] public float yellowZoneMargin = 0.15f;
    
    [Header("Velocidad del Indicador")]
    public float indicatorSpeed = 0.3f; // Velocidad de movimiento (0 a 1 por segundo)
    
    [Header("UI del Juego")]
    public Button waterButton;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI instructionsText;
    public GameObject[] lifeIcons; // Array de 3 iconos de vida
    
    [Header("Paneles")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public Button restartButton;
    public Button nextButton;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip wateringSound;
    public AudioClip successSound;
    public AudioClip failSound;
    public AudioClip growthSound;
    public AudioClip gameOverSound;
    public AudioClip victorySound;
    
    [Header("Navegación")]
    public string nextSceneName = "Tablero";
    
    // Variables del juego
    private int currentStage = 0; // 0=semilla, 1=brote, 2=adulta
    private int livesRemaining = 3;
    private float indicatorPosition = 0f; // 0 a 1
    private bool isHoldingButton = false;
    private bool canWater = true;
    private bool gameEnded = false;
    
    // Tracking vocacional
    private int perfectHits = 0;
    private int goodHits = 0;
    private int totalAttempts = 0;
    private float gameStartTime;
    
    private void Start()
    {
        InitializeGame();
        SetupButtons();
        gameStartTime = Time.time;
    }
    
    private void OnEnable()
    {
        // Resetear cuando se activa la escena
        StopAllCoroutines();
    }
    
    private void Update()
    {
        // Mover indicador mientras se mantiene presionado
        if (isHoldingButton && !gameEnded)
        {
            MoveIndicator();
        }
    }
    
    private void InitializeGame()
    {
        currentStage = 0;
        livesRemaining = 3;
        gameEnded = false;
        perfectHits = 0;
        goodHits = 0;
        totalAttempts = 0;
        canWater = true;
        isHoldingButton = false;
        
        // RESETEAR escalas y posiciones
        if (plantImage != null)
        {
            plantImage.transform.localScale = Vector3.one;
            plantImage.transform.localRotation = Quaternion.identity;
        }
        
        UpdatePlantDisplay();
        UpdateUI();
        HideWateringBar();
        
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        
        Debug.Log("🌱 Juego de planta inicializado");
    }
    
    private void SetupButtons()
    {
        if (waterButton != null)
        {
            // Agregar Event Trigger para detectar PointerDown y PointerUp
            var eventTrigger = waterButton.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = waterButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            }
            
            // Limpiar eventos existentes
            eventTrigger.triggers.Clear();
            
            // PointerDown - Cuando presiona
            var pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { StartWatering(); });
            eventTrigger.triggers.Add(pointerDown);
            
            // PointerUp - Cuando suelta
            var pointerUp = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { StopWatering(); });
            eventTrigger.triggers.Add(pointerUp);
        }
        
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
            
        if (nextButton != null)
            nextButton.onClick.AddListener(GoToNextScene);
    }
    
    private void StartWatering()
    {
        if (!canWater || gameEnded) return;
        
        Debug.Log("▶ Iniciando riego");
        isHoldingButton = true;
        canWater = false;
        
        ShowWateringBar();
        ResetIndicator();
        PlaySound(wateringSound);
    }
    
    private void StopWatering()
    {
        if (!isHoldingButton || gameEnded) return;
        
        Debug.Log($"⏸ Detenido en posición: {indicatorPosition:F2}");
        isHoldingButton = false;
        
        EvaluateWatering();
    }
    
    private void MoveIndicator()
    {
        // Mover el indicador mientras se mantiene presionado
        indicatorPosition += indicatorSpeed * Time.deltaTime;
        indicatorPosition = Mathf.Clamp01(indicatorPosition);
        
        UpdateIndicatorVisual();
        
        // Si llega al final, auto-soltar
        if (indicatorPosition >= 0.99f)
        {
            Debug.Log("⚠ Llegó al final!");
            StopWatering();
        }
    }
    
    private void UpdateIndicatorVisual()
    {
        if (indicatorTransform == null)
        {
            Debug.LogError("❌ Indicator Transform NO asignado!");
            return;
        }
        
        if (barBackground == null)
        {
            Debug.LogError("❌ Bar Background NO asignado!");
            return;
        }
        
        // Obtener el RectTransform de la barra
        RectTransform barRect = barBackground.GetComponent<RectTransform>();
        
        // Obtener el ancho REAL de la barra
        float barWidth = barRect.rect.width;
        
        // Calcular la posición dentro de la barra (de 0 a barWidth)
        float localXPos = indicatorPosition * barWidth;
        
        // Posicionar el indicador
        indicatorTransform.anchoredPosition = new Vector2(localXPos, 0);
        
        // Debug visual cada 20 frames
        if (Time.frameCount % 20 == 0 && isHoldingButton)
        {
            Debug.Log($"🎯 Indicador: {indicatorPosition:P0} ({localXPos:F0}px de {barWidth:F0}px)");
        }
    }
    
    private void EvaluateWatering()
    {
        totalAttempts++;
        
        // Determinar en qué zona cayó
        ZoneType zone = GetZoneAtPosition(indicatorPosition);
        
        Debug.Log($"📊 Posición: {indicatorPosition:F2} → Zona: {zone}");
        
        StartCoroutine(ProcessWateringResult(zone));
    }
    
    private ZoneType GetZoneAtPosition(float pos)
    {
        // Zona verde (centro)
        if (pos >= greenZoneStart && pos <= greenZoneEnd)
        {
            return ZoneType.Perfect;
        }
        
        // Zonas amarillas (alrededor del verde)
        float yellowStartLeft = Mathf.Max(0, greenZoneStart - yellowZoneMargin);
        float yellowEndRight = Mathf.Min(1, greenZoneEnd + yellowZoneMargin);
        
        if ((pos >= yellowStartLeft && pos < greenZoneStart) || 
            (pos > greenZoneEnd && pos <= yellowEndRight))
        {
            return ZoneType.Good;
        }
        
        // Zonas rojas (extremos)
        return ZoneType.Bad;
    }
    
    private IEnumerator ProcessWateringResult(ZoneType zone)
    {
        yield return new WaitForSeconds(0.3f);
        
        Debug.Log($"✓ Resultado: {zone} en posición {indicatorPosition:F2}");
        
        switch (zone)
        {
            case ZoneType.Perfect:
                perfectHits++;
                yield return StartCoroutine(HandleSuccess("¡Perfecto! La planta está feliz 🌱"));
                break;
                
            case ZoneType.Good:
                goodHits++;
                yield return StartCoroutine(HandleSuccess("¡Bien! Pero cuidado con el riego ⚠️"));
                break;
                
            case ZoneType.Bad:
                yield return StartCoroutine(HandleFailure());
                break;
        }
    }
    
    private IEnumerator HandleSuccess(string message)
    {
        PlaySound(successSound);
        ShowMessage(message);
        
        yield return new WaitForSeconds(1f);
        
        // Crecer planta
        yield return StartCoroutine(GrowPlant());
        
        HideWateringBar();
        canWater = true;
    }
    
    private IEnumerator HandleFailure()
    {
        PlaySound(failSound);
        ShowMessage("¡Demasiada agua! ❌");
        
        livesRemaining--;
        UpdateLivesDisplay();
        
        yield return new WaitForSeconds(1.5f);
        
        if (livesRemaining <= 0)
        {
            // Game Over
            yield return StartCoroutine(ShowPlantDeath());
        }
        else
        {
            // Puede intentar de nuevo
            ShowMessage($"Te quedan {livesRemaining} intentos");
            HideWateringBar();
            canWater = true;
        }
    }
    
    private IEnumerator GrowPlant()
    {
        PlaySound(growthSound);
        
        // GUARDAR escala original para restaurarla SIEMPRE
        Vector3 originalScale = Vector3.one;
        if (plantImage != null)
        {
            originalScale = plantImage.transform.localScale;
        }
        
        Vector3 bigScale = originalScale * 1.3f;
        
        float duration = 0.5f;
        float elapsed = 0f;
        
        // Crecer
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            plantImage.transform.localScale = Vector3.Lerp(originalScale, bigScale, progress);
            yield return null;
        }
        
        // Cambiar a siguiente etapa
        currentStage++;
        UpdatePlantDisplay();
        
        // Volver a tamaño normal
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            plantImage.transform.localScale = Vector3.Lerp(bigScale, originalScale, progress);
            yield return null;
        }
        
        // FORZAR escala original
        plantImage.transform.localScale = originalScale;
        
        // Verificar victoria
        if (currentStage >= plantGrowthSprites.Length)
        {
            yield return new WaitForSeconds(1f);
            ShowVictory();
        }
        else
        {
            UpdateUI();
        }
    }
    
    private IEnumerator ShowPlantDeath()
    {
        PlaySound(gameOverSound);
        
        // Mostrar planta seca
        if (plantDeadSprite != null && plantImage != null)
        {
            // Restaurar escala antes de cambiar sprite
            plantImage.transform.localScale = Vector3.one;
            plantImage.sprite = plantDeadSprite;
        }
        
        ShowMessage("¡La planta se secó! 💀");
        
        yield return new WaitForSeconds(2f);
        
        gameEnded = true;
        canWater = false;
        isHoldingButton = false;
        
        HideWateringBar();
        
        // Mostrar panel de game over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    
    private void ShowVictory()
    {
        PlaySound(victorySound);
        gameEnded = true;
        canWater = false;
        isHoldingButton = false;
        
        // Asegurar que la planta esté en escala correcta
        if (plantImage != null)
        {
            plantImage.transform.localScale = Vector3.one;
        }
        
        HideWateringBar();
        
        // Mostrar panel de victoria
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        
        SaveVocationalData();
    }
    
    private void SaveVocationalData()
    {
        if (GameManager.Instance == null) return;
        
        float gameTime = Time.time - gameStartTime;
        float precision = (float)perfectHits / totalAttempts;
        float accuracy = (float)(perfectHits + goodHits) / totalAttempts;
        
        // Calcular scores
        float precisionScore = precision * 15f;
        float patienceScore = Mathf.Clamp(10f - (totalAttempts - 3), 0f, 10f); // Menos intentos = más paciencia
        float persistenceScore = currentStage >= plantGrowthSprites.Length ? 10f : 5f;
        
        GameManager.Instance.LogVocationalAction(VocationalArea.CienciasNaturales, 
            precisionScore + patienceScore + persistenceScore, "Completó minijuego de planta");
            
        if (UserDataManager.Instance != null)
        {
            UserData userData = UserDataManager.Instance.GetUserData();
            userData.vocationalData.paciencia += patienceScore;
            userData.vocationalData.atencionDetalle += precisionScore;
            UserDataManager.Instance.UpdateUserData(userData);
        }
        
        Debug.Log($"Precisión: {precision:P0}, Intentos: {totalAttempts}, Aciertos perfectos: {perfectHits}");
    }
    
    private void UpdatePlantDisplay()
    {
        if (plantImage != null && plantGrowthSprites != null && 
            currentStage < plantGrowthSprites.Length)
        {
            plantImage.sprite = plantGrowthSprites[currentStage];
        }
    }
    
    private void UpdateUI()
    {
        // Actualizar etapa
        if (stageText != null)
        {
            string[] stageNames = { "Semilla", "Brote", "Planta Adulta" };
            if (currentStage < stageNames.Length)
            {
                stageText.text = $"Etapa: {stageNames[currentStage]}";
            }
        }
        
        UpdateLivesDisplay();
    }
    
    private void UpdateLivesDisplay()
    {
        // Actualizar texto de vidas
        if (livesText != null)
        {
            livesText.text = $"Vidas: {livesRemaining}/3";
        }
        
        // Actualizar iconos de vida
        if (lifeIcons != null && lifeIcons.Length >= 3)
        {
            for (int i = 0; i < lifeIcons.Length; i++)
            {
                if (lifeIcons[i] != null)
                {
                    lifeIcons[i].SetActive(i < livesRemaining);
                }
            }
        }
    }
    
    private void ShowMessage(string message)
    {
        if (instructionsText != null)
        {
            instructionsText.text = message;
        }
    }
    
    private void ShowWateringBar()
    {
        if (wateringBarPanel != null)
        {
            wateringBarPanel.SetActive(true);
            
            // FORZAR que el indicador sea visible
            if (indicatorImage != null)
            {
                indicatorImage.enabled = true;
                indicatorImage.gameObject.SetActive(true);
                
                // Asegurar que tenga color visible
                Color color = indicatorImage.color;
                color.a = 1f; // Opacidad total
                indicatorImage.color = color;
            }
            
            if (indicatorTransform != null)
            {
                indicatorTransform.gameObject.SetActive(true);
            }
            
            Debug.Log("✓ Barra y indicador mostrados");
        }
        else
        {
            Debug.LogError("❌ Watering Bar Panel NO asignado!");
        }
    }
    
    private void HideWateringBar()
    {
        if (wateringBarPanel != null)
        {
            wateringBarPanel.SetActive(false);
        }
    }
    
    private void ResetIndicator()
    {
        indicatorPosition = 0f;
        
        // Asegurar que el indicador sea hijo de la barra
        if (indicatorTransform != null && barBackground != null)
        {
            RectTransform barRect = barBackground.GetComponent<RectTransform>();
            if (indicatorTransform.parent != barRect)
            {
                indicatorTransform.SetParent(barRect, false);
            }
            
            // Configurar desde el inicio
            indicatorTransform.anchorMin = new Vector2(0, 0.5f);
            indicatorTransform.anchorMax = new Vector2(0, 0.5f);
            indicatorTransform.pivot = new Vector2(0.5f, 0.5f);
            indicatorTransform.anchoredPosition = new Vector2(0, 0);
            
            // ASEGURAR que sea visible
            if (indicatorImage != null)
            {
                indicatorImage.enabled = true;
                indicatorImage.raycastTarget = false;
            }
        }
        
        Debug.Log("↺ Indicador reseteado y visible");
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    public void RestartGame()
    {
        // FORZAR limpieza total
        StopAllCoroutines();
        
        // Resetear variables
        currentStage = 0;
        livesRemaining = 3;
        gameEnded = false;
        canWater = true;
        isHoldingButton = false;
        indicatorPosition = 0f;
        
        // RESETEAR todos los paneles
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (wateringBarPanel != null) wateringBarPanel.SetActive(false);
        
        // RESETEAR escala de la planta
        if (plantImage != null)
        {
            plantImage.transform.localScale = Vector3.one;
            plantImage.transform.localRotation = Quaternion.identity;
            plantImage.transform.localPosition = Vector3.zero;
        }
        
        // Recargar escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
    
    private enum ZoneType
    {
        Bad,
        Good,
        Perfect
    }
}