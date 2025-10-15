using UnityEngine;
using System.Collections.Generic;

public class AvatarManager : MonoBehaviour
{
    public static AvatarManager Instance { get; private set; }
    
    [Header("Configuración de Avatares")]
    public List<AvatarData> availableAvatars = new List<AvatarData>();
    
    [Header("Debug")]
    public bool debugMode = false;
    
    private int selectedAvatarId = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAvatars();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeAvatars()
    {
        // Cargar avatar seleccionado si existe
        if (UserDataManager.Instance != null)
        {
            selectedAvatarId = UserDataManager.Instance.GetUserData().selectedAvatarId;
        }
        
        if (debugMode)
        {
            Debug.Log($"AvatarManager inicializado. Avatar seleccionado: {selectedAvatarId}");
        }
    }
    
    public AvatarData GetAvatarById(int id)
    {
        return availableAvatars.Find(avatar => avatar.avatarId == id);
    }
    
    public AvatarData GetSelectedAvatar()
    {
        return GetAvatarById(selectedAvatarId);
    }
    
    public void SelectAvatar(int avatarId)
    {
        selectedAvatarId = avatarId;
        
        // Guardar en UserData
        if (UserDataManager.Instance != null)
        {
            UserData userData = UserDataManager.Instance.GetUserData();
            userData.selectedAvatarId = avatarId;
            
            // Agregar puntos vocacionales basados en el avatar elegido
            AvatarData selectedAvatar = GetAvatarById(avatarId);
            if (selectedAvatar != null)
            {
                userData.vocationalData.AddScore(selectedAvatar.primaryArea, 5f);
                userData.vocationalData.AddScore(selectedAvatar.secondaryArea, 3f);
            }
            
            UserDataManager.Instance.UpdateUserData(userData);
        }
        
        if (debugMode)
        {
            Debug.Log($"Avatar seleccionado: {avatarId}");
        }
    }
    
    public List<AvatarData> GetAllAvatars()
    {
        return availableAvatars;
    }
    
    public Sprite GetSelectedAvatarIcon()
    {
        AvatarData selected = GetSelectedAvatar();
        return selected?.avatarIcon;
    }
    
    public Sprite GetSelectedAvatarSprite()
    {
        AvatarData selected = GetSelectedAvatar();
        return selected?.avatarSprite;
    }
    
    public string GetSelectedAvatarName()
    {
        AvatarData selected = GetSelectedAvatar();
        return selected?.avatarName ?? "Sin Avatar";
    }
    
    // Método para testing
    [ContextMenu("Log Selected Avatar Info")]
    public void LogSelectedAvatarInfo()
    {
        AvatarData selected = GetSelectedAvatar();
        if (selected != null)
        {
            Debug.Log($"Avatar: {selected.avatarName} (ID: {selected.avatarId}) - {selected.description}");
        }
        else
        {
            Debug.Log("No hay avatar seleccionado");
        }
    }
}