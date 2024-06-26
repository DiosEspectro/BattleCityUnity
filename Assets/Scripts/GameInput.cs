using System;
using UnityEngine;

public class GameInput : MonoBehaviour {
    // Обработка управления танками игроками

    public static GameInput Instance {  get; private set; }

    private PlayerInputActions playerInputActions;

    public event EventHandler OnPlayerShoot;
    public event EventHandler OnPlayer2Shoot;

    private void Awake() {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        // Подписываемся на события управления игроков
        playerInputActions.Action.Shoot.started += PlayerShoot_started;
        playerInputActions.Action2.Shoot.started += Player2Shoot_started;
    }

    private void PlayerShoot_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPlayerShoot?.Invoke(this, EventArgs.Empty);
    }

    private void Player2Shoot_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPlayer2Shoot?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVector(int player) {
        // Установка вектора движения

        Vector2 inputVector;

        if (player == 1) inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
            else inputVector = playerInputActions.Player2.Move2.ReadValue<Vector2>();

        if (inputVector.x != 0 && inputVector.y != 0)
            inputVector = new Vector2(Mathf.Round(inputVector.x), 0);

        return inputVector;
    }
}