using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public float throwForceMultiplier = 10f;
    public float stopThreshold = 0.5f;

    private Rigidbody rb;
    private bool hasBeenThrown = false;
    private Renderer rend;
    private Color originalColor;

    private bool isDragging = false;
    private Vector3 mouseDownPos;
    private Vector3 mouseUpPos;

    private GameManager gameManager;

    public enum Team { Red, Blue }
    public Team ballTeam; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }

        gameManager = FindObjectOfType<GameManager>();
    }

    void OnMouseDown()
    {
        if (!hasBeenThrown && gameManager != null && gameManager.IsBallOnCurrentTeam(this))
        {
            mouseDownPos = Input.mousePosition;
            isDragging = true;
        }
    }

    void OnMouseUp()
    {
        if (isDragging && !hasBeenThrown && gameManager != null && gameManager.IsBallOnCurrentTeam(this))
        {
            mouseUpPos = Input.mousePosition;
            Vector3 throwDirection = (mouseUpPos - mouseDownPos);

            Vector3 worldDirection = Camera.main.transform.forward * throwDirection.y
                                     + Camera.main.transform.right * throwDirection.x;
            worldDirection = worldDirection.normalized * (throwDirection.magnitude * throwForceMultiplier);

            rb.AddForce(worldDirection);
            hasBeenThrown = true;
            isDragging = false;

            // Notify the GameManager that this ball has been thrown
            if (gameManager != null)
            {
                gameManager.OnBallThrown(this);
            }
        }

        // Reset dragging state
        isDragging = false;
    }

    void Update()
    {
        // If thrown and moving slowly stop completely
        if (hasBeenThrown && rb.velocity.magnitude < stopThreshold && rb.velocity.magnitude > 0f)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void HighlightBall(Color highlight)
    {
        if (rend != null)
        {
            rend.material.color = highlight;
        }
    }

    public void RevertColor()
    {
        if (rend != null)
        {
            rend.material.color = originalColor;
        }
    }

    public bool HasBeenThrown()
    {
        return hasBeenThrown;
    }

    public bool IsStopped()
    {
        return hasBeenThrown && rb.velocity.magnitude < stopThreshold;
    }
}