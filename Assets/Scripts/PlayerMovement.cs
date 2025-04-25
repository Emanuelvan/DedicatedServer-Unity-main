using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float minMovementThreshold = 0.001f; // Minimum distance threshold to send updates
    public float updateInterval = 0.1f; // Minimum time between position updates
    private Vector3 lastPosition;
    private float lastUpdateTime;
    private GameManager gameManager;
    private PlayerController playerController;

    void Start()
    {
        lastPosition = transform.position;
        gameManager = FindFirstObjectByType<GameManager>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Only allow movement if player is connected
        if (!gameManager.IsPlayerConnected(playerController.playerId))
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        transform.Translate(speed * Time.deltaTime * movement, Space.World);

        // Only send position updates if the player has moved enough AND enough time has passed
        if (Vector3.Distance(transform.position, lastPosition) > minMovementThreshold &&
            Time.time - lastUpdateTime >= updateInterval)
        {
            if (gameManager != null && playerController != null)
            {
                gameManager.SendPlayerPosition(playerController.playerId);
                lastPosition = transform.position;
                lastUpdateTime = Time.time;
            }
        }
    }
}
