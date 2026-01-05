using UnityEngine;

[CreateAssetMenu(fileName = "ChaseStrategy", menuName = "Creatures/Strategies/Chase")]
public class ChaseMovementStrategySO : MovementStrategySO
{
    public override void StartMovement(CreatureStateMachine creature)
    {
        creature.Agent.speed = creature.Data.chaseSpeed;
        creature.Agent.acceleration = creature.Data.acceleration;
        creature.Agent.stoppingDistance = creature.Data.stoppingDistance;
        creature.Agent.updateRotation = false;
        creature.Agent.autoBraking = true;
    }

    public override void Move(CreatureStateMachine creature)
    {
        if (creature.FollowTarget == null) return;

        // 1. Detección de movimiento
        bool isMoving = creature.Agent.velocity.sqrMagnitude > 0.1f;

        // 2. Navegación y Rotación
        creature.Agent.SetDestination(creature.FollowTarget.position);
        ApplySmoothRotation(creature, isMoving);

        // 3. Definición de objetivos (Targets)
        float targetVert = isMoving ? 1f : 0f;
        float targetState = isMoving ? (creature.Agent.velocity.magnitude / creature.Agent.speed) : 0f;

        // 4. Aplicar suavizado (Damping) a ambos
        creature.Anim.SetFloat(CreatureStateMachine.VertHash, targetVert, creature.Data.animDampTime, Time.deltaTime);
        creature.Anim.SetFloat(CreatureStateMachine.StateHash, targetState, creature.Data.animDampTime, Time.deltaTime);

        // 5. LIMPIEZA DE DECIMALES (Snap to Zero)
        // Si no se mueve y los valores son muy pequeños, los forzamos a 0 exacto
        if (!isMoving)
        {
            // Umbral de corte (0.01 es invisible al ojo en animaciones)
            float currentVert = creature.Anim.GetFloat(CreatureStateMachine.VertHash);
            float currentState = creature.Anim.GetFloat(CreatureStateMachine.StateHash);

            if (currentVert < 0.01f)
                creature.Anim.SetFloat(CreatureStateMachine.VertHash, 0f);

            if (currentState < 0.01f)
                creature.Anim.SetFloat(CreatureStateMachine.StateHash, 0f);
        }
    }

    private void ApplySmoothRotation(CreatureStateMachine creature, bool isMoving)
    {
        // ... (tu código de rotación está perfecto) ...
        if (isMoving)
        {
            Quaternion targetRot = Quaternion.LookRotation(creature.Agent.velocity.normalized);
            creature.transform.rotation = Quaternion.Slerp(creature.transform.rotation, targetRot, Time.deltaTime * creature.Data.rotationSpeed);
        }
        else
        {
            Vector3 directionToPlayer = (ContextDirection(creature));
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(directionToPlayer);
                creature.transform.rotation = Quaternion.Slerp(creature.transform.rotation, targetRot, Time.deltaTime * (creature.Data.rotationSpeed * 0.5f));
            }
        }
    }

    private Vector3 ContextDirection(CreatureStateMachine creature)
    {
        Vector3 dir = (creature.FollowTarget.position - creature.transform.position).normalized;
        dir.y = 0;
        return dir;
    }
}
