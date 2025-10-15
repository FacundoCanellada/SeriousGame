using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class RegistrationForm : MonoBehaviour
{
    [Header("Campos del Formulario")]
    public TMP_InputField nameInputField;
    public TMP_InputField ageInputField;
    public TMP_InputField emailInputField;
    
    [Header("Toggles de Intereses")]
    public Toggle scienceToggle;
    public Toggle artToggle;
    public Toggle techToggle;
    public Toggle literatureToggle;
    public Toggle sportsToggle;
    public Toggle medicineToggle;
    
    [Header("Botones")]
    public Button submitButton;
    public Button skipButton;
    
    [Header("UI Feedback")]
    public TextMeshProUGUI errorMessageText;
    public GameObject loadingPanel;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successSound;
    public AudioClip errorSound;
    
    [Header("Navegación")]
    public string nextSceneName = "AvatarSelection";
    public string skipSceneName = "Tablero";
    
    private UserData tempUserData;
    
    private void Start()
    {
        InitializeForm();
        SetupButtons();
    }
    
    private void InitializeForm()
    {
        tempUserData = new UserData();
        
        // Verificar si ya hay datos guardados
        if (UserDataManager.Instance != null)
        {
            UserData existingData = UserDataManager.Instance.GetUserData();
            if (existingData != null && !string.IsNullOrEmpty(existingData.playerName))
            {
                LoadExistingData(existingData);
            }
        }
        
        // Inicializar UI
        if (errorMessageText != null)
            errorMessageText.gameObject.SetActive(false);
            
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }
    
    private void SetupButtons()
    {
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitButtonClick);
            
        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkipButtonClick);
    }
    
    private void LoadExistingData(UserData data)
    {
        if (nameInputField != null) nameInputField.text = data.playerName;
        if (ageInputField != null) ageInputField.text = data.age > 0 ? data.age.ToString() : "";
        if (emailInputField != null) emailInputField.text = data.email;
        
        if (scienceToggle != null) scienceToggle.isOn = data.interesCiencias;
        if (artToggle != null) artToggle.isOn = data.interesArte;
        if (techToggle != null) techToggle.isOn = data.interesTecnologia;
        if (literatureToggle != null) literatureToggle.isOn = data.interesLiteratura;
        if (sportsToggle != null) sportsToggle.isOn = data.interesDeportes;
        if (medicineToggle != null) medicineToggle.isOn = data.interesMedicina;
    }
    
    public void OnSubmitButtonClick()
    {
        if (ValidateForm())
        {
            CollectFormData();
            StartCoroutine(SubmitRegistration());
        }
        else
        {
            PlaySound(errorSound);
        }
    }
    
    public void OnSkipButtonClick()
    {
        PlaySound(successSound);
        StartCoroutine(LoadSceneAfterDelay(skipSceneName, 0.5f));
    }
    
    private bool ValidateForm()
    {
        string errorMessage = "";
        
        if (string.IsNullOrEmpty(nameInputField.text.Trim()))
        {
            errorMessage = "Por favor, ingresa tu nombre";
        }
        else if (string.IsNullOrEmpty(ageInputField.text.Trim()))
        {
            errorMessage = "Por favor, ingresa tu edad";
        }
        else if (!int.TryParse(ageInputField.text, out int age) || age < 10 || age > 100)
        {
            errorMessage = "Por favor, ingresa una edad válida (10-100)";
        }
        else if (!string.IsNullOrEmpty(emailInputField.text) && !IsValidEmail(emailInputField.text))
        {
            errorMessage = "Por favor, ingresa un email válido o déjalo vacío";
        }
        
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return false;
        }
        
        HideError();
        return true;
    }
    
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    private void CollectFormData()
    {
        tempUserData.playerName = nameInputField.text.Trim();
        tempUserData.age = int.Parse(ageInputField.text);
        tempUserData.email = emailInputField.text.Trim();
        
        tempUserData.interesCiencias = scienceToggle != null && scienceToggle.isOn;
        tempUserData.interesArte = artToggle != null && artToggle.isOn;
        tempUserData.interesTecnologia = techToggle != null && techToggle.isOn;
        tempUserData.interesLiteratura = literatureToggle != null && literatureToggle.isOn;
        tempUserData.interesDeportes = sportsToggle != null && sportsToggle.isOn;
        tempUserData.interesMedicina = medicineToggle != null && medicineToggle.isOn;
        
        // Dar puntos iniciales basados en intereses declarados
        if (tempUserData.interesCiencias)
            tempUserData.vocationalData.AddScore(VocationalArea.CienciasNaturales, 10f);
        if (tempUserData.interesArte)
            tempUserData.vocationalData.AddScore(VocationalArea.ArtesCreativas, 10f);
        if (tempUserData.interesTecnologia)
            tempUserData.vocationalData.AddScore(VocationalArea.TecnologiaIngenieria, 10f);
        if (tempUserData.interesLiteratura)
            tempUserData.vocationalData.AddScore(VocationalArea.HumanidadesLiteratura, 10f);
        if (tempUserData.interesDeportes)
            tempUserData.vocationalData.AddScore(VocationalArea.DeportesActividad, 10f);
        if (tempUserData.interesMedicina)
            tempUserData.vocationalData.AddScore(VocationalArea.SaludMedicina, 10f);
    }
    
    private IEnumerator SubmitRegistration()
    {
        // Mostrar loading
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
            
        // Deshabilitar botón
        if (submitButton != null)
            submitButton.interactable = false;
            
        // Simular procesamiento
        yield return new WaitForSeconds(1f);
        
        // Guardar datos
        if (UserDataManager.Instance != null)
        {
            UserDataManager.Instance.UpdateUserData(tempUserData);
            UserDataManager.Instance.CompleteRegistration();
        }
        
        PlaySound(successSound);
        
        // Esperar un poco más para el feedback visual
        yield return new WaitForSeconds(0.5f);
        
        // Cargar siguiente escena
        SceneManager.LoadScene(nextSceneName);
    }
    
    private void ShowError(string message)
    {
        if (errorMessageText != null)
        {
            errorMessageText.text = message;
            errorMessageText.gameObject.SetActive(true);
        }
    }
    
    private void HideError()
    {
        if (errorMessageText != null)
        {
            errorMessageText.gameObject.SetActive(false);
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}