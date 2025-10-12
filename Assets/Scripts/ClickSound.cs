using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    public AudioClip clickSound; // Asigna el sonido desde el Inspector
    public AudioSource audioSource; // Arrastra aquí un AudioSource desde la escena

    private void Start()
    {
        // Obtiene el botón y le añade un listener
        Button button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogWarning("Falta el AudioSource o el AudioClip en " + gameObject.name);
        }
    }
}
