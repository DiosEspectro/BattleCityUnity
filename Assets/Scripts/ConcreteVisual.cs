using UnityEngine;

public class ConcreteVisual : MonoBehaviour {
    // Класс каждого из блоков в препатствии с бетоном

    [SerializeField]
    private int blockNum; // Номер блока
    private Concrete concreteObject; // Ссылка на основной объект препятствия с бетоном

    void Start() {
        concreteObject = gameObject.transform.parent.GetComponent<Concrete>(); // Инициализация основного объекта
    }

    public void HitRegistration(int hitPower, bool isPlayerBullet = false) {
        // Обработка попадания по блоку
        concreteObject.HitHandler(blockNum, hitPower, isPlayerBullet);
    }
}