using System;
using UnityEngine;

public class Player : Tank {
    // Класс для танков игроков, родительский класс - Tank
    public static Player Instance { get; private set; }

    [SerializeField]
    private GameObject immunityObject;
    private bool isImmunity = false;
    private float immunitySecs = 0f;
    private float immunityMaxSecs = 3f;

    protected override void Update() {
        if(game.CanControl()) inputVector = GameInput.Instance.GetMovementVector(playerNum); // Установка направления движения

        // Обработка анимации и звука движения
        if (isMoving && !freeze) {
            animator.StopPlayback();
            aux.PlayerMovePlaySound(playerNum);
        } else {
            animator.StartPlayback();
            aux.PlayerMoveStopSound(playerNum);
        }

        // Функции эффектов
        Immunity(); // Неуязвимость
        Stunning(); // Оглушение
    }


    public override void SetPlayerNum(int playerNum) {
        // Установка номера игрока

        this.playerNum = playerNum;

        if (playerNum == 1 || playerNum == 2) StartImmunity(3f);

        // Подписка на события управления и изменение цвета, исходя из номера игрока
        if (playerNum == 1) {
            GameInput.Instance.OnPlayerShoot += Player_OnPlayerShoot;
            spriteRenderer.color = new Color(0.8962264f, 0.7097941f, 0.139507f, 1f);
        } else if (playerNum == 2) {
            GameInput.Instance.OnPlayer2Shoot += Player_OnPlayerShoot;
            spriteRenderer.color = new Color(0.335352f, 0.764151f, 0.09011213f, 1f);
        }
    }

    private void Player_OnPlayerShoot(object sender, EventArgs e) {
        if (game.CanShoot()) Shoot();
    }

    public override void SetTankLevel(int level) {
        // Установка уровня танка игрока

        tankLevel = level;

        switch (level) {
            case 1:
                maxBullets = 1; // Максимальное одновременное количество пуль на экране
                bulletPower = 1; // Сила пуль
                bulletSpeed = 6f; // Скорость пуль
                break;
            case 2:
                maxBullets = 1;
                bulletPower = 1;
                bulletSpeed = 12f;
                break;
            case 3:
                maxBullets = 1;
                bulletPower = 1;
                bulletSpeed = 12f;
                break;
            case 4:
                maxBullets = 2;
                bulletPower = 2;
                bulletSpeed = 12f;
                break;
        }

        animator.SetInteger("Level", tankLevel);
    }

    public void StartImmunity(float seconds) {
        // Запуск неуязвимости

        isImmunity = true;
        immunitySecs = 0f;
        immunityMaxSecs = seconds;

        immunityObject.SetActive(true);
    }

    private void Immunity() {
        // Метод, который контроллирует время неуязвимости

        if (isImmunity) {
            immunitySecs += Time.fixedDeltaTime;

            if (immunitySecs > immunityMaxSecs) StopImmunity(); // Если время истекло, завершаем
        }
    }

    private void StopImmunity() {
        // Метод, завершающий неуязвимость

        isImmunity = false;
        immunityObject.SetActive(false);
    }

    public override void ReceiveBullet(int bulletPower, bool isPlayerBullet) {
        // Метод, обрабатывающий получение пули

        // Если не действует неуязвимость
        if (!isImmunity) {
            // Если пуля получена от игрока
            if (isPlayerBullet) {
                if (playerNum == 1 || playerNum == 2) {
                    // Если игрок получает пулю от другого игрока, то это оглушает его на время
                    StartStunning(5f);
                }
            } else {
                // Иначе мы уничтожены

                aux.PlaySound(AudioController.ClipName.ExplodePlayer);
                Instantiate(explodePrefab, this.transform.position, this.transform.rotation);
                if(playerNum == 1) GameInput.Instance.OnPlayerShoot -= Player_OnPlayerShoot;
                    else if (playerNum == 2) GameInput.Instance.OnPlayer2Shoot -= Player_OnPlayerShoot;
                game.PlayerExploded(playerNum);
                DestroyObject();
            }
        }
    }

    public void DestroyObject() {
        // Уничтожение объекта

        if (playerNum == 1) GameInput.Instance.OnPlayerShoot -= Player_OnPlayerShoot;
            else if (playerNum == 2) GameInput.Instance.OnPlayer2Shoot -= Player_OnPlayerShoot;
        Destroy(gameObject);
    }

    protected override void PlaySound(int sound) {
        // Воспроизведение звука выстрела
        aux.PlaySound(AudioController.ClipName.Shoot);
    }

    protected override void HandleMovement() {
        // Обработка движения танка

        if (game.CanControl() && !dontMove && !freeze) rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));

        // Условия возможности или невозможности движения
        if (game.CanControl() && (Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed)) {
            isMoving = true;
        } else {
            isMoving = false;
        }

        if (isMoving && !freeze) {
            // Если можем двигаться, то производим действия

            if (inputVector.x < 0) movingDirection = DIRECTION_LEFT;
            else if (inputVector.x > 0) movingDirection = DIRECTION_RIGHT;
            else if (inputVector.y < 0) movingDirection = DIRECTION_DOWN;
            else if (inputVector.y > 0) movingDirection = DIRECTION_UP;

            if ((movingDirection == DIRECTION_LEFT || movingDirection == DIRECTION_RIGHT) && (oldMovingDirection == DIRECTION_DOWN || oldMovingDirection == DIRECTION_UP || oldMovingDirection == "")) {
                this.transform.position = new Vector2(this.transform.position.x, roundBy05(this.transform.position.y));
                oldMovingDirection = movingDirection;
            } else
            if ((movingDirection == DIRECTION_DOWN || movingDirection == DIRECTION_UP) && (oldMovingDirection == DIRECTION_LEFT || oldMovingDirection == DIRECTION_RIGHT || oldMovingDirection == "")) {
                this.transform.position = new Vector2(roundBy05(this.transform.position.x), this.transform.position.y);
                oldMovingDirection = movingDirection;
            }

            if (!oldDirection.Equals(movingDirection)) {
                UpdateDirectionThings();
                SetAnimParameters();
            }
        }
    }

    public void BonusActivate_IncreaseLevel() {
        // Активация бонуса - повышение уровня танка

        if (tankLevel < 4) {
            tankLevel++;
            game.SetPlayerLevel(playerNum, tankLevel);
            SetTankLevel(tankLevel);
        }
    }
}