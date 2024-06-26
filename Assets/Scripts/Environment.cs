using UnityEngine;

public class Environment : MonoBehaviour {
    // Базовый класс для окружения (объектов на игровом поле)
    // Общий метод уничтожения объекта
    public void DestroyObject(float sec) {
        Destroy(gameObject, sec);
    }
}