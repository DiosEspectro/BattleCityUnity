using UnityEngine;

public class Enemy : Tank {
    // Класс для вражеских танков, родительский класс - Tank

    private float movingDuration = 0f;
    private Vector2 movingVector = Vector2.zero;
    private float movingSecs = 0f;
    private bool changeMovingNeed = false;
    private float changeMovingSecs = 0f;

    private bool shootingNeeded = false;
    private float shootingSecs = 0f;
    private float shootingAfterSecs = 0f;

    protected override void Start() {
        SetTankLevel(tankLevel); // Установка уровня танка
        SetMovement(); // Инициализация движения
        GenerateShootingAfterSecs(); // Генерируем задержку выстрела
    }

    public new void SetPlayerNum(int playerNum) {
        this.playerNum = playerNum; // Установка номера танка
    }

    public override void SetTankLevel(int level) {
        // Метод установки уровня танка

        tankLevel = level;

        switch (level) {
            case 1:
                movingSpeed = 1.8f; // Скорость танка
                maxBullets = 1; // Максимальное одновременное количество пуль на экране
                bulletPower = 1; // Сила пуль
                bulletSpeed = 8f; // Скорость пуль
                hitPoints = 1; // Количество попаданий в танк
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

        SetColorByHP(); // Установка цвета, исходя из количества оставшихся попаданий
        animator.SetInteger("Level", tankLevel); // Обновление аниматора
    }

    public override void ReceiveBullet(int bulletPower, bool isPlayerBullet) {
        // Метод, обрабатывающий попадание пуль по танку 
        if (isPlayerBullet) {
            // Если пуля от игрока
            hitPoints--; // Уменьшаем количество оставшихся попаданий
            if (isBonusTank) { game.GenerateBonus(); } // Если это бонусный танк, то генерируем бонус

            if (hitPoints == 0) {
                // Если попаданий не осталось, то взрываем танк
                aux.PlaySound(AudioController.ClipName.ExplodeEnemy);
                Instantiate(explodePrefab, this.transform.position, this.transform.rotation);
                game.EnemyExploded(); // Сообщаем GameController, что враг взорван
                Destroy(gameObject); // Уничтожаем
            } else {
                // Если ещё остались попадания, то проигрываем звук
                aux.PlaySound(AudioController.ClipName.HitArmor);
                SetColorByHP(); // Меняем цвета
                flickerColor1 = getColorByHp();
            }
        }
    }
    public void DestroyObject() {
        Destroy(gameObject);
    }

    protected override void HandleMovement() {
        // Обработка движения
        if (!dontMove && !freeze) { // Если двигаемся и не заморожены
            rb.MovePosition(rb.position + movingVector * (movingSpeed * Time.fixedDeltaTime)); // Двигаемся
            isMoving = true;

            if (changeMovingNeed) { // Если требуется изменение движения, то отсчитываем время и меняем курс
                changeMovingSecs += Time.fixedDeltaTime;
                if(changeMovingSecs >= 0.5f) SetMovement();
            } else {
                movingSecs += Time.fixedDeltaTime;
                if (movingSecs > movingDuration) changeMovingNeed = true; // SetMovement();
            }
        }

        ShootingProcess(); // Процесс стрельбы
    }

    private void ShootingProcess() {
        // Метод, отвечающий за стрельбу. Если танк не заморожен.
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
        // Начало движения
        changeMovingNeed = false;
        changeMovingSecs = 0f;
        movingSecs = 0f;
        GenerateMovingVector(); // Выбор направления
        GenerateMovingDuration(); // Выбор длительности движения
    }

    private void GenerateMovingVector() {
        // Генерация направления движения
        int direction = 3;
        int dirPerc = Random.Range(0, 101); // Берём промежуток от 0 до 100 и потом в зависимости от положения танка высчитываем куда ему дальше ехать. Сделано для того, чтобы повысить процент выпадания значений "в сторону вражеской базы"
        float enemyX = gameObject.transform.position.x;

        if (dirPerc >= 0 && dirPerc < 20) direction = 1; // В этом промежутке всегда "ВВЕРХ" (20%)
        else
        if(enemyX <= 6.5) { // Если враг левее базы
            if (dirPerc >= 20 && dirPerc < 40) direction = 4; // влево минимальный процент 20%
            else if (dirPerc >= 40 && dirPerc < 70) direction = 3; // вниз 30%
            else direction = 2; // вправо оставшиеся 30%
        } else { // Иначе, если враг правее базы
            if (dirPerc >= 20 && dirPerc < 50) direction = 4; // влево 30%
            else if (dirPerc >= 50 && dirPerc < 80) direction = 3; // вниз 30%
            else direction = 2; // вправо оставшиеся минимальные 20%
        }


        float x = 0;
        float y = 0;
        // 1 - вверх, 2 - вправо, 3 - вниз, 4 - влево
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
        // Генерация длительности движения
        this.movingDuration = Random.Range(0.5f, 10f);
    }

    private void GenerateShootingAfterSecs() {
        // Генерируем задержку выстрела в миллисекундах
        shootingNeeded = true;
        shootingSecs = 0;
        shootingAfterSecs = Random.Range(0.0f, 1.25f);
    }

    protected override Vector2 GetColliderSize(string direction) {
        // Меняем параметры коллайдера танка в зависимости от направления движения
        Vector2 ret;
        float length = 0.98f; // Длинна коллайдера танка (от носа до задней части)
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
        // Меняем позицию спрайта (центруем) в зависимости от направления движения
        Vector3 ret;

        float offset1; // Отступ 1
        float offset2; // Отступ 2

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
        // Установка цвета танка в зависимости от HP
        spriteRenderer.color = getColorByHp();
    }

    protected override void OnCollisionStay2D(Collision2D other) {
        // Обработка столкновений. Если столкнулись, то не двигаем и не тащим
        if (other.collider.tag == "Player" || other.collider.tag == "Enemy") {
            dontMove = true;
        }

        changeMovingNeed = true; // Если столкнулись с чем-то, то меняем направление движения
    }

    protected override void OnCollisionExit2D(Collision2D other) {
        // Обработка столкновений. Если столкновение прошло, то двигаемся
        if (other.collider.tag == "Player" || other.collider.tag == "Enemy") {
            dontMove = false;
        }
    }
}