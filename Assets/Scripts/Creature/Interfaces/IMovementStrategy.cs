using UnityEngine;

public interface IMovementStrategy
{
    // Se llama una vez al entrar al estado de movimiento
    void StartMovement(CreatureStateMachine creature);

    // Se llama cada frame en el Update
    void Move(CreatureStateMachine creature);
}
