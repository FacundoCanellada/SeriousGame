using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlantGrowthGame : MonoBehaviour
{
    [Header("Estados de la Planta")]
    public Sprite[] plantSprites; // 0=semilla, 1=brote, 2=planta adulta
    public Image plantImage;
    
    [Header("UI del Juego")]
    public Button waterButton;
    public TextMeshProUGUI instructionsText;
    public TextMeshProUGUI progressText;
    public GameObject completePanel;
    public Button nextButton;
    public Button restartButton;
    
    [Header("Efectos Visuales")]
    public GameObject waterDropEffect; // Partículas o sprite de gotitas
    public float waterAnimationDuration = 1f;
    public float growthAnimationDuration = 2f;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip waterSound;
    public AudioClip growthSound;
    public AudioClip completeSound;
    
    [Header("Configuración del Juego")]
    public int waterClicksNeeded = 5; // Clicks para pasar al siguiente nivel
    public float plantTiltAngle = 15f; // Grados de inclinación
    public float tiltDuration = 0.5f;
    
    [Header("Navegación")]
    public string nextSceneName = "Tablero";
    
    // Variables del estado del juego
    private int currentPlantStage = 0; // 0, 1, 2
    private int currentWaterClicks = 0;
    private bool isWatering = false;
    private bool gameComplete = false;
    
    // Para tracking vocacional
    private float patience = 0f;
    private float persistence = 0f;
    private int totalClicks = 0;
    private float gameStartTime;
    
    private void Start()
    {
        InitializeGame();
        SetupUI();
        gameStartTime = Time.time;
    }
    
    private void InitializeGame()
    {
        // Configurar estado inicial
        currentPlantStage = 0;
        currentWaterClicks = 0;
        gameComplete = false;
        
        // Mostrar primera fase de la planta
        UpdatePlantDisplay();
        UpdateUI();
        
        // Ocultar panel de completado
        if (completePanel != null)
            completePanel.SetActive(false);
    }
    
    private void SetupUI()
    {
        if (waterButton != null)
            waterButton.onClick.AddListener(OnWaterButtonClick);
            
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextButtonClick);
            
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonClick);
    }
    
    public void OnWaterButtonClick()
    {
        if (isWatering || gameComplete) return;
        
        StartCoroutine(WaterPlant());
    }
    
    // Método alternativo para tap directo en la planta
    private void Update()
    {
        if (gameComplete || isWatering) return;
        
        // Detectar tap/click en la planta
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 inputPosition = Input.mousePosition;
            if (Input.touchCount > 0)
                inputPosition = Input.GetTouch(0).position;
                
            // Verificar si tocó la planta
            if (IsTouchingPlant(inputPosition))
            {
                OnWaterButtonClick();
            }
        }
    }
    
    private bool IsTouchingPlant(Vector2 screenPosition)
    {
        if (plantImage == null) return false;
        
        RectTransform plantRect = plantImage.GetComponent<RectTransform>();
        if (plantRect == null) return false;
        
        return RectTransformUtility.RectangleContainsScreenPoint(plantRect, screenPosition, Camera.main);
    }
    
    private IEnumerator WaterPlant()
    {
        isWatering = true;
        totalClicks++;
        currentWaterClicks++;
        
        // Registrar paciencia (tiempo entre clicks)
        patience += Time.deltaTime;
        
        // Efectos de riego
        PlayWaterEffects();
        
        // Inclinar planta
        yield return StartCoroutine(TiltPlant());
        
        // Verificar si puede crecer
        if (currentWaterClicks >= waterClicksNeeded && currentPlantStage < plantSprites.Length - 1)
        {
            yield return StartCoroutine(GrowPlant());
            currentWaterClicks = 0; // Reset para siguiente etapa
        }
        
        UpdateUI();
        
        // Verificar si terminó el juego
        if (currentPlantStage >= plantSprites.Length - 1)
        {
            yield return new WaitForSeconds(1f);
            CompleteGame();
        }
        
        isWatering = false;
    }
    
    private void PlayWaterEffects()
    {
        // Sonido
        if (audioSource != null && waterSound != null)
            audioSource.PlayOneShot(waterSound);
            
        // Efecto visual de agua
        if (waterDropEffect != null)
        {
            GameObject effect = Instantiate(waterDropEffect, plantImage.transform.position, Quaternion.identity);
            Destroy(effect, waterAnimationDuration);
        }
    }
    
    private IEnumerator TiltPlant()
    {
        if (plantImage == null) yield break;
        
        Vector3 originalRotation = plantImage.transform.eulerAngles;
        Vector3 tiltRotation = originalRotation + new Vector3(0, 0, plantTiltAngle);
        
        // Inclinar hacia la derecha
        float elapsed = 0f;
        while (elapsed < tiltDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (tiltDuration / 2);
            plantImage.transform.eulerAngles = Vector3.Lerp(originalRotation, tiltRotation, progress);
            yield return null;
        }
        
        // Volver a la posición original
        elapsed = 0f;
        while (elapsed < tiltDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (tiltDuration / 2);
            plantImage.transform.eulerAngles = Vector3.Lerp(tiltRotation, originalRotation, progress);
            yield return null;
        }
        
        plantImage.transform.eulerAngles = originalRotation;
    }
    
    private IEnumerator GrowPlant()
    {
        // Sonido de crecimiento
        if (audioSource != null && growthSound != null)
            audioSource.PlayOneShot(growthSound);
            
        // Cambiar sprite con animación
        currentPlantStage++;
        
        // Efecto de escalado para simular crecimiento
        Vector3 originalScale = plantImage.transform.localScale;
        Vector3 bigScale = originalScale * 1.2f;
        
        // Crecer
        float elapsed = 0f;
        while (elapsed < growthAnimationDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (growthAnimationDuration / 2);
            plantImage.transform.localScale = Vector3.Lerp(originalScale, bigScale, progress);
            yield return null;
        }
        
        // Cambiar sprite
        UpdatePlantDisplay();
        
        // Volver al tamaño normal
        elapsed = 0f;
        while (elapsed < growthAnimationDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (growthAnimationDuration / 2);
            plantImage.transform.localScale = Vector3.Lerp(bigScale, originalScale, progress);
            yield return null;
        }
        
        plantImage.transform.localScale = originalScale;
        
        // Registrar persistencia
        persistence += 10f; // Bonus por completar etapa
    }
    
    private void UpdatePlantDisplay()
    {
        if (plantImage != null && plantSprites != null && currentPlantStage < plantSprites.Length)
        {
            plantImage.sprite = plantSprites[currentPlantStage];
        }
    }
    
    private void UpdateUI()
    {
        // Actualizar instrucciones
        if (instructionsText != null)
        {
            switch (currentPlantStage)
            {
                case 0:
                    instructionsText.text = "¡Riega la semilla para que crezca!";
                    break;
                case 1:
                    instructionsText.text = "¡Sigue regando el brote!";
                    break;
                case 2:
                    instructionsText.text = "¡Tu planta está creciendo hermosa!";
                    break;
            }
        }
        
        // Actualizar progreso
        if (progressText != null)
        {
            if (currentPlantStage < plantSprites.Length - 1)
            {
                progressText.text = $"Riegos: {currentWaterClicks}/{waterClicksNeeded}";
            }
            else
            {
                progressText.text = "¡Completado!";
            }
        }
        
        // Habilitar/deshabilitar botón de riego
        if (waterButton != null)
        {
            waterButton.interactable = !gameComplete;
        }
    }
    
    private void CompleteGame()
    {
        gameComplete = true;
        
        // Sonido de completado
        if (audioSource != null && completeSound != null)
            audioSource.PlayOneShot(completeSound);
            
        // Mostrar panel de completado
        if (completePanel != null)
            completePanel.SetActive(true);
            
        // Calcular y guardar datos vocacionales
        SaveVocationalData();
        
        UpdateUI();
    }
    
    private void SaveVocationalData()
    {
        if (GameManager.Instance == null) return;
        
        float gameTime = Time.time - gameStartTime;
        
        // Calcular scores basados en comportamiento
        float patienceScore = Mathf.Clamp(patience / gameTime, 0f, 10f); // Más paciencia = mayor score
        float persistenceScore = Mathf.Clamp(persistence, 0f, 15f);
        float efficiencyScore = Mathf.Clamp(30f / totalClicks, 0f, 10f); // Menos clicks = más eficiente
        
        // Registrar en sistema vocacional
        GameManager.Instance.LogVocationalAction(VocationalArea.CienciasNaturales, 
            patienceScore + persistenceScore, "Completó minijuego de planta");
            
        // También sumar a habilidades específicas
        if (UserDataManager.Instance != null)
        {
            UserData userData = UserDataManager.Instance.GetUserData();
            userData.vocationalData.paciencia += patienceScore;
            userData.vocationalData.atencionDetalle += efficiencyScore;
            UserDataManager.Instance.UpdateUserData(userData);
        }
        
        Debug.Log($"Datos guardados - Paciencia: {patienceScore:F1}, Persistencia: {persistenceScore:F1}, Eficiencia: {efficiencyScore:F1}");
    }
    
    public void OnNextButtonClick()
    {
        SceneManager.LoadScene(nextSceneName);
    }
    
    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}