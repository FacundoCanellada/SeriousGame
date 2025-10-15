using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [Header("Configuración de Navegación")]
    public bool checkRegistration = true;
    public string registrationSceneName = "RegistroScene";
    public string avatarSelectionSceneName = "AvatarSelection";
    
    // Este método se puede llamar desde el botón
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    // Nuevo método para el botón Play que verifica el registro
    public void OnPlayButtonClick()
    {
        if (checkRegistration && UserDataManager.Instance != null)
        {
            if (!UserDataManager.Instance.HasUserRegistered())
            {
                // Si no está registrado, ir a registro
                SceneManager.LoadScene(registrationSceneName);
            }
            else
            {
                // Si ya está registrado, ir directo a selección de avatar
                SceneManager.LoadScene(avatarSelectionSceneName);
            }
        }
        else
        {
            // Si no hay verificación, ir directo al tablero
            SceneManager.LoadScene("Tablero");
        }
    }
}
