using UnityEngine;

public class Explode : MonoBehaviour {
    // ����� ��� �������� ������. ������ ������������� �������� � ������������.
    void Start() {
        Destroy(gameObject, 1f);
    }
}