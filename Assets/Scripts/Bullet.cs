using UnityEngine;

public class Bullet : MonoBehaviour {
    // ����� ����������� ������� ����

    private AudioController aux;

    private float bulletSpeed; // �������� ����
    public float BulletSpeed { set { bulletSpeed = value; } }

    private string direction; // ����������� ����
    public string Direction { set { direction = value; } }

    private bool isPlayerBullet; // ������� ����, ��� ���� ������
    private int ownerNum = 0; // ����� �����, ������� ������� �������. 1 ��� 2 - ��� �������, 1-n ��� ������
    public bool IsPlayer { set { isPlayerBullet = value; } get { return isPlayerBullet; } }
    public int ownerN { set { ownerNum = value; } get { return ownerNum; } }

    private int bulletPower; // ���� ����
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
        // ��� ������� ���� ����������� ����������� ���� ��� �������������� ����������, �������������� ����������� �������
        Vector3 bulletVelocity;
        switch(direction) {
            case "UP": default:
                bulletVelocity = transform.up; // ����������� ��������
                sprite.flipY = false; // ������� �������� ������� �� Y
                this.transform.Rotate(0, 0, 0); // �������
                boxCollider.offset = new Vector2(boxCollider.offset.x, -0.03f); // �������� ����������
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

        rb.velocity = bulletVelocity * bulletSpeed; // ������� ��������
        Destroy(gameObject, 5f); // ����������� ���� ����� 5 ������ (�� ������ ������)
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // ��������� ��������� ���� �� ������ ��������

        bool hit = true;
        bool showHit = true;
        bool smalHit = false;

        // ���� ������� ���������-������ � �����, �� �� ������� �� ���������
        if (collision.name.StartsWith("TankCollisionBlocker")) {
            hit = false;
        } else
        if (collision.name.StartsWith("Player") || collision.name.StartsWith("Enemy")) { // ���� ���� ������ � ����
            Tank tank = collision.GetComponent<Tank>();

            if (tank.getPlayerNum() != ownerNum) { // ������� - �� ����� �� ��� ����
                hit = true;
                tank.ReceiveBullet(bulletPower, isPlayerBullet); // ���� ���, �� ���� �������� ����
            } else hit = false;
        } else 
        if(collision.name.StartsWith("BricksVisual_")) { // ���� ���� ������ � �������
            BricksVisual bv = collision.GetComponent<BricksVisual>();
            bv.HitRegistration(direction, bulletPower, isPlayerBullet);
        } else
        if (collision.name.StartsWith("ConcreteVisual_")) { // ���� ���� ������ � ������
            ConcreteVisual cv = collision.GetComponent<ConcreteVisual>();
            cv.HitRegistration(bulletPower, isPlayerBullet);
        } else
        if (collision.name.StartsWith("Base")) { // ���� ���� ������ � ����
            Base bs = collision.GetComponent<Base>();
            if(bs.IsIntact()) {
                showHit = false;
                bs.DestroyBase();
            }
        } else
        if (collision.name.StartsWith("Bullet")) { // ���� ���� ������ � ����
            smalHit = true;
        } else {
            if(isPlayerBullet) aux.PlaySound(AudioController.ClipName.HitConcrete);
        }

        if (hit) { // ���� ���� ����-�� ������
            if (showHit) { // ���� ����� ���������� ���������
                BulletHit bh = Instantiate(bulletHitPrefab, this.transform.position, this.transform.rotation);
                if (smalHit) bh.SetSmallHit();
            }
            Destroy(gameObject); // ���������� ����
        }
    }
}