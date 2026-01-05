using UnityEngine;
[CreateAssetMenu(fileName = "GrazeBehaviorStrategy", menuName = "Creatures/Strategies/Horse/Idle")]
public class GrazeBehaviorStrategySO : IdleStrategySO
{
    public override void Idling(CreatureStateMachine creature)
    {
        CreatureData data = creature.Data;
        creature.TriggerStateTransition(data.animationParameterTargetValue, data.timeToReachTargetValue, CreatureStateMachine.StateHash, data.cooldownTime, data.timesToLoopAnimation);
    }
}
