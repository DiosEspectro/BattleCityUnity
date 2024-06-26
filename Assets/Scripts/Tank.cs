using System;
using UnityEngine;

public class Tank : MonoBehaviour {
    // Базовый класс для танков игроков и танков

    protected GameController game;
    protected AudioController aux;

    [SerializeField]
    protected float movingSpeed = 2f; // Скорость перемещения
    [SerializeField]
    private GameObject firePoint; // Точка выстрела
    [SerializeField]
    protected BoxCollider2D boxCollider;
    [SerializeField]
    private BoxCollider2D boxColliderBlocker;

    // Связь с составляющими PlayerVisual
    [SerializeField]
    protected PlayerVisual playerVisual;
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    private Bullet bulletPrefab;

    protected Rigidbody2D rb;
    protected float minMovingSpeed = 0.1f;
    protected bool isMoving = false;
    protected Vector2 inputVector;
    
    // Константы с направлениями
    protected const string DIRECTION_UP = "UP";
    protected const string DIRECTION_DOWN = "DOWN";
    protected const string DIRECTION_LEFT = "LEFT";
    protected const string DIRECTION_RIGHT = "RIGHT";

    protected string oldDirection = "";
    protected string movingDirection = DIRECTION_UP;
    protected string oldMovingDirection = "";

    protected bool dontMove = false;
    protected bool freeze = false;

    protected Bullet[] bullets = new Bullet[2];

    [SerializeField]
    protected int playerNum = 0;

    [SerializeField]
    protected int tankLevel = 1;
    protected int maxBullets = 1;
    protected int bulletPower = 1;
    protected float bulletSpeed = 10f;

    protected int hitPoints = 0;

    private bool isStunning = false;
    private float stunningSecs = 0f;
    private float stunningMaxSecs = 3f;
    private float stunningAlpha = 1;

    protected Collider2D ignoredCollider = null;

    protected Explode explodePrefab;

    protected bool isBonusTank = false;
    private float flickerSecs = 0f;
    private float flickerMaxSecs = 1f;
    private int flickerCurColorNum = 1;
    protected Color flickerColor1;
    private Color flickerColor2;

    protected virtual void Start() {
        
    }

    protected virtual void Awake() {
        game = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
        aux = game.getAudioController();

        rb = GetComponent<Rigidbody2D>();
        bulletPrefab = Resources.Load<Bullet>("Bullet");
        explodePrefab = Resources.Load<Explode>("Explode");

        UpdateDirectionThings(); // Обновление элементов, исходя из направления движения
        SetAnimParameters(); // Установка параметров анимации
    }

    protected virtual void Update() {
        // Управление анимацией
        if (isMoving && !freeze) animator.StopPlayback();
        else animator.StartPlayback();

        // Функции эффектов
        Stunning(); // Оглушение
    }

    protected virtual void FixedUpdate() {
        HandleMovement(); // Обработка движения

        if (isBonusTank) FlickeringProcess(); // Если это бонусный танк, то работает процесс мигания
    }

    protected void UpdateDirectionThings() {
        // Обновление элементов, исходя из направления движения

        firePoint.transform.localPosition = GetFirePointPlace(movingDirection); // Позиция точки выстрела
        boxCollider.size = GetColliderSize(movingDirection); // Размер коллайдера
        boxColliderBlocker.size = GetColliderSize(movingDirection); // Размер коллайдера-блокера
        playerVisual.transform.localPosition = GetSpritePosition(movingDirection); // Позиция спрайта
    }

    public virtual void SetPlayerNum(int playerNum) {
        this.playerNum = playerNum; // Установка номера игрока
    }

    protected virtual void HandleMovement() {
        
    }

    protected float roundBy05(float value) {
        return (float)Math.Round(value * 2, MidpointRounding.AwayFromZero) / 2; // Округление до 0.5
    }

    private Vector3 GetFirePointPlace(string direction) {
        // Возвращает позицию точки выстрела танка в зависимости от направления движения

        Vector3 ret;

        switch (direction) {
            case DIRECTION_UP: default: ret = new Vector3(0f, 0.5f, 0f); break;
            case DIRECTION_DOWN: ret = new Vector3(0f, -0.5f, 0f); break;
            case DIRECTION_LEFT: ret = new Vector3(-0.5f, 0f, 0f); break;
            case DIRECTION_RIGHT: ret = new Vector3(0.5f, 0f, 0f); break;
        }

        return ret;
    }

    protected virtual Vector2 GetColliderSize(string direction) {
        // Возвращает параметры коллайдера танка в зависимости от направления движения

        Vector2 ret;
        float length = 0.98f; // Длинна коллайдера танка (от носа до задней части)
        float width = tankLevel == 4 ? 0.87f : 0.8f; // Танк 4 уровня шире других

        switch (direction) {
            case DIRECTION_UP: default: ret = new Vector2(width, length); break;
            case DIRECTION_DOWN: ret = new Vector2(width, length); break;
            case DIRECTION_LEFT: ret = new Vector2(length, width); break;
            case DIRECTION_RIGHT: ret = new Vector2(length, width); break;
        }

        return ret;
    }

