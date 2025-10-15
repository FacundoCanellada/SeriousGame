using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarButton : MonoBehaviour
{
    [Header("UI Components")]
    public Image avatarImage;
    public TextMeshProUGUI avatarNameText;
    public Button button;
    public GameObject selectionFrame; // Marco que aparece cuando está seleccionado
    
    private AvatarData avatarData;
    private AvatarSelector selector;
    
    private void Start()
    {
        if (button == null)
            button = GetComponent<Button>();
            
        if (button != null)
            button.onClick.AddListener(OnButtonClick);
    }
    
    public void SetupAvatar(AvatarData data, AvatarSelector avatarSelector)
    {
        avatarData = data;
        selector = avatarSelector;
        
        // Configurar UI
        if (avatarImage != null)
            avatarImage.sprite = data.avatarSprite;
            
        if (avatarNameText != null)
            avatarNameText.text = data.avatarName;
            
        // Inicialmente no seleccionado
        SetSelected(false);
    }
    
    private void OnButtonClick()
    {
        if (selector != null && avatarData != null)
        {
            selector.OnAvatarSelected(avatarData.avatarId);
            
            // Actualizar selección visual en todos los botones
            AvatarButton[] allButtons = FindObjectsOfType<AvatarButton>();
            foreach (var btn in allButtons)
            {
                btn.SetSelected(btn == this);
            }
        }
    }
    
    public void SetSelected(bool selected)
    {
        if (selectionFrame != null)
            selectionFrame.SetActive(selected);
    }
}