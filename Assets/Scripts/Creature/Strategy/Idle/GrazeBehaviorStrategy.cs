
public class GrazeBehaviorStrategy : IIdleStrategy
{
    private readonly float _parameterTargetValue = 1f;
    private readonly float _timeToReachTargetValue = 1.25f;
    private readonly float _cooldownTime = 17.5f;
    private readonly int _timesToLoopAnimation = 2;
    public void Idling(CreatureStateMachine creature)
    {
        creature.TriggerStateTransition(_parameterTargetValue, _timeToReachTargetValue, CreatureStateMachine.StateHash, _cooldownTime, _timesToLoopAnimation);
    }
}
