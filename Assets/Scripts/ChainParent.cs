using UnityEngine;

public class ChainParent : MonoBehaviour
{
    public static ChainParent Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        transform.position = Vector3.zero;
    }
}
