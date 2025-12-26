using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestiona la interfaz de usuario (Vista) de las estad?sticas del jugador.
/// Su ?nica responsabilidad es suscribirse a los cambios del Modelo y 
/// actualizar los elementos visuales (Sliders) en consecuencia.
/// </summary>
public class CharacterStatsView : MonoBehaviour
{
    #region Referencias UI
    [Header("Elementos de Interfaz")]
    [Tooltip("Barra de UI que representa la salud del jugador.")]
    [SerializeField] private Slider healthSlider;

    [Tooltip("Barra de UI que representa la estamina del jugador.")]
    [SerializeField] private Slider staminaSlider;
    #endregion

    #region Atributos Privados
    private CharacterStatsModel _model;
    #endregion

    #region Ciclo de Vida
    private void Start()
    {
        // 1. Obtener el Modelo a trav?s del Controlador. 
        // Nota: Asume que el Controlador est? en el mismo GameObject.
        CharacterStatsController controller = GetComponent<CharacterStatsController>();

        if (controller == null)
        {
            Debug.LogError($"<color=red>Error:</color> No se encontr? CharacterStatsController en {gameObject.name}");
            return;
        }

        _model = controller.GetModel();

        // 2. Suscripci?n a eventos del Modelo (Patr?n Observer)
        _model.OnHealthChanged += UpdateHealthBar;
        _model.OnStaminaChanged += UpdateStaminaBar;

        // 3. Inicializaci?n: Ponemos las barras en su estado actual al iniciar el juego
        UpdateHealthBar(_model.GetHealthPercentage());
    }

    /// <summary>
    /// Es vital desvincular los eventos para evitar fugas de memoria y errores
    /// cuando el objeto es destruido o se cambia de escena.
    /// </summary>
    private void OnDestroy()
    {
        if (_model != null)
        {
            _model.OnHealthChanged -= UpdateHealthBar;
            _model.OnStaminaChanged -= UpdateStaminaBar;
        }
    }
    #endregion

    #region M?todos de Actualizaci?n Visual
    /// <summary>
    /// Actualiza el slider de estamina con el nuevo valor normalizado (0 a 1).
    /// </summary>
    private void UpdateStaminaBar(float normalizedValue)
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = normalizedValue;
            // Debug.Log($"UI Estamina: {normalizedValue * 100}%");
        }
    }

    /// <summary>
    /// Actualiza el slider de salud con el nuevo valor normalizado (0 a 1).
    /// </summary>
    private void UpdateHealthBar(float normalizedValue)
    {
        if (healthSlider != null)
        {
            healthSlider.value = normalizedValue;
            // Debug.Log($"UI Salud: {normalizedValue * 100}%");
        }
    }
    #endregion
}