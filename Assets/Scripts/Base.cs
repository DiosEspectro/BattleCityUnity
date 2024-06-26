using UnityEngine;

public class Base : MonoBehaviour {
    // Класс игрового объекта - базы

    private GameController game;
    private AudioController aux;

    [SerializeField]
    private GameObject baseVisual;
    [SerializeField]
    private GameObject baseExploded;

    
    private bool isIntact = true; // Признак того, что база не тронута (не взорвана)

    private bool needChangeSprite = false;
    private float timer = 0f;

    private Explode explodePrefab;

    private void Awake() {
        game = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
        aux = game.getAudioController();

        explodePrefab = Resources.Load<Explode>("Explode");
    }

    private void Update() {
        if (needChangeSprite) ChangeSprite(); // Если нужна замена спрайта, то меняем спрайт
    }

    public void DestroyBase() {
        // Если база нетронута, то взрываем
        if(isIntact) {
            needChangeSprite = true;
            timer = 0f;
            aux.PlaySound(AudioController.ClipName.ExplodePlayer);
            Instantiate(explodePrefab, this.transform.position, this.transform.rotation);
            isIntact = false;
            game.setGameOver(); // Инициируем конец игры
        }
    }

    private void ChangeSprite() {
        // Меняем спрайт с некоторой задержкой

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