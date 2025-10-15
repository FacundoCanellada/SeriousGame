using System;
using UnityEngine;

[System.Serializable]
public class UserData
{
    [Header("Información Personal")]
    public string playerName;
    public int age;
    public string email;
    
    [Header("Intereses Iniciales")]
    public bool interesCiencias;
    public bool interesArte;
    public bool interesTecnologia;
    public bool interesLiteratura;
    public bool interesDeportes;
    public bool interesMedicina;
    
    [Header("Avatar")]
    public int selectedAvatarId = 0;
    
    [Header("Progreso Vocacional")]
    public VocationalScores vocationalData;
    
    [Header("Configuración")]
    public DateTime registrationDate;
    public bool hasCompletedRegistration = false;
    
    public UserData()
    {
        registrationDate = DateTime.Now;
        vocationalData = new VocationalScores();
    }
}

[System.Serializable]
public class VocationalScores
{
    [Header("Puntajes por Área")]
    public float cienciasNaturales = 0f;
    public float artesCreativas = 0f;
    public float tecnologiaIngenieria = 0f;
    public float humanidadesLiteratura = 0f;
    public float saludMedicina = 0f;
    public float deportesActividad = 0f;
    
    [Header("Habilidades Observadas")]
    public float paciencia = 0f;
    public float creatividad = 0f;
    public float logica = 0f;
    public float liderazgo = 0f;
    public float atencionDetalle = 0f;
    
    public void AddScore(VocationalArea area, float points)
    {
        switch (area)
        {
            case VocationalArea.CienciasNaturales:
                cienciasNaturales += points;
                break;
            case VocationalArea.ArtesCreativas:
                artesCreativas += points;
                break;
            case VocationalArea.TecnologiaIngenieria:
                tecnologiaIngenieria += points;
                break;
            case VocationalArea.HumanidadesLiteratura:
                humanidadesLiteratura += points;
                break;
            case VocationalArea.SaludMedicina:
                saludMedicina += points;
                break;
            case VocationalArea.DeportesActividad:
                deportesActividad += points;
                break;
        }
    }
}

public enum VocationalArea
{
    CienciasNaturales,
    ArtesCreativas,
    TecnologiaIngenieria,
    HumanidadesLiteratura,
    SaludMedicina,
    DeportesActividad
}