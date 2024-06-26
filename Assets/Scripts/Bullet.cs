using UnityEngine;

public class Bullet : MonoBehaviour {
    // Класс выпускаемой танками пули

    private AudioController aux;

    private float bulletSpeed; // Скорость пули
    public float BulletSpeed { set { bulletSpeed = value; } }

    private string direction; // Направление пули
    public string Direction { set { direction = value; } }

    private bool isPlayerBullet; // Признак того, что пуля игрока
    private int ownerNum = 0; // Номер юнита, который произвёл выстрел. 1 или 2 - для игроков, 1-n для врагов
    public bool IsPlayer { set { isPlayerBullet = value; } get { return isPlayerBullet; } }
    public int ownerN { set { ownerNum = value; } get { return ownerNum; } }

    private int bulletPower; // Сила пули
    public int BulletPower { set { bulletPower = value; } }

    public Rigidbody2D rb;
    private SpriteRenderer sprite;

    private BulletHit bulletHitPrefab;
    private BoxCollider2D boxCollider;

    private void Awake() {
        aux = GameObject.FindObjectOfType(typeof(AudioController)) as AudioController;

        sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
        bulletHitPrefab = Resources.Load<BulletHit>("BulletHit");
        boxCollider = gameObject.GetComponentInChildren<BoxCollider2D>();
    }

    void Start() {
        // При запуске пули учитывается направление пули для редактирования коллайдера, редактирования отображения спрайта
        Vector3 bulletVelocity;
        switch(direction) {
            case "UP": default:
                bulletVelocity = transform.up; // Направление движения
                sprite.flipY = false; // Признак поворота спрайта по Y
                this.transform.Rotate(0, 0, 0); // Поворот
                boxCollider.offset = new Vector2(boxCollider.offset.x, -0.03f); // Смещение коллайдера
                break;
            case "DOWN": 
                bulletVelocity = -transform.up;
                sprite.flipY = true;
                this.transform.Rotate(0, 0, 0);
                boxCollider.offset = new Vector2(boxCollider.offset.x, 0.03f);
                break;
            case "LEFT": 
                bulletVelocity = -transform.right;
                sprite.flipY = false;
                this.transform.Rotate(0, 0, 90);
                boxCollider.offset = new Vector2(boxCollider.offset.x, -0.03f);
                break;
            case "RIGHT": 
                bulletVelocity = transform.right;
                sprite.flipY = true;
                this.transform.Rotate(0, 0, 90);
                boxCollider.offset = new Vector2(boxCollider.offset.x, 0.03f);
                break;
        }

        rb.velocity = bulletVelocity * bulletSpeed; // Задание движения
        Destroy(gameObject, 5f); // Уничтожение пули через 5 секунд (на всякий случай)
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // Обработка попадания пули по разным объектам

        bool hit = true;
        bool showHit = true;
        bool smalHit = false;

        // Если попался коллайдер-стопер у танка, то не считаем за попадание
        if (collision.name.StartsWith("TankCollisionBlocker")) {
            hit = false;
        } else
        if (collision.name.StartsWith("Player") || collision.name.StartsWith("Enemy")) { // Если пуля попала в танк
            Tank tank = collision.GetComponent<Tank>();

            if (tank.getPlayerNum() != ownerNum) { // Смотрим - не автор ли это пули
                hit = true;
                tank.ReceiveBullet(bulletPower, isPlayerBullet); // Если нет, то танк получает пулю
            } else hit = false;
        } else 
        if(collision.name.StartsWith("BricksVisual_")) { // Если пуля попала в кирпичи
            BricksVisual bv = collision.GetComponent<BricksVisual>();
            bv.HitRegistration(direction, bulletPower, isPlayerBullet);
        } else
        if (collision.name.StartsWith("ConcreteVisual_")) { // Если пуля попала в цемент
            ConcreteVisual cv = collision.GetComponent<ConcreteVisual>();
            cv.HitRegistration(bulletPower, isPlayerBullet);
        } else
        if (collision.name.StartsWith("Base")) { // Если пуля попала в базу
            Base bs = collision.GetComponent<Base>();
            if(bs.IsIntact()) {
                showHit = false;
                bs.DestroyBase();
            }
        } else
        if (collision.name.StartsWith("Bullet")) { // Если пуля попала в пулю
            smalHit = true;
        } else {
            if(isPlayerBullet) aux.PlaySound(AudioController.ClipName.HitConcrete);
        }

        if (hit) { // Если пуля куда-то попала
            if (showHit) { // Если нужно отобразить попадание
                BulletHit bh = Instantiate(bulletHitPrefab, this.transform.position, this.transform.rotation);
                if (smalHit) bh.SetSmallHit();
            }
            Destroy(gameObject); // Уничтожаем пулю
        }
    }
}