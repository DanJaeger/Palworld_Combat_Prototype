using UnityEngine;

/// <summary>
/// Contenedor de datos que define todas las capacidades fisicas y de recursos del jugador.
/// Permite ajustar el comportamiento del personaje desde el Inspector sin modificar el codigo.
/// </summary>
[CreateAssetMenu(fileName = "NuevosStatsPersonaje", menuName = "SO/Stats de Creature")]
public class CreatureStatsSO : ScriptableObject
{
    #region Salud y Estamina
    [Header("VIDA Y ESTAMINA")]
    [Tooltip("Los puntos de vida maximos que el jugador puede alcanzar.")]
    public float MaxHealth = 100f;

    [Tooltip("Los puntos de vida con los que el jugador comienza la partida.")]
    public float InitialHealth = 100f;
    #endregion
}