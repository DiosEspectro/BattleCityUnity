using UnityEngine;

public class BonusItem : Environment {
    // ����� �������� ���������

    private GameController game;
    private AudioController aux;

    private int bonusType; // ��� ������

    [SerializeField]
    private GameObject BonusImage;
    [SerializeField]
    private GameObject BonusHelmet;
    [SerializeField]
    private GameObject BonusShovel;
    [SerializeField]
    private GameObject BonusTimer;
    [SerializeField]
    private GameObject BonusStar;
    [SerializeField]
    private GameObject BonusBomb;
    [SerializeField]
    private GameObject BonusLife;

    private float bonusFlickerSecs = 0;
    private float bonusFlickerMaxSecs = 0.15f;
    private bool isVisible = true;

    private void Awake() {
        game = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
        aux = game.getAudioController();

        aux.PlaySound(AudioController.ClipName.Bonus); // ��� ��������� ������� ����� ������������� ����

        bonusType = Random.Range(1, 7); // �������� �������� ��� ������

        // � ����������� �� ���� ������ ���������� ������ ������
        BonusHelmet.SetActive(bonusType == 1);
        BonusShovel.SetActive(bonusType == 2);
        BonusTimer.SetActive(bonusType == 3);
        BonusStar.SetActive(bonusType == 4);
        BonusBomb.SetActive(bonusType == 5);
        BonusLife.SetActive(bonusType == 6);

        Destroy(gameObject, 20f); // ���������� ������ ����� ����������� ���������� �������
    }

    protected virtual void FixedUpdate() {
        bonusFlickerSecs += Time.fixedDeltaTime;

        if (bonusFlickerSecs >= bonusFlickerMaxSecs) {
            isVisible = !isVisible;
            BonusImage.SetActive(isVisible);
            bonusFlickerSecs = 0f;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // ��������� �������, ����� ����� ���� �����
        if (collision.name.StartsWith("Player")) {
            Player player = collision.GetComponent<Player>();

            bool standardSound = true;
            switch (bonusType) {
                case 1: player.StartImmunity(10f); break; // ����� ����
                case 2: game.BonusActivate_BaseWals(); break; // ����� ������
                case 3: game.BonusActivate_Timer(); break; // ����� ������
                case 4: player.BonusActivate_IncreaseLevel(); break; // ����� ������
                case 5: game.BonusActivate_ExplodeEnemies(); break; // ����� �����
                case 6: // ����� �����
                        standardSound = false;
                        game.BonusActivate_IncreaseTankLife(player.getPlayerNum());
                        aux.PlaySound(AudioController.ClipName.OneUp);
                    break; 
            }

            if(standardSound) aux.PlaySound(AudioController.ClipName.BonusTake);

            Destroy(gameObject);
        }
    }
}