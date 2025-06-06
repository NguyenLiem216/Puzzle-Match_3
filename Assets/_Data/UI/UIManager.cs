using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text moveText;
    [SerializeField] private Button restartButton;

     [SerializeField] private TMP_Text targetText;

    [Header("Win/Lose Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Button winPlayAgainButton;
    [SerializeField] private Button losePlayAgainButton;

    private int score = 0;
    private int moves = 10;
    private readonly int targetScore = 1000;
    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        restartButton.onClick.AddListener(RestartGame);
        winPlayAgainButton.onClick.AddListener(RestartGame);
        losePlayAgainButton.onClick.AddListener(RestartGame);

        UpdateScoreUI();
        UpdateMoveUI();

        winPanel.transform.localScale = Vector3.zero;
        losePanel.transform.localScale = Vector3.zero;
    }

    public void AddScore(int amount)
    {
        if (gameEnded) return;

        score += amount;
        UpdateScoreUI();

        if (score >= targetScore)
        {
            Win();
        }
    }

    public void UseMove()
    {
        if (gameEnded) return;

        moves--;
        UpdateMoveUI();

        if (moves <= 0)
        {
            Lose();
        }
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    private void UpdateMoveUI()
    {
        moveText.text = "Moves: " + moves;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Win()
    {
        Debug.Log("You Win!");
        gameEnded = true;
        Invoke(nameof(ShowWinPanel), 1.5f);
    }

    private void Lose()
    {
        Debug.Log("You Lose!");
        gameEnded = true;
        Invoke(nameof(ShowLosePanel), 1.5f);
    }

    public void ShowWinPanel()
    {
        winPanel.transform.localScale = Vector3.zero;
        winPanel.SetActive(true);
        winPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack); 
    }

    public void ShowLosePanel()
    {
        losePanel.transform.localScale = Vector3.zero;
        losePanel.SetActive(true);
        losePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    public void SetTarget(int target)
    {
        targetText.text = "Target: " + target.ToString();
    }

    public void SetMoveLimit(int moveLimit)
    {
        moveText.text = "Moves: " + moveLimit.ToString();
    }

}
