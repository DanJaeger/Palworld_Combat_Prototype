using System;

/// <summary>
/// Representa los datos puros y las reglas de negocio de los atributos del jugador.
/// Esta clase es independiente de Unity (Model), encarg?ndose ?nicamente del 
/// almacenamiento, validaci?n (clamping) y notificaci?n de cambios en vida y estamina.
/// </summary>
public class CharacterStatsModel
{
    #region Atributos Privados
    private float _health;
    private float _stamina;
    private readonly float _maxHealth;
    private readonly float _maxStamina;
    #endregion

    #region Eventos (Observer)
    /// <summary> Se dispara cuando la vida cambia, enviando el porcentaje actual (0 a 1). </summary>
    public Action<float> OnHealthChanged;

    /// <summary> Se dispara cuando la estamina cambia, enviando el porcentaje actual (0 a 1). </summary>
    public Action<float> OnStaminaChanged;
    #endregion

    #region Propiedades P?blicas
    public float Health => _health;
    public float Stamina => _stamina;
    #endregion

    /// <summary>
    /// Constructor del modelo. Inicializa valores y aplica l?mites de seguridad.
    /// </summary>
    public CharacterStatsModel(float initHealth, float maxH)
    {
        _maxHealth = maxH;

        // Usamos los m?todos Set para asegurar que el valor inicial est? dentro de los l?mites
        SetHealth(initHealth);
    }
    /// <summary> Devuelve el porcentaje de vida actual para uso en barras de interfaz. </summary>
    public float GetHealthPercentage() => _health / Math.Max(_maxHealth, 0.0001f);

    #region M?todos de Modificaci?n (L?gica de Negocio)
    /// <summary>
    /// Actualiza la vida aplicando l?mites y notificando a los suscriptores si hubo cambios.
    /// </summary>
    public void SetHealth(float value)
    {
        float clamped = Math.Clamp(value, 0, _maxHealth);
        // Solo notificamos si el cambio es significativo para ahorrar procesamiento
        if (Math.Abs(_health - clamped) > 0.001f)
        {
            _health = clamped;
            OnHealthChanged?.Invoke(GetHealthPercentage());
        }
    }

    /// <summary> Reduce la vida actual en una cantidad espec?fica. </summary>
    public void TakeDamage(float amount) => SetHealth(_health - amount);
    #endregion
}