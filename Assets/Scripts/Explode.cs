using UnityEngine;

public class Explode : MonoBehaviour {
    // Класс для большого взрыва. Просто проигрывается анимация и уничтожается.
    void Start() {
        Destroy(gameObject, 1f);
    }
}