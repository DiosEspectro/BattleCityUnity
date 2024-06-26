using System;
using UnityEngine;

public class Player : Tank {
    // ����� ��� ������ �������, ������������ ����� - Tank
    public static Player Instance { get; private set; }

    [SerializeField]
    private GameObject immunityObject;
    private bool isImmunity = false;
    private float immunitySecs = 0f;
    private float immunityMaxSecs = 3f;

    protected override void Update() {
        if(game.CanControl()) inputVector = GameInput.Instance.GetMovementVector(playerNum); // ��������� ����������� ��������

        // ��������� �������� � ����� ��������
        if (isMoving && !freeze) {
            animator.StopPlayback();
            aux.PlayerMovePlaySound(playerNum);
        } else {
            animator.StartPlayback();
            aux.PlayerMoveStopSound(playerNum);
        }

        // ������� ��������
        Immunity(); // ������������
        Stunning(); // ���������
    }


    public override void SetPlayerNum(int playerNum) {
        // ��������� ������ ������

        this.playerNum = playerNum;

        if (playerNum == 1 || playerNum == 2) StartImmunity(3f);

        // �������� �� ������� ���������� � ��������� �����, ������ �� ������ ������
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
        // ��������� ������ ����� ������

        tankLevel = level;

        switch (level) {
            case 1:
                maxBullets = 1; // ������������ ������������� ���������� ���� �� ������
                bulletPower = 1; // ���� ����
                bulletSpeed = 6f; // �������� ����
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
        // ������ ������������

        isImmunity = true;
        immunitySecs = 0f;
        immunityMaxSecs = seconds;

        immunityObject.SetActive(true);
    }

    private void Immunity() {
        // �����, ������� ������������� ����� ������������

        if (isImmunity) {
            immunitySecs += Time.fixedDeltaTime;

            if (immunitySecs > immunityMaxSecs) StopImmunity(); // ���� ����� �������, ���������
        }
    }

    private void StopImmunity() {
        // �����, ����������� ������������

        isImmunity = false;
        immunityObject.SetActive(false);
    }

    public override void ReceiveBullet(int bulletPower, bool isPlayerBullet) {
        // �����, �������������� ��������� ����

        // ���� �� ��������� ������������
        if (!isImmunity) {
            // ���� ���� �������� �� ������
            if (isPlayerBullet) {
                if (playerNum == 1 || playerNum == 2) {
                    // ���� ����� �������� ���� �� ������� ������, �� ��� �������� ��� �� �����
                    StartStunning(5f);
                }
            } else {
                // ����� �� ����������

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
        // ����������� �������

        if (playerNum == 1) GameInput.Instance.OnPlayerShoot -= Player_OnPlayerShoot;
            else if (playerNum == 2) GameInput.Instance.OnPlayer2Shoot -= Player_OnPlayerShoot;
        Destroy(gameObject);
    }

    protected override void PlaySound(int sound) {
        // ��������������� ����� ��������
        aux.PlaySound(AudioController.ClipName.Shoot);
    }

    protected override void HandleMovement() {
        // ��������� �������� �����

        if (game.CanControl() && !dontMove && !freeze) rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));

        // ������� ����������� ��� ������������� ��������
        if (game.CanControl() && (Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed)) {
            isMoving = true;
        } else {
            isMoving = false;
        }

        if (isMoving && !freeze) {
            // ���� ����� ���������, �� ���������� ��������

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
        // ��������� ������ - ��������� ������ �����

        if (tankLevel < 4) {
            tankLevel++;
            game.SetPlayerLevel(playerNum, tankLevel);
            SetTankLevel(tankLevel);
        }
    }
}