using UnityEngine;

[CreateAssetMenu(fileName = "TigerPatrolBehaviorStrategy", menuName = "Creatures/Strategies/Tiger/Patrol")]
public class TigerPatrolBehaviorStrategy : MovementStrategySO
{
    public override void StartMovement(CreatureStateMachine creature)
    {
        // 1. Configuramos el agente

        creature.Agent.speed = creature.Data.patrolSpeed;
        creature.Agent.acceleration = creature.Data.acceleration;
        creature.Agent.updateRotation = false;
        creature.Agent.isStopped = false;

        // 2. Calculamos el destino
        // Cada animal llamar? a su propia instancia de GetRandomPatrolPoint()
        Vector3 target = creature.GetRandomPatrolPoint();
        creature.Agent.SetDestination(target);
    }

    public override void Move(CreatureStateMachine creature)
    {
        // 1. Actualizar Animaci?n (Cada animal tiene su propio Animator)
        // Usamos el snap a 1f para el galope constante
        creature.Anim.SetFloat(CreatureStateMachine.VertHash, 1f, 0.1f, Time.deltaTime);

        // 2. Rotaci?n Manual Suave
        ApplySmoothRotation(creature);
    }

    private void ApplySmoothRotation(CreatureStateMachine creature)
    {
        if (creature.Agent.velocity.sqrMagnitude > 0.1f)
        {
            float rotSpeed = creature.Data.rotationSpeed;

            Vector3 direction = creature.Agent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Cada transform.rotation es ?nico de la instancia del animal
            creature.transform.rotation = Quaternion.Slerp(
                creature.transform.rotation,
                targetRotation,
                Time.deltaTime * rotSpeed
            );
        }
    }
}