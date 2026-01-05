using UnityEngine;
[CreateAssetMenu(fileName = "TigerIdleBehaviorStrategy", menuName = "Creatures/Strategies/Tiger/Idle")]
public class TigerIdleBehaviorStrategy : IdleStrategySO
{
    public override void Idling(CreatureStateMachine creature)
    {
        CreatureData data = creature.Data;
        creature.TriggerStateTransition(data.animationParameterTargetValue, data.timeToReachTargetValue, CreatureStateMachine.StateHash, data.cooldownTime, data.timesToLoopAnimation);
    }
}
