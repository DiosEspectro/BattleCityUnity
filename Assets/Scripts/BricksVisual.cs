using UnityEngine;

public class BricksVisual : MonoBehaviour {
    // Класс каждого из кирпичиков в препатствии с кирпичами

    [SerializeField]
    private int brickNum; // Номер кирпича
    private Bricks bricksObject; // Ссылка на основной объект препятствия с кирпичами

    void Start() {
        bricksObject = gameObject.transform.parent.GetComponent<Bricks>(); // Инициализация основного объекта
    }

    public void HitRegistration(string direction, int hitPower, bool isPlayerBullet = false) {
        // Обработка попадания по кирпичу
        bricksObject.HitHandler(brickNum, direction, hitPower, isPlayerBullet);
    }
}