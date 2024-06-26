using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField]
    private Player player;

    private Animator animator;
    private string movingDirection;
    private int type = 1;

    private const string IS_MOVING = "IsMoving";

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {

    }

    private void AdjustPlayerFacingDirection() {
        movingDirection = Player.Instance.GetMovingDirection();
    }

    private void SwitchAnimatorTo(string direction) {
        animator.SetBool("IsUp", direction == "UP" ? true : false);
        animator.SetBool("IsDown", direction == "DOWN" ? true : false);
        animator.SetBool("IsLeft", direction == "LEFT" ? true : false);
        animator.SetBool("IsRight", direction == "RIGHT" ? true : false);

        Debug.Log(gameObject.gameObject.GetInstanceID() + ") " + animator.GetParameter(1).name + " = " + animator.GetParameter(1).ToString());
    }
}