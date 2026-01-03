using UnityEngine;

[CreateAssetMenu(fileName = "NewCreatureData", menuName = "Creatures/Data")]
public class CreatureData : ScriptableObject
{
    [Header("Estrategias")]
    public IdleStrategySO idleStrategy;
    public MovementStrategySO patrolStrategy; // Aquí arrastrarás tu GallopStrategySO
    public MovementStrategySO chaseStrategy;

    [Header("Movement Settings")]
    public float idleDuration = 10f;

    [Header("Idle Animation Settings")]
    public float animationParameterTargetValue = 1f;
    public float timeToReachTargetValue = 1.25f;
    public float cooldownTime = 17.5f;
    public int timesToLoopAnimation = 2;

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float acceleration = 15f;
    public float patrolRadius = 10f;

    [Header("Distances")]
    public float stoppingDistance = 3f;
    public float interactionRadius = 3f;

    [Header("Visuals")]
    public float rotationSpeed = 7f;
    public float animDampTime = 0.1f;
}