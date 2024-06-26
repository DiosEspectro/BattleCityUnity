using UnityEngine;

public class ConcreteVisual : MonoBehaviour {
    // ����� ������� �� ������ � ����������� � �������

    [SerializeField]
    private int blockNum; // ����� �����
    private Concrete concreteObject; // ������ �� �������� ������ ����������� � �������

    void Start() {
        concreteObject = gameObject.transform.parent.GetComponent<Concrete>(); // ������������� ��������� �������
    }

    public void HitRegistration(int hitPower, bool isPlayerBullet = false) {
        // ��������� ��������� �� �����
        concreteObject.HitHandler(blockNum, hitPower, isPlayerBullet);
    }
}