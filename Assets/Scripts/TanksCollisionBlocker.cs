using UnityEngine;

public class TanksCollisionBlocker : MonoBehaviour {
    // ����������� �������� ������, ����� ��� �� ������� ���� �����

    [SerializeField]
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    private BoxCollider2D bpxCollider2DBlocker;

    private void Start() {
        Physics2D.IgnoreCollision(boxCollider2D, bpxCollider2DBlocker, true);
    }
}