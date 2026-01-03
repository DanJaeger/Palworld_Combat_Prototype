using UnityEngine;
public abstract class IdleStrategySO : ScriptableObject
{
    public abstract void Idling(CreatureStateMachine creature);
}
