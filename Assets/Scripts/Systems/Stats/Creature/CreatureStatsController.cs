using UnityEngine;
using System.Collections;

/// <summary>
/// Actua como el Controlador (MVC) de las estad?sticas del jugador.
/// Gestiona la comunicaci?n entre el Modelo y Unity, controlando procesos temporales
/// como la regeneraci?n de estamina mediante corrutinas.
/// </summary>
public class CreatureStatsController : MonoBehaviour
{
    #region Atributos Privados
    private CreatureStatsModel _model;
    private Coroutine _regenCoroutine;
    #endregion

    #region Configuracion
    [Header("Configuracion Base")]
    [Tooltip("Plantilla de datos iniciales para el modelo.")]
    [SerializeField] private CreatureStatsSO statsTemplate;
    #endregion

    private void Awake()
    {
        // Inicializaci?n del Modelo con los datos del ScriptableObject
        _model = new CreatureStatsModel(
            statsTemplate.InitialHealth,
            statsTemplate.MaxHealth);
    }

    /// <summary>
    /// Expone el modelo para que la Vista pueda suscribirse a sus eventos.
    /// </summary>
    public CreatureStatsModel GetModel() => _model;
}