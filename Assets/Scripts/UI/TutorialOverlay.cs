using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla una secuencia de carteles/tutorial (imágenes o paneles) que pueden:
///  - Aparecer al inicio de una escena.
///  - Avanzarse con "Siguiente".
///  - Saltarse por completo con "Saltar".
///  - Mostrar cada panel sólo una vez por sesión (opcional) o siempre.
/// Uso: Asignar en el inspector la lista de GameObjects (cada uno puede contener imagen, texto, etc).
/// Colocar los botones Next y Skip (si se desean) y llamar a ShowSequence() en Start o manualmente.
/// Para la escena de Avatar usa una sola tarjeta (index 0). Para Biologia, asigna tres tarjetas.
/// </summary>
public class TutorialOverlay : MonoBehaviour
{
    [Header("Carteles/Tarjetas en orden")]
    [Tooltip("Lista de paneles (GameObjects) que representan cada paso del tutorial.")]
    public GameObject[] steps;

    [Header("Botones")]
    public Button nextButton;
    public Button skipButton;

    [Header("Opciones")]
    [Tooltip("Si es true, se reproducirá sólo si no se ha marcado como visto (usa PlayerPrefs).")]
    public bool showOnlyOnce = false;
    [Tooltip("Clave única para PlayerPrefs si showOnlyOnce = true. Debe ser diferente por escena.")]
    public string playerPrefsKey = "Tutorial_Avatar";
    [Tooltip("Ocultar automaticamente el botón 'Siguiente' en el último paso.")]
    public bool hideNextOnLast = true;
    [Tooltip("Cerrar el tutorial automáticamente al terminar el último paso.")]
    public bool autoCloseOnEnd = true;
    [Tooltip("Activar por defecto al iniciar (Start). Si false, llamar a ShowSequence() manualmente.")]
    public bool showOnStart = true;

    [Header("Fade opcional (CanvasGroup)")]
    [Tooltip("CanvasGroup para permitir fade in/out opcional.")]
    public CanvasGroup canvasGroup;
    [Tooltip("Duración en segundos del fade.")]
    public float fadeDuration = 0.25f;

    [Header("Capas/Orden visual")]
    [Tooltip("Mover este objeto al último hijo del Canvas para asegurar que quede encima.")]
    public bool bringToFront = true;

    private int currentIndex = -1;
    private bool isShowing = false;
    private float fadeTimer = 0f;
    private bool fadingIn = false;
    private bool fadingOut = false;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        // Asegurar estado inicial oculto
        SetAllInactive();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void Start()
    {
        WireButtons();
        if (showOnStart)
        {
            TryShowSequence();
        }
    }

    private void Update()
    {
        HandleFade();
    }

    public void TryShowSequence()
    {
        if (showOnlyOnce && PlayerPrefs.GetInt(playerPrefsKey, 0) == 1)
        {
            // Ya visto, no mostrar
            return;
        }
        ShowSequence();
    }

    public void ShowSequence()
    {
        if (steps == null || steps.Length == 0) return;
        currentIndex = -1;
        isShowing = true;
        gameObject.SetActive(true);
        if (bringToFront)
        {
            transform.SetAsLastSibling();
        }
        // Pausar juego mientras el tutorial está activo
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            fadingIn = true;
            fadeTimer = 0f;
        }
        ShowNextInternal();
    }

    public void ShowNext()
    {
        if (!isShowing) return;
        ShowNextInternal();
    }

    private void ShowNextInternal()
    {
        // Apagar el actual
        if (currentIndex >= 0 && currentIndex < steps.Length)
        {
            steps[currentIndex].SetActive(false);
        }
        currentIndex++;
        if (currentIndex >= steps.Length)
        {
            EndSequence();
            return;
        }
        steps[currentIndex].SetActive(true);
        UpdateButtonsState();
    }

    public void SkipAll()
    {
        if (!isShowing) return;
        EndSequence();
    }

    private void EndSequence()
    {
        isShowing = false;
        if (showOnlyOnce)
        {
            PlayerPrefs.SetInt(playerPrefsKey, 1);
        }
        if (autoCloseOnEnd)
        {
            HideAll();
        }
        else
        {
            // Si no se auto-cierra, sólo oculta paneles
            SetAllInactive();
        }
        // Reanudar juego al terminar el tutorial
        Time.timeScale = previousTimeScale;
    }

    private void HideAll()
    {
        SetAllInactive();
        if (canvasGroup != null)
        {
            fadingOut = true;
            fadeTimer = 0f;
        }
        else
        {
            gameObject.SetActive(false);
        }
        // Reanudar juego inmediatamente si se oculta sin pasar por EndSequence
        Time.timeScale = previousTimeScale;
    }

    private void SetAllInactive()
    {
        if (steps == null) return;
        foreach (var s in steps)
        {
            if (s != null) s.SetActive(false);
        }
    }

    private void UpdateButtonsState()
    {
        if (nextButton != null)
        {
            bool last = currentIndex == steps.Length - 1;
            nextButton.gameObject.SetActive(!(hideNextOnLast && last));
        }
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(true); // Siempre visible mientras hay secuencia
        }
    }

    private void WireButtons()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(ShowNext);
            nextButton.onClick.AddListener(ShowNext);
        }
        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(SkipAll);
            skipButton.onClick.AddListener(SkipAll);
        }
    }

    private void HandleFade()
    {
        if (canvasGroup == null) return;
        if (fadingIn)
        {
            fadeTimer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration);
            canvasGroup.alpha = t;
            if (t >= 1f)
            {
                fadingIn = false;
            }
        }
        else if (fadingOut)
        {
            fadeTimer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration);
            canvasGroup.alpha = 1f - t;
            if (t >= 1f)
            {
                fadingOut = false;
                gameObject.SetActive(false);
            }
        }
    }
}
