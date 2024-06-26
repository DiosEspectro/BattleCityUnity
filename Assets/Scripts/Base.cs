using UnityEngine;

public class Base : MonoBehaviour {
    // ����� �������� ������� - ����

    private GameController game;
    private AudioController aux;

    [SerializeField]
    private GameObject baseVisual;
    [SerializeField]
    private GameObject baseExploded;

    
    private bool isIntact = true; // ������� ����, ��� ���� �� ������� (�� ��������)

    private bool needChangeSprite = false;
    private float timer = 0f;

    private Explode explodePrefab;

    private void Awake() {
        game = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
        aux = game.getAudioController();

        explodePrefab = Resources.Load<Explode>("Explode");
    }

    private void Update() {
        if (needChangeSprite) ChangeSprite(); // ���� ����� ������ �������, �� ������ ������
    }

    public void DestroyBase() {
        // ���� ���� ���������, �� ��������
        if(isIntact) {
            needChangeSprite = true;
            timer = 0f;
            aux.PlaySound(AudioController.ClipName.ExplodePlayer);
            Instantiate(explodePrefab, this.transform.position, this.transform.rotation);
            isIntact = false;
            game.setGameOver(); // ���������� ����� ����
        }
    }

    private void ChangeSprite() {
        // ������ ������ � ��������� ���������

        timer += Time.deltaTime;

        if(timer > 0.5f) {
            needChangeSprite = false;
            baseVisual.SetActive(false);
            baseExploded.SetActive(true);
        }
    }

    public bool IsIntact() { return isIntact; }

    public void DestroyObject(float sec) {
        Destroy(gameObject, sec);
    }
}