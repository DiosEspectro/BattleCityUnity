using UnityEngine;

public class Environment : MonoBehaviour {
    // ������� ����� ��� ��������� (�������� �� ������� ����)
    // ����� ����� ����������� �������
    public void DestroyObject(float sec) {
        Destroy(gameObject, sec);
    }
}