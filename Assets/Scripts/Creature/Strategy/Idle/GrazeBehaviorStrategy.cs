
public class GrazeBehaviorStrategy : IIdleStrategy
{
    public void Idling(CreatureStateMachine creature)
    {
        CreatureData data = creature.Data;
        creature.TriggerStateTransition(data.animationParameterTargetValue, data.timeToReachTargetValue, CreatureStateMachine.StateHash, data.cooldownTime, data.timesToLoopAnimation);
    }
}
