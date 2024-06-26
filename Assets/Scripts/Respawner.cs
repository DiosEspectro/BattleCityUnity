using UnityEngine;

public class Respawner : MonoBehaviour {
    // ����� ����������, ������� ����� �������� �������� ������ ����� (������ � ���������)

    private GameController game;

    [SerializeField]
    private Animator animator;
    private Object respawnObject;
    private GameObject respawnSpot;

    private int type; // ���
    private bool isBonusTank = false; // ������� ��������� �����

    void Start() {
        game = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
    }

    void Update() {
        // ���� �������� ��� �� ������, �� ������ ����, ������ ���������
        if (!IsAnimationPlaying()) {
            CreateObject();
            Destroy(gameObject, 0f);
        }
    }

    public void setRespawnSettings(Object obj, GameObject respSpot, int otype, bool obonus = false) {
        // ������������� ��������� ����������, � ������� ������� �� �������� ����� ���� ����� ���������
        respawnObject = obj; // ������ (Prefab)
        respawnSpot = respSpot; // ����� ���������
        type = otype; // ���
        isBonusTank = obonus; // ������� ��������� �����
    }


    private bool IsAnimationPlaying() {
        // ���������� false, ���� �������� ��� ���������
        return animator.GetCurrentAnimatorStateInfo(0).length >
           animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void CreateObject() {
        // �����, ������� ������ ����, ������ �� ������� ����������

        switch (respawnObject.name) {
            case "Player": // ���� ������
                    Player pl = Instantiate((Player) respawnObject, respawnSpot.transform.position, Quaternion.identity);
                    pl.SetTankLevel(game.GetPlayerLevel(type));
                    pl.SetPlayerNum(type);
                    game.PlayerHasBeenCreated(type, pl); // ��������� ����, ��� ������ ���� ������
                break;

            case "Enemy": // ���� �����
                    Enemy en = Instantiate((Enemy)respawnObject, respawnSpot.transform.position, Quaternion.identity);
                    en.SetTankLevel(type);
                    en.SetPlayerNum(0);
                    if (isBonusTank) en.SetTankAsBonus(); 
                    game.EnemyHasBeenCreated(en); // ��������� ����, ��� ������ ���� �����
                break;
        }
    }
}