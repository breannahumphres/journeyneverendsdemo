using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance {get; private set; }
    private Transform playerTransform;
    public Vector3 offset = new Vector3(0,2,-10);

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindPlayer();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (playerTransform != null)
        {
            FollowPlayer();
        }
    }

    public void FindPlayer()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }
    private void FollowPlayer()
    {
        Vector3 targetPosition = playerTransform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
    }
}
