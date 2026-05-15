using UnityEngine;

public class ChainParent : MonoBehaviour
{
    public static ChainParent Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        transform.position = Vector3.zero;
    }
}
