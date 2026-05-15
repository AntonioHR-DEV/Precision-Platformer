using UnityEngine;

public class EndCheckpointPressed : StateMachineBehaviour
{
    private float pushUpForce = 20f;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       PlayerController.Instance.Rb.linearVelocity = new Vector2(PlayerController.Instance.Rb.linearVelocity.x, pushUpForce);
    }
}
