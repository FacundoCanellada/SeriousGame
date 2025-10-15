using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarDisplay : MonoBehaviour
{
    [Header("UI Components")]
    public Image avatarIconImage;
    public TextMeshProUGUI playerNameText;
    
    [Header("Configuración")]
    public bool showPlayerName = true;
    public bool updateOnStart = true;
    
    private void Start()
    {
        if (updateOnStart)
        {
            UpdateDisplay();
        }
    }
    
    public void UpdateDisplay()
    {
        // Mostrar avatar
        if (avatarIconImage != null && AvatarManager.Instance != null)
        {
            Sprite avatarIcon = AvatarManager.Instance.GetSelectedAvatarIcon();
            if (avatarIcon != null)
            {
                avatarIconImage.sprite = avatarIcon;
                avatarIconImage.gameObject.SetActive(true);
            }
            else
            {
                avatarIconImage.gameObject.SetActive(false);
            }
        }
        
        // Mostrar nombre del jugador
        if (showPlayerName && playerNameText != null && UserDataManager.Instance != null)
        {
            string playerName = UserDataManager.Instance.GetUserData().playerName;
            if (!string.IsNullOrEmpty(playerName))
            {
                playerNameText.text = playerName;
                playerNameText.gameObject.SetActive(true);
            }
            else
            {
                playerNameText.text = "Jugador";
            }
        }
    }
    
    // Método público para actualizar desde otros scripts
    public void RefreshDisplay()
    {
        UpdateDisplay();
    }
}