using UnityEngine;

public class Respawner : MonoBehaviour {
    // Класс Респаунера, который после анимации респауна создаёт танки (игрока и вражеские)

    private GameController game;

    [SerializeField]
    private Animator animator;
    private Object respawnObject;
    private GameObject respawnSpot;

    private int type; // Тип
    private bool isBonusTank = false; // Признак бонусного танка

    void Start() {
        game = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
    }

    void Update() {
        // Если анимация уже не играет, то создаём танк, удаляя респаунер
        if (!IsAnimationPlaying()) {
            CreateObject();
            Destroy(gameObject, 0f);
        }
    }

    public void setRespawnSettings(Object obj, GameObject respSpot, int otype, bool obonus = false) {
        // Устанавливаем настройки респаунера, с помощью которых мы понимаем какой танк нужно создавать
        respawnObject = obj; // Объект (Prefab)
        respawnSpot = respSpot; // Точка появления
        type = otype; // Тип
        isBonusTank = obonus; // Признак бонусного танка
    }


    private bool IsAnimationPlaying() {
        // Возвращает false, если анимация уже завершена
        return animator.GetCurrentAnimatorStateInfo(0).length >
           animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void CreateObject() {
        // Метод, который создаёт танк, исходя из входных параметров

        switch (respawnObject.name) {
            case "Player": // Танк игрока
                    Player pl = Instantiate((Player) respawnObject, respawnSpot.transform.position, Quaternion.identity);
                    pl.SetTankLevel(game.GetPlayerLevel(type));
                    pl.SetPlayerNum(type);
                    game.PlayerHasBeenCreated(type, pl); // Оповещаем игру, что создан танк игрока
                break;

            case "Enemy": // Танк врага
                    Enemy en = Instantiate((Enemy)respawnObject, respawnSpot.transform.position, Quaternion.identity);
                    en.SetTankLevel(type);
                    en.SetPlayerNum(0);
                    if (isBonusTank) en.SetTankAsBonus(); 
                    game.EnemyHasBeenCreated(en); // Оповещаем игру, что создан танк врага
                break;
        }
    }
}