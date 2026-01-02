using UnityEngine;

public class ChaseMovementStrategy : IMovementStrategy
{
    private float _followSpeed = 4f;
    private float _acceleration = 15f; // Aceleración alta para que reaccione rápido
    private float _stopDistance = 3f;
    private float _rotationSpeed = 7f;

    public void StartMovement(CreatureStateMachine creature)
    {
        creature.Agent.speed = _followSpeed;
        creature.Agent.acceleration = _acceleration;
        creature.Agent.stoppingDistance = _stopDistance;
        creature.Agent.updateRotation = false;
        creature.Agent.autoBraking = true; // Crucial para que no choque con el jugador
    }

    public void Move(CreatureStateMachine creature)
    {
        if (creature.FollowTarget == null) return;

        // 1. Calculamos si se está moviendo
        // Usamos sqrMagnitude por rendimiento
        bool isMoving = creature.Agent.velocity.sqrMagnitude > 0.1f;

        // 2. Actualizar destino
        creature.Agent.SetDestination(creature.FollowTarget.position);

        // 3. Rotación (Pasamos el bool para simplificar)
        ApplySmoothRotation(creature, isMoving);

        // 4. Animaciones
        // Si isMoving es falso, podemos forzar el ratio a 0
        float speedRatio = isMoving ? (creature.Agent.velocity.magnitude / creature.Agent.speed) : 0f;

        creature.Anim.SetFloat(CreatureStateMachine.VertHash, isMoving ? 1f : 0f);
        creature.Anim.SetFloat(CreatureStateMachine.StateHash, speedRatio);
    }

    private void ApplySmoothRotation(CreatureStateMachine creature, bool isMoving)
    {
        // Si el agente se está moviendo, rotamos hacia donde va la velocidad
        if (isMoving)
        {
            Quaternion targetRot = Quaternion.LookRotation(creature.Agent.velocity.normalized);
            creature.transform.rotation = Quaternion.Slerp(
                creature.transform.rotation,
                targetRot,
                Time.deltaTime * _rotationSpeed
            );
        }
        else
        {
            // Si está casi quieto, que mire directamente al jugador
            Vector3 directionToPlayer = (creature.FollowTarget.position - creature.transform.position).normalized;
            directionToPlayer.y = 0; // Evitar que el caballo se incline hacia arriba/abajo

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(directionToPlayer);
                creature.transform.rotation = Quaternion.Slerp(
                    creature.transform.rotation,
                    targetRot,
                    Time.deltaTime * (_rotationSpeed * 0.5f)
                );
            }
        }
    }
}