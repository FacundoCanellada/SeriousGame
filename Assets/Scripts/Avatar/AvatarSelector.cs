using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class AvatarSelector : MonoBehaviour
{
    [Header("UI Referencias")]
    public Transform avatarGrid; // Parent donde van los botones de avatar
    public GameObject avatarButtonPrefab; // Prefab del botón de avatar
    
    [Header("Preview del Avatar")]
    public Image avatarPreviewImage;
    public TextMeshProUGUI avatarNameText;
    public TextMeshProUGUI avatarDescriptionText;
    
    [Header("Botones de Navegación")]
    public Button confirmButton;
    public Button backButton;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip selectSound;
    public AudioClip confirmSound;
    
    [Header("Configuración")]
    public string nextSceneName = "Tablero";
    public string backSceneName = "RegistroScene";
    
    private int currentSelectedId = -1;
    
    private void Start()
    {
        InitializeAvatarSelection();
        SetupButtons();
    }
    
    private void InitializeAvatarSelection()
    {
        if (AvatarManager.Instance == null)
        {
            Debug.LogError("AvatarManager no encontrado!");
            return;
        }
        
        CreateAvatarButtons();
        
        // Seleccionar el avatar actual si existe
        if (UserDataManager.Instance != null)
        {
            int savedAvatarId = UserDataManager.Instance.GetUserData().selectedAvatarId;
            if (savedAvatarId >= 0)
            {
                SelectAvatarById(savedAvatarId);
            }
        }
        
        UpdateConfirmButton();
    }
    
    private void CreateAvatarButtons()
    {
        if (avatarGrid == null || avatarButtonPrefab == null) return;
        
        // Limpiar botones existentes
        foreach (Transform child in avatarGrid)
        {
            Destroy(child.gameObject);
        }
        
        // Crear botón para cada avatar
        var avatars = AvatarManager.Instance.GetAllAvatars();
        
        foreach (var avatar in avatars)
        {
            GameObject buttonGO = Instantiate(avatarButtonPrefab, avatarGrid);
            AvatarButton avatarButton = buttonGO.GetComponent<AvatarButton>();
            
            if (avatarButton != null)
            {
                avatarButton.SetupAvatar(avatar, this);
            }
        }
    }
    
    public void OnAvatarSelected(int avatarId)
    {
        currentSelectedId = avatarId;
        UpdatePreview();
        UpdateConfirmButton();
        PlaySound(selectSound);
    }
    
    private void SelectAvatarById(int avatarId)
    {
        currentSelectedId = avatarId;
        UpdatePreview();
        UpdateConfirmButton();
    }
    
    private void UpdatePreview()
    {
        if (currentSelectedId < 0) return;
        
        AvatarData avatar = AvatarManager.Instance.GetAvatarById(currentSelectedId);
        if (avatar == null) return;
        
        if (avatarPreviewImage != null)
            avatarPreviewImage.sprite = avatar.avatarSprite;
            
        if (avatarNameText != null)
            avatarNameText.text = avatar.avatarName;
            
        if (avatarDescriptionText != null)
            avatarDescriptionText.text = avatar.description;
    }
    
    private void UpdateConfirmButton()
    {
        if (confirmButton != null)
        {
            confirmButton.interactable = (currentSelectedId >= 0);
        }
    }
    
    private void SetupButtons()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButtonClick);
            
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClick);
    }
    
    public void OnConfirmButtonClick()
    {
        if (currentSelectedId < 0) return;
        
        // Guardar selección
        AvatarManager.Instance.SelectAvatar(currentSelectedId);
        
        PlaySound(confirmSound);
        StartCoroutine(LoadNextSceneAfterDelay());
    }
    
    public void OnBackButtonClick()
    {
        SceneManager.LoadScene(backSceneName);
    }
    
    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nextSceneName);
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}