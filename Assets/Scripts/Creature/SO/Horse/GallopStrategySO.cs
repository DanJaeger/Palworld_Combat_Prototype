using UnityEngine;

[CreateAssetMenu(fileName = "GallopStrategy", menuName = "Creatures/Strategies/Horse/Gallop")]
public class GallopStrategySO : MovementStrategySO
{
    [Header("Configuración Local (Opcional)")]
    [Tooltip("Si activas esto, usará estos valores en lugar de los del CreatureData")]
    [SerializeField] private bool _overrideDataSettings = false;
    [SerializeField] private float _customSpeed = 4f;
    [SerializeField] private float _customRotationSpeed = 7f;

    public override void StartMovement(CreatureStateMachine creature)
    {
        // 1. Configuramos el agente
        // Accedemos a los datos del animal o a los locales del SO
        float speed = _overrideDataSettings ? _customSpeed : creature.Data.patrolSpeed;

        creature.Agent.speed = speed;
        creature.Agent.acceleration = creature.Data.acceleration;
        creature.Agent.updateRotation = false;
        creature.Agent.isStopped = false;

        // 2. Calculamos el destino
        // Cada animal llamará a su propia instancia de GetRandomPatrolPoint()
        Vector3 target = creature.GetRandomPatrolPoint();
        creature.Agent.SetDestination(target);
    }

    public override void Move(CreatureStateMachine creature)
    {
        // 1. Actualizar Animación (Cada animal tiene su propio Animator)
        // Usamos el snap a 1f para el galope constante
        creature.Anim.SetFloat(CreatureStateMachine.VertHash, 1f, 0.1f, Time.deltaTime);

        // 2. Rotación Manual Suave
        ApplySmoothRotation(creature);
    }

    private void ApplySmoothRotation(CreatureStateMachine creature)
    {
        if (creature.Agent.velocity.sqrMagnitude > 0.1f)
        {
            float rotSpeed = _overrideDataSettings ? _customRotationSpeed : creature.Data.rotationSpeed;

            Vector3 direction = creature.Agent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Cada transform.rotation es único de la instancia del animal
            creature.transform.rotation = Quaternion.Slerp(
                creature.transform.rotation,
                targetRotation,
                Time.deltaTime * rotSpeed
            );
        }
    }
}