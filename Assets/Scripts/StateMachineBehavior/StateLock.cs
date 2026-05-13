using UnityEngine;

public class StateLock : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.GetComponent<PlayerVisual>().IsLocked = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1f)
        {
            animator.GetComponent<PlayerVisual>().IsLocked = false;
        }
    }
}
