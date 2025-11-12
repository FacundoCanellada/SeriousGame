using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;   // Instancia global (singleton)
    public AudioSource audioSource;        // Asigná tu AudioSource desde el inspector

    void Awake()
    {
        // Si ya existe una instancia anterior, destruye la nueva para evitar duplicados
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Asigna esta como la instancia principal
        Instance = this;
        DontDestroyOnLoad(gameObject); // 🔥 Mantiene este objeto al cambiar de escena
    }

    // Método para reproducir un sonido (opcional)
    public void PlaySound(AudioClip clip, bool loop = false)
    {
        if (audioSource == null) return;

        if (audioSource.clip == clip && audioSource.isPlaying)
            return; // Evita reiniciar si ya está sonando el mismo clip

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    // Método para detener la música
    public void StopSound()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
