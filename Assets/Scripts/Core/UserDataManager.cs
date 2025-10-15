using UnityEngine;
using System.IO;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager Instance { get; private set; }
    
    [Header("Configuración")]
    public bool useJsonSave = true; // Si false, usa PlayerPrefs
    
    private UserData currentUserData;
    private string saveFilePath;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUserData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeUserData()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "userdata.json");
        LoadUserData();
    }
    
    public UserData GetUserData()
    {
        return currentUserData;
    }
    
    public void SaveUserData()
    {
        if (useJsonSave)
        {
            SaveToJson();
        }
        else
        {
            SaveToPlayerPrefs();
        }
        
        Debug.Log("Datos del usuario guardados correctamente");
    }
    
    public void LoadUserData()
    {
        if (useJsonSave)
        {
            LoadFromJson();
        }
        else
        {
            LoadFromPlayerPrefs();
        }
    }
    
    private void SaveToJson()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(currentUserData, true);
            File.WriteAllText(saveFilePath, jsonData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error guardando datos: " + e.Message);
        }
    }
    
    private void LoadFromJson()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                string jsonData = File.ReadAllText(saveFilePath);
                currentUserData = JsonUtility.FromJson<UserData>(jsonData);
            }
            else
            {
                currentUserData = new UserData();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error cargando datos: " + e.Message);
            currentUserData = new UserData();
        }
    }
    
    private void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetString("UserName", currentUserData.playerName);
        PlayerPrefs.SetInt("UserAge", currentUserData.age);
        PlayerPrefs.SetString("UserEmail", currentUserData.email);
        PlayerPrefs.SetInt("SelectedAvatar", currentUserData.selectedAvatarId);
        PlayerPrefs.SetInt("HasRegistered", currentUserData.hasCompletedRegistration ? 1 : 0);
        
        // Guardar intereses
        PlayerPrefs.SetInt("InterestScience", currentUserData.interesCiencias ? 1 : 0);
        PlayerPrefs.SetInt("InterestArt", currentUserData.interesArte ? 1 : 0);
        PlayerPrefs.SetInt("InterestTech", currentUserData.interesTecnologia ? 1 : 0);
        PlayerPrefs.SetInt("InterestLiterature", currentUserData.interesLiteratura ? 1 : 0);
        PlayerPrefs.SetInt("InterestSports", currentUserData.interesDeportes ? 1 : 0);
        PlayerPrefs.SetInt("InterestMedicine", currentUserData.interesMedicina ? 1 : 0);
        
        PlayerPrefs.Save();
    }
    
    private void LoadFromPlayerPrefs()
    {
        currentUserData = new UserData();
        currentUserData.playerName = PlayerPrefs.GetString("UserName", "");
        currentUserData.age = PlayerPrefs.GetInt("UserAge", 0);
        currentUserData.email = PlayerPrefs.GetString("UserEmail", "");
        currentUserData.selectedAvatarId = PlayerPrefs.GetInt("SelectedAvatar", 0);
        currentUserData.hasCompletedRegistration = PlayerPrefs.GetInt("HasRegistered", 0) == 1;
        
        // Cargar intereses
        currentUserData.interesCiencias = PlayerPrefs.GetInt("InterestScience", 0) == 1;
        currentUserData.interesArte = PlayerPrefs.GetInt("InterestArt", 0) == 1;
        currentUserData.interesTecnologia = PlayerPrefs.GetInt("InterestTech", 0) == 1;
        currentUserData.interesLiteratura = PlayerPrefs.GetInt("InterestLiterature", 0) == 1;
        currentUserData.interesDeportes = PlayerPrefs.GetInt("InterestSports", 0) == 1;
        currentUserData.interesMedicina = PlayerPrefs.GetInt("InterestMedicine", 0) == 1;
    }
    
    public bool HasUserRegistered()
    {
        return currentUserData != null && currentUserData.hasCompletedRegistration;
    }
    
    public void CompleteRegistration()
    {
        currentUserData.hasCompletedRegistration = true;
        SaveUserData();
    }
    
    public void UpdateUserData(UserData newData)
    {
        currentUserData = newData;
        SaveUserData();
    }
    
    // Método para testing - eliminar datos
    [ContextMenu("Eliminar Datos de Usuario")]
    public void ClearUserData()
    {
        if (useJsonSave && File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
        
        currentUserData = new UserData();
        Debug.Log("Datos de usuario eliminados");
    }
}