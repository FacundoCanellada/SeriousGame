using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Configuración del Juego")]
    public bool enableVocationalTracking = true;
    public bool debugMode = false;
    
    [Header("Escenas")]
    public string menuSceneName = "Menu";
    public string registrationSceneName = "RegistroScene";
    public string avatarSelectionSceneName = "AvatarSelection";
    public string tableroSceneName = "Tablero";
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeGame()
    {
        // Asegurar que el UserDataManager esté inicializado
        if (UserDataManager.Instance == null)
        {
            GameObject userDataManagerGO = new GameObject("UserDataManager");
            userDataManagerGO.AddComponent<UserDataManager>();
        }
        
        if (debugMode)
        {
            Debug.Log("GameManager inicializado correctamente");
        }
    }
    
    public void RestartGame()
    {
        if (UserDataManager.Instance != null)
        {
            UserDataManager.Instance.ClearUserData();
        }
        
        SceneManager.LoadScene(menuSceneName);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void LogVocationalAction(VocationalArea area, float points, string action = "")
    {
        if (!enableVocationalTracking || UserDataManager.Instance == null) return;
        
        UserData userData = UserDataManager.Instance.GetUserData();
        userData.vocationalData.AddScore(area, points);
        UserDataManager.Instance.UpdateUserData(userData);
        
        if (debugMode)
        {
            Debug.Log($"Acción vocacional registrada: {area} +{points} puntos ({action})");
        }
    }
    
    // Método para facilitar la navegación desde cualquier script
    public void NavigateToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}