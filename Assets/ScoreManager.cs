using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text finalScoreText;
    public Text highScoreText;
    public Text scoreTextPrefab;
    public Text totalScoreTextPrefab;
    public GameObject scoreCard;
    public Image scoreCardBacking;

    private GameManager gameManager;
    private int[] frameScores;
    private int[] frameBonuses;
    private int totalScore = 0;

    private Vector3 initialScorePosition;
    private Vector3 initialTotalScorePosition;
    private float frameOffset = 47.5f;
    private float frameTotalScoreOffset = 47.7f;
    private float innerFrameOffset = 23.4f;
    private int frameScore = 0;
    private int highScore;
    private bool lastFrameBonus = false;
    private bool lastFrameSpareOverride = false; // For case where spare can be scored on a bonus throw

    
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        frameScores = new int[10];
        frameBonuses = new int[10];
        initialScorePosition = new Vector3(-268f, 37.4f,0);
        initialTotalScorePosition = new Vector3(-257f, 10.3f, 0);

        finalScoreText.gameObject.SetActive(false);
        highScoreText.gameObject.SetActive(false);
        highScore = PlayerPrefs.GetInt("highscore", highScore);
    }

    public void setScore(int frameIndex) {
        Text totalScoreText = Instantiate(totalScoreTextPrefab, initialTotalScorePosition + Vector3.right * (frameIndex * frameTotalScoreOffset), Quaternion.identity);
        int totalInFrame = 0;
        for (int j = 0; j <= frameIndex; j++)
        {
            totalInFrame += frameScores[j];
        }
        totalScoreText.text = totalInFrame.ToString();
        totalScoreText.transform.SetParent(scoreCard.transform, false);

    }

    public void centreScoreCard() {
        // Set new high score if it has been beaten
        int totalScore = 0;
        for (int i = 0; i < 10; i++) {
            totalScore += frameScores[i];
        }
        if (totalScore > highScore) {
            highScore = totalScore;
            PlayerPrefs.SetInt("highscore",totalScore);
        }

        // Display score details
        gameManager.setCountingScore(false);
        gameManager.setCountingTimeout(false);
        scoreCardBacking.color = Color.white;
        RectTransform cardTransform = scoreCard.GetComponent<RectTransform>();
        cardTransform.anchorMin = new Vector2(0.5f, 0.5f);
        cardTransform.anchorMax = new Vector2(0.5f, 0.5f);
        cardTransform.pivot = new Vector2(0.5f, 0.5f);
        cardTransform.anchoredPosition = Vector2.zero;
        finalScoreText.gameObject.SetActive(true);
        highScoreText.gameObject.SetActive(true);
        finalScoreText.text = "Score: " + totalScore.ToString();
        highScoreText.text = "High Score: " + highScore.ToString();

    }

    public void updateBox(int score, int frameIndex, int boxIndex) {
        frameScore = (boxIndex == 0) ? score : frameScore + score;
        Text scoreText;
        scoreText = Instantiate(scoreTextPrefab, initialScorePosition + Vector3.right * ((frameIndex * frameOffset) + (boxIndex * innerFrameOffset)), Quaternion.identity);
        scoreText.transform.SetParent(scoreCard.transform, false);
        frameScores[frameIndex] += score;

        for (int i = 0; i < 10; i++)
        {
            if (frameBonuses[i] > 0)
            {
                frameScores[i] += score;
                frameBonuses[i]--;
                if (frameBonuses[i] == 0)
                {
                    Debug.Log("End of bonus detected");
                    setScore(i);
                }
            }
        }

        // Last frame logic is convoluted, handle separately
        if (frameIndex == 9) {
            if (boxIndex == 0)
            {
                if (score == 0)
                {
                    scoreText.text = "-";
                }
                else if (score < 10)
                {
                    scoreText.text = score.ToString();
                }
                else if (score == 10)
                {
                    scoreText.text = "X";
                    lastFrameBonus = true;
                }
            }
            else if (boxIndex == 1)
            {
                if (score == 0)
                {
                    scoreText.text = "-";
                    lastFrameSpareOverride = true;
                }
                else if (score < 10)
                {
                    if (frameScores[frameIndex] < 10 || frameScores[frameIndex] > 10)
                    {
                        scoreText.text = score.ToString();
                    }
                    else if (frameScores[frameIndex] == 10)
                    {
                        scoreText.text = "/";
                        lastFrameBonus = true;
                    }
                }
                else if (score == 10)
                {
                    if (frameScores[frameIndex] == score)
                    {
                        scoreText.text = "/";
                        lastFrameBonus = true;
                    }
                    else
                    {
                        scoreText.text = "X";
                    }
                }
                // Set score if over
                if (!lastFrameBonus) {
                    setScore(frameIndex);
                    gameManager.endGame();
                }
            }
            else if (boxIndex == 2) {
                if (score == 0)
                {
                    scoreText.text = "-";
                }
                else if (score < 10)
                {
                    if (gameManager.getRemainingPinCount() == 0)
                    {
                        scoreText.text = "/";
                    }
                    else
                    {
                        scoreText.text = score.ToString();
                    }
                }
                else if (score == 10) {
                    if (lastFrameSpareOverride)
                    {
                        scoreText.text = "/";
                    }
                    else {
                        scoreText.text = "X";
                    }
                }
                // Set score
                setScore(frameIndex);
                gameManager.endGame();
            }
            return;
        }


        if (score == 0)
        {
            // No hit
            scoreText.text = "-";
        }
        else if ((score > 0 && score < 10) || (score == 10 && boxIndex == 1))
        {
            // Hit or spare
            if (frameScore < 10)
            {
                scoreText.text = score.ToString();
            }
            else {
                scoreText.text = "/";
                if (frameIndex != 9) {
                    frameBonuses[frameIndex] = 1;
                }
            }
            if (boxIndex == 1) {
                if (frameScore < 10)
                {
                    //frameScores[gameManager.getFrameIndex()] += score;
                    //setScore(gameManager.getFrameIndex());
                    //setScore(gameManager.getFrameIndex(), frameScore, 0);
                }
            }
        }
        else if (score == 10)
        {
            // Strike
            scoreText.text = "X";
            if (frameIndex != 9) {
                frameBonuses[frameIndex] = 2;
            }
        }

        // Set score if no bonuses left for frame
        int totalBonuses = 0;
        for (int i = 0; i <= frameIndex; i++)
        {
            totalBonuses += frameBonuses[i];
        }
        if (totalBonuses == 0 && boxIndex == 1)
        {
            setScore(frameIndex);
        }
    }

    public bool getLastFrameBonus() {
        return lastFrameBonus;
    }
}
