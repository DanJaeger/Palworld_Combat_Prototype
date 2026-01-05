using UnityEngine;

public abstract class MovementStrategySO : ScriptableObject
{
    public abstract void StartMovement(CreatureStateMachine creature);
    public abstract void Move(CreatureStateMachine creature);
}