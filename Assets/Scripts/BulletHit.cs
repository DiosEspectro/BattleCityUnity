using UnityEngine;

public class BulletHit : MonoBehaviour {
    // Класс маленького взрыва пули

    void Start() {
        Destroy(gameObject, 1f);
    }

    public void SetSmallHit(float scale = 0.30f) {
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }
}