    protected virtual Vector3 GetSpritePosition(string direction) {
        // Возвращает позицию спрайта (центруем) в зависимости от направления движения

        Vector3 ret;

        float offset1; // Отступ 1
        float offset2; // Отступ 2

        switch (tankLevel) {
            default:
            case 1: offset1 = 0.03f; offset2 = 0.035f; break;
            case 2: offset1 = 0.03f; offset2 = 0f; break;
            case 3: offset1 = 0.03f; offset2 = -0.03f; break;
            case 4: offset1 = 0f; offset2 = -0.03f; break;
        }

        switch (direction) {
            case DIRECTION_UP: default: ret = new Vector3(offset1, offset2, 0f); break;
            case DIRECTION_DOWN: ret = new Vector3(-offset1, -offset2, 0f); break;
            case DIRECTION_LEFT: ret = new Vector3(-offset2, offset1, 0f); break;
            case DIRECTION_RIGHT: ret = new Vector3(offset2, -offset1, 0f); break;
        }

        return ret;
    }

    protected void Shoot() {
        // Метод стрельбы пулями

        if (bullets[0] == null) {
            bullets[0] = StartBullet();
        } else if (maxBullets > 1 && bullets[1] == null) {
            bullets[1] = StartBullet();
        }
    }

    private Bullet StartBullet() {
        // Запуск пули

        PlaySound(1);
        Bullet bullet = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
        bullet.BulletSpeed = bulletSpeed;
        bullet.Direction = movingDirection;
        bullet.BulletPower = bulletPower;
        bullet.IsPlayer = playerNum > 0 ? true : false;
        bullet.ownerN = playerNum;

        return bullet;
    }

    protected virtual void PlaySound(int sound) {
        
    }

    public virtual void SetTankLevel(int level) {
        // Установка уровня танка

        tankLevel = level;

        switch (level) {
            case 1:
                maxBullets = 1;
                bulletPower = 1;
                bulletSpeed = 8f;
                break;
            case 2:
                maxBullets = 1;
                bulletPower = 1;
                bulletSpeed = 12f;
                break;
            case 3:
                maxBullets = 2;
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

    public virtual void ReceiveBullet(int bulletPower, bool isPlayerBullet) {
        
    }

    public bool IsMoving() { return isMoving; }

    public string GetMovingDirection() { return movingDirection; }

    public int getPlayerNum() { return playerNum; }

    protected void SetAnimParameters() {
        // Установка параметров аниматора

        animator.SetBool("IsUp", movingDirection == "UP");
        animator.SetBool("IsDown", movingDirection == "DOWN");
        animator.SetBool("IsLeft", movingDirection == "LEFT");
        animator.SetBool("IsRight", movingDirection == "RIGHT");
        oldDirection = movingDirection;
    }

    public void StartStunning(float seconds) {
        // Начало процесса оглушения

        isStunning = true;
        stunningSecs = 0f;
        stunningMaxSecs = seconds;
        stunningAlpha = 0.5f;

        UpdateDirectionThings();
    }

    protected void Stunning() {
        // Процесс оглушения

        if (isStunning) {
            dontMove = true;
            freeze = true;
            stunningSecs += Time.fixedDeltaTime;

            // Если оглушён игрок, то он мигает
            if (playerNum == 1 || playerNum == 2) {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.MoveTowards(spriteRenderer.color.a, stunningAlpha, 2f * Time.deltaTime));

                if (spriteRenderer.color.a == stunningAlpha) {
                    if (stunningAlpha == 1.0f) {
                        stunningAlpha = 0.3f;
                    } else
                        stunningAlpha = 1.0f;
                }
            }

            if (stunningSecs > stunningMaxSecs) StopStunning();
        }
    }

    protected void StopStunning() {
        // Остановка оглушения

        isStunning = false;
        dontMove = false;
        freeze = false;
        stunningAlpha = 1f;

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, stunningAlpha);
    }

    public void SetTankAsBonus() {
        // Установка танка как бонусного

        isBonusTank = true;
        flickerSecs = 0f;
        flickerMaxSecs = 0.2f;
        flickerCurColorNum = 1;
        flickerColor1 = getColorByHp();
        flickerColor2 = new Color(0.9716981f, 0.2154236f, 0.4834655f, 1f);
        spriteRenderer.color = flickerColor2;
    }

    protected Color getColorByHp() {
        // Возвращает цвет, исходя из количества HP

        Color color;

        switch (hitPoints) {
            case 1: default: color = new Color(0.8584906f, 0.8584906f, 0.8584906f, 1f); break;
            case 2: color = new Color(0.7877358f, 0.9191375f, 0.8584906f, 1f); break;
            case 3: color = new Color(0.6179246f, 0.7944711f, 1f, 1f); break;
            case 4: color = new Color(0.7674662f, 0.5896226f, 1f, 1f); break;
        }

        return color;
    }

    private void FlickeringProcess() {
        // Процесс мерцания

        flickerSecs += Time.fixedDeltaTime;

        if (flickerSecs > flickerMaxSecs) {
            if (flickerCurColorNum == 1) {
                spriteRenderer.color = flickerColor2;
                flickerCurColorNum = 2;
            } else {
                spriteRenderer.color = flickerColor1;
                flickerCurColorNum = 1;
            }

            flickerSecs = 0f;
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D other) {
        // Обработка столкновений. Если столкнулись, то не двигаем и не тащим

        if (other.collider.tag.StartsWith("Player") || other.collider.tag.StartsWith("Enemy")) {
            dontMove = true;
        }

    }

    protected virtual void OnCollisionExit2D(Collision2D other) {
        // Обработка столкновений. Если столкновение прошло, то двигаемся

        if (other.collider.tag.StartsWith("Player") || other.collider.tag.StartsWith("Enemy")) {
            dontMove = false;
        }
    }
}