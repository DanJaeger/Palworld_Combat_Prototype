using UnityEngine;
using System.Collections;

/// <summary>
/// Act?a como el Controlador (MVC) de las estad?sticas del jugador.
/// Gestiona la comunicaci?n entre el Modelo y Unity, controlando procesos temporales
/// como la regeneraci?n de estamina mediante corrutinas.
/// </summary>
public class CharacterStatsController : MonoBehaviour
{
    #region Atributos Privados
    private CharacterStatsModel _model;
    #endregion

    #region Configuraci?n
    [Header("Configuracion Base")]
    [Tooltip("Plantilla de datos iniciales para el modelo.")]
    [SerializeField] private CharacterStatsSO statsTemplate;
    #endregion

    private void Awake()
    {
        // Inicializaci?n del Modelo con los datos del ScriptableObject
        _model = new CharacterStatsModel(
            statsTemplate.InitialHealth,
            statsTemplate.MaxHealth);
    }

    /// <summary>
    /// Expone el modelo para que la Vista pueda suscribirse a sus eventos.
    /// </summary>
    public CharacterStatsModel GetModel() => _model;
}