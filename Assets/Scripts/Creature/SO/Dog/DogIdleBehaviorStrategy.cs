using UnityEngine;
[CreateAssetMenu(fileName = "DogIdleBehaviorStrategy", menuName = "Creatures/Strategies/Dog/Idle")]
public class DogIdleBehaviorStartegy : IdleStrategySO
{
    public override void Idling(CreatureStateMachine creature)
    {
        creature.Anim.SetFloat(CreatureStateMachine.StateHash, 0f);
        creature.Anim.SetFloat(CreatureStateMachine.VertHash, 0f);
    }
}
