using UnityEngine;

public class Enemy : Tank {
    // ����� ��� ��������� ������, ������������ ����� - Tank

    private float movingDuration = 0f;
    private Vector2 movingVector = Vector2.zero;
    private float movingSecs = 0f;
    private bool changeMovingNeed = false;
    private float changeMovingSecs = 0f;

    private bool shootingNeeded = false;
    private float shootingSecs = 0f;
    private float shootingAfterSecs = 0f;

    protected override void Start() {
        SetTankLevel(tankLevel); // ��������� ������ �����
        SetMovement(); // ������������� ��������
        GenerateShootingAfterSecs(); // ���������� �������� ��������
    }

    public new void SetPlayerNum(int playerNum) {
        this.playerNum = playerNum; // ��������� ������ �����
    }

    public override void SetTankLevel(int level) {
        // ����� ��������� ������ �����

        tankLevel = level;

        switch (level) {
            case 1:
                movingSpeed = 1.8f; // �������� �����
                maxBullets = 1; // ������������ ������������� ���������� ���� �� ������
                bulletPower = 1; // ���� ����
                bulletSpeed = 8f; // �������� ����
                hitPoints = 1; // ���������� ��������� � ����
                break;
            case 2:
                movingSpeed = 4f;
                maxBullets = 1;
                bulletPower = 1;
                bulletSpeed = 12f;
                hitPoints = 1;
                break;
            case 3:
                movingSpeed = 2f;
                maxBullets = 1;
                bulletPower = 1;
                bulletSpeed = 10f;
                hitPoints = 1;
                break;
            case 4:
                movingSpeed = 2f;
                maxBullets = 1;
                bulletPower = 1;
                bulletSpeed = 10f;
                hitPoints = 4;
                break;
        }

        SetColorByHP(); // ��������� �����, ������ �� ���������� ���������� ���������
        animator.SetInteger("Level", tankLevel); // ���������� ���������
    }

    public override void ReceiveBullet(int bulletPower, bool isPlayerBullet) {
        // �����, �������������� ��������� ���� �� ����� 
        if (isPlayerBullet) {
            // ���� ���� �� ������
            hitPoints--; // ��������� ���������� ���������� ���������
            if (isBonusTank) { game.GenerateBonus(); } // ���� ��� �������� ����, �� ���������� �����

            if (hitPoints == 0) {
                // ���� ��������� �� ��������, �� �������� ����
                aux.PlaySound(AudioController.ClipName.ExplodeEnemy);
                Instantiate(explodePrefab, this.transform.position, this.transform.rotation);
                game.EnemyExploded(); // �������� GameController, ��� ���� �������
                Destroy(gameObject); // ����������
            } else {
                // ���� ��� �������� ���������, �� ����������� ����
                aux.PlaySound(AudioController.ClipName.HitArmor);
                SetColorByHP(); // ������ �����
                flickerColor1 = getColorByHp();
            }
        }
    }
    public void DestroyObject() {
        Destroy(gameObject);
    }

    protected override void HandleMovement() {
        // ��������� ��������
        if (!dontMove && !freeze) { // ���� ��������� � �� ����������
            rb.MovePosition(rb.position + movingVector * (movingSpeed * Time.fixedDeltaTime)); // ���������
            isMoving = true;

            if (changeMovingNeed) { // ���� ��������� ��������� ��������, �� ����������� ����� � ������ ����
                changeMovingSecs += Time.fixedDeltaTime;
                if(changeMovingSecs >= 0.5f) SetMovement();
            } else {
                movingSecs += Time.fixedDeltaTime;
                if (movingSecs > movingDuration) changeMovingNeed = true; // SetMovement();
            }
        }

        ShootingProcess(); // ������� ��������
    }

    private void ShootingProcess() {
        // �����, ���������� �� ��������. ���� ���� �� ���������.
        if (!freeze) {
            if (bullets[0] == null) {
                if (shootingNeeded) {
                    shootingSecs += Time.fixedDeltaTime;
                    if (shootingSecs >= shootingAfterSecs) {
                        Shoot();
                        shootingNeeded = false;
                    }
                } else {
                    GenerateShootingAfterSecs();
                }
            }
        }
    }

    private void SetMovement() {
        // ������ ��������
        changeMovingNeed = false;
        changeMovingSecs = 0f;
        movingSecs = 0f;
        GenerateMovingVector(); // ����� �����������
        GenerateMovingDuration(); // ����� ������������ ��������
    }

    private void GenerateMovingVector() {
        // ��������� ����������� ��������
        int direction = 3;
        int dirPerc = Random.Range(0, 101); // ���� ���������� �� 0 �� 100 � ����� � ����������� �� ��������� ����� ����������� ���� ��� ������ �����. ������� ��� ����, ����� �������� ������� ��������� �������� "� ������� ��������� ����"
        float enemyX = gameObject.transform.position.x;

        if (dirPerc >= 0 && dirPerc < 20) direction = 1; // � ���� ���������� ������ "�����" (20%)
        else
        if(enemyX <= 6.5) { // ���� ���� ����� ����
            if (dirPerc >= 20 && dirPerc < 40) direction = 4; // ����� ����������� ������� 20%
            else if (dirPerc >= 40 && dirPerc < 70) direction = 3; // ���� 30%
            else direction = 2; // ������ ���������� 30%
        } else { // �����, ���� ���� ������ ����
            if (dirPerc >= 20 && dirPerc < 50) direction = 4; // ����� 30%
            else if (dirPerc >= 50 && dirPerc < 80) direction = 3; // ���� 30%
            else direction = 2; // ������ ���������� ����������� 20%
        }


        float x = 0;
        float y = 0;
        // 1 - �����, 2 - ������, 3 - ����, 4 - �����
        switch (direction) {
            case 1: x = 0; y = 1; break;
            case 2: x = 1; y = 0;  break;
            case 3: x = 0; y = -1; break;
            case 4: x = -1; y = 0; break;
        }

        movingVector = new Vector2(x, y);

        if (movingVector.x < 0) movingDirection = DIRECTION_LEFT;
        else if (movingVector.x > 0) movingDirection = DIRECTION_RIGHT;
        else if (movingVector.y < 0) movingDirection = DIRECTION_DOWN;
        else if (movingVector.y > 0) movingDirection = DIRECTION_UP;


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

    private void GenerateMovingDuration() {
        // ��������� ������������ ��������
        this.movingDuration = Random.Range(0.5f, 10f);
    }

    private void GenerateShootingAfterSecs() {
        // ���������� �������� �������� � �������������
        shootingNeeded = true;
        shootingSecs = 0;
        shootingAfterSecs = Random.Range(0.0f, 1.25f);
    }

    protected override Vector2 GetColliderSize(string direction) {
        // ������ ��������� ���������� ����� � ����������� �� ����������� ��������
        Vector2 ret;
        float length = 0.98f; // ������ ���������� ����� (�� ���� �� ������ �����)
        float width = 0.8f;

        switch (direction) {
            case DIRECTION_UP: default: ret = new Vector2(width, length); break;
            case DIRECTION_DOWN: ret = new Vector2(width, length); break;
            case DIRECTION_LEFT: ret = new Vector2(length, width); break;
            case DIRECTION_RIGHT: ret = new Vector2(length, width); break;
        }

        return ret;
    }

    protected override Vector3 GetSpritePosition(string direction) {
        // ������ ������� ������� (��������) � ����������� �� ����������� ��������
        Vector3 ret;

        float offset1; // ������ 1
        float offset2; // ������ 2

        switch (tankLevel) {
            default:
            case 1: offset1 = 0.03f; offset2 = 0.035f; break;
            case 2: offset1 = 0.03f; offset2 = 0f; break;
            case 3: offset1 = 0.03f; offset2 = -0.03f; break;
            case 4: offset1 = 0.03f; offset2 = 0.03f; break;
            //case 4: offset1 = 0f; offset2 = -0.03f; break;
        }

        switch (direction) {
            case DIRECTION_UP: default: ret = new Vector3(offset1, offset2, 0f); break;
            case DIRECTION_DOWN: ret = new Vector3(-offset1, -offset2, 0f); break;
            case DIRECTION_LEFT: ret = new Vector3(-offset2, offset1, 0f); break;
            case DIRECTION_RIGHT: ret = new Vector3(offset2, -offset1, 0f); break;
        }

        return ret;
    }

    private void SetColorByHP() {
        // ��������� ����� ����� � ����������� �� HP
        spriteRenderer.color = getColorByHp();
    }

    protected override void OnCollisionStay2D(Collision2D other) {
        // ��������� ������������. ���� �����������, �� �� ������� � �� �����
        if (other.collider.tag == "Player" || other.collider.tag == "Enemy") {
            dontMove = true;
        }

        changeMovingNeed = true; // ���� ����������� � ���-��, �� ������ ����������� ��������
    }

    protected override void OnCollisionExit2D(Collision2D other) {
        // ��������� ������������. ���� ������������ ������, �� ���������
        if (other.collider.tag == "Player" || other.collider.tag == "Enemy") {
            dontMove = false;
        }
    }
}