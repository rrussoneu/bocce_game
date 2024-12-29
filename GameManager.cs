using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Transform pallino;
    public GameObject redBallPrefab;
    public GameObject blueBallPrefab;
    public Transform ballSpawnPoint;
    public TextMeshProUGUI winnerText;

    private enum Team { Red, Blue }
    private Team currentTeam = Team.Red;

    private int redBallsThrown = 0;
    private int blueBallsThrown = 0;
    private int maxBallsPerTeam = 3;
    public float spawnDelay = 2f;

    private BallController selectedBall;
    private BallController previouslySelectedBall;
    private Color highlightColor = Color.yellow;
    private bool gameFinished = false;

    // List of all thrown balls for final calculation
    private System.Collections.Generic.List<BallController> thrownBalls = new System.Collections.Generic.List<BallController>();

    void Start()
    {
        SpawnNextBall(); 
    }

    void Update()
    {
        if (gameFinished)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            SelectBallUnderMouse();
        }

        if (AllBallsThrown())
        {
            if (AllBallsStopped())
            {
                DetermineWinner();
                gameFinished = true;
            }
        }
    }

    void SelectBallUnderMouse()
    {
        if (gameFinished) return;
        if (AllBallsThrown()) return; 

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            BallController ball = hit.collider.GetComponent<BallController>();
            if (ball != null && !ball.HasBeenThrown())
            {
                // Only select if it's the current ball (the one just spawned for current team)
                // Deselect previous ball
                if (previouslySelectedBall != null && previouslySelectedBall != ball)
                {
                    previouslySelectedBall.RevertColor();
                }

                selectedBall = ball;
                selectedBall.HighlightBall(highlightColor);
                previouslySelectedBall = selectedBall;
            }
        }
    }

    public void OnBallThrown(BallController ball)
    {
        thrownBalls.Add(ball);
        // Once thrown, switch to the other team if not all thrown
        SwitchTurn();
    }

    void SpawnNextBall()
    {
        if (AllBallsThrown()) return;

        GameObject prefabToSpawn = (currentTeam == Team.Red) ? redBallPrefab : blueBallPrefab;
        var spawnedObj = Instantiate(prefabToSpawn, ballSpawnPoint.position, Quaternion.identity);
        var bc = spawnedObj.GetComponent<BallController>();


        // Deselect any previous ball
        if (previouslySelectedBall != null)
        {
            previouslySelectedBall.RevertColor();
            previouslySelectedBall = null;
        }
        selectedBall = bc; // The newly spawned ball is now the selected one
        selectedBall.HighlightBall(highlightColor);
        previouslySelectedBall = selectedBall;
    }

    void SwitchTurn()
    {
        if (currentTeam == Team.Red)
        {
            redBallsThrown++;
            currentTeam = Team.Blue;
        }
        else
        {
            blueBallsThrown++;
            currentTeam = Team.Red;
        }

        if (!AllBallsThrown())
        {
            StartCoroutine(SpawnNextBallWithDelay(spawnDelay));
        }
    }


    bool AllBallsThrown()
    {
        return (redBallsThrown >= maxBallsPerTeam) && (blueBallsThrown >= maxBallsPerTeam);
    }

    bool AllBallsStopped()
    {
        foreach (var b in thrownBalls)
        {
            if (!b.IsStopped()) return false;
        }
        return true;
    }

    void DetermineWinner()
    {
        BallController closestBall = null;
        float closestDist = float.MaxValue;

        foreach (var b in thrownBalls)
        {
            float dist = Vector3.Distance(b.transform.position, pallino.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestBall = b;
            }
        }

        if (closestBall != null)
        {
            string winnerTeam = (closestBall.ballTeam == BallController.Team.Red) ? "Red" : "Blue";
            if (winnerText != null)
            {
                winnerText.text = winnerTeam + " Team Wins!";
            }
            else
            {
                Debug.Log(winnerTeam + " Team Wins!");
            }
        }
    }

    public bool IsBallOnCurrentTeam(BallController ball)
    {
        
        return (currentTeam == GameManager.Team.Red && ball.ballTeam == BallController.Team.Red) 
               || (currentTeam == GameManager.Team.Blue && ball.ballTeam == BallController.Team.Blue);
    }
    private System.Collections.IEnumerator SpawnNextBallWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNextBall();
    }
}