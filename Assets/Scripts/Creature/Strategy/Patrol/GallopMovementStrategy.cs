using UnityEngine;

public class GallopMovementStrategy : IMovementStrategy
{
    private float _gallopSpeed = 2f;
    private float _acceleration = 4f;
    private float _rotationSpeed = 5f; // Controla qué tan "pesado" se siente el giro

    public void StartMovement(CreatureStateMachine creature)
    {
        creature.Agent.speed = _gallopSpeed;
        creature.Agent.acceleration = _acceleration;

        // --- LA CLAVE ---
        // Desactivamos que el NavMeshAgent rote el objeto automáticamente
        creature.Agent.updateRotation = false;

        creature.Agent.isStopped = false;

        Vector3 target = creature.GetRandomPatrolPoint();
        creature.Agent.SetDestination(target);
    }

    public void Move(CreatureStateMachine creature)
    {
        // 1. Actualizar Animación
        creature.Anim.SetFloat(CreatureStateMachine.VertHash, 1f);

        // 2. Rotación Manual Suave
        ApplySmoothRotation(creature);
    }

    private void ApplySmoothRotation(CreatureStateMachine creature)
    {
        // Solo rotamos si el caballo se está moviendo significativamente
        if (creature.Agent.velocity.sqrMagnitude > 0.1f)
        {
            // Calculamos la dirección hacia donde se está moviendo el agente
            Vector3 direction = creature.Agent.velocity.normalized;

            // Creamos la rotación deseada (mirando hacia la dirección del movimiento)
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Aplicamos una interpolación suave (Slerp)
            creature.transform.rotation = Quaternion.Slerp(
                creature.transform.rotation,
                targetRotation,
                Time.deltaTime * _rotationSpeed
            );
        }
    }
}