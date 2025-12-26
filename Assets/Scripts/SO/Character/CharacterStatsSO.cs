using UnityEngine;

/// <summary>
/// Contenedor de datos que define todas las capacidades f?sicas y de recursos del jugador.
/// Permite ajustar el comportamiento del personaje desde el Inspector sin modificar el c?digo.
/// </summary>
[CreateAssetMenu(fileName = "NuevosStatsPersonaje", menuName = "SO/Stats de Personaje")]
public class CharacterStatsSO : ScriptableObject
{
    #region Salud
    [Header("VIDA Y ESTAMINA")]
    [Tooltip("Los puntos de vida m?ximos que el jugador puede alcanzar.")]
    public float MaxHealth = 100f;

    [Tooltip("Los puntos de vida con los que el jugador comienza la partida.")]
    public float InitialHealth = 100f;
    #endregion

    #region Capas de F?sica
    [Header("CAPAS (LAYERS)")]
    [Tooltip("La LayerMask usada para identificar al jugador. Se usa para excluir el propio collider del jugador en chequeos f?sicos.")]
    public LayerMask PlayerLayer;
    #endregion

    #region Configuracion de Input
    [Header("INPUT")]
    [Tooltip("Si est? activo, el input de movimiento se ajustar? a -1, 0 o 1. ?til para un comportamiento consistente entre teclado y mandos anal?gicos.")]
    public bool SnapInput = true;

    [Tooltip("Input vertical m?nimo requerido para registrar una acci?n (trepar, etc). Previene inputs accidentales por el drift de la palanca."), Range(0.01f, 0.99f)]
    public float VerticalDeadZoneThreshold = 0.3f;

    [Tooltip("Input horizontal m?nimo requerido para registrar movimiento. Previene que el personaje se deslice solo por drift del hardware."), Range(0.01f, 0.99f)]
    public float HorizontalDeadZoneThreshold = 0.1f;
    #endregion

    #region Ajustes de Movimiento
    [Header("MOVIMIENTO")]
    [Tooltip("La velocidad de caminar del personaje.")]
    public float WalkSpeed = 4f;

    [Tooltip("La velocidad de caminar del personaje.")]
    public float RunSpeed = 8f;

    [Tooltip("La velocidad de rotacion del personaje.")]
    public float RotationFactorPerFrame = 15f;
    #endregion

    #region Ajustes de Salto
    [Header("SALTO")]

    [Tooltip("Altura terminal o altura maxima a la que el jugador puede llegar.")]
    public float MaxJumpHeight = 2f;

    [Tooltip("Tiempo para llegar al punto mas alto.")]
    public float MaxJumpTime = 0.7f;
    #endregion
}