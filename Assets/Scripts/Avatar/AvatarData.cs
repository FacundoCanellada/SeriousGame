using UnityEngine;

[System.Serializable]
public class AvatarData
{
    [Header("Información del Avatar")]
    public int avatarId;
    public string avatarName;
    public Sprite avatarSprite;
    public Sprite avatarIcon; // Para mostrar en el tablero
    
    [Header("Descripción")]
    [TextArea(2, 4)]
    public string description;
    
    [Header("Características Vocacionales")]
    public VocationalArea primaryArea;
    public VocationalArea secondaryArea;
    
    public AvatarData()
    {
        avatarId = 0;
        avatarName = "";
        description = "";
        primaryArea = VocationalArea.CienciasNaturales;
        secondaryArea = VocationalArea.ArtesCreativas;
    }
}