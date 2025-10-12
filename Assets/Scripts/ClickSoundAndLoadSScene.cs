using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlaySoundAndLoadScene : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioSource audioSource;
    public string sceneNameToLoad;

    public void OnButtonClick()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            StartCoroutine(LoadSceneAfterSound(clickSound.length));
        }
        else
        {
            Debug.LogWarning("Falta asignar el AudioSource o el ClickSound.");
            SceneManager.LoadScene(sceneNameToLoad); // Por si falta el sonido
        }
    }

    private System.Collections.IEnumerator LoadSceneAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneNameToLoad);
    }
}
