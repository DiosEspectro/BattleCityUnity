using UnityEngine;

public class BricksVisual : MonoBehaviour {
    // ����� ������� �� ���������� � ����������� � ���������

    [SerializeField]
    private int brickNum; // ����� �������
    private Bricks bricksObject; // ������ �� �������� ������ ����������� � ���������

    void Start() {
        bricksObject = gameObject.transform.parent.GetComponent<Bricks>(); // ������������� ��������� �������
    }

    public void HitRegistration(string direction, int hitPower, bool isPlayerBullet = false) {
        // ��������� ��������� �� �������
        bricksObject.HitHandler(brickNum, direction, hitPower, isPlayerBullet);
    }
}