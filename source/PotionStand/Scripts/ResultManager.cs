using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [Header("表示テキスト")]
    public Text scoreCountText;        // ×(スコア)
    public Text scoreValueText;        // (スコア)scores

    public Text lifeCountText;         // ×(残機)
    public Text lifeBonusText;         // (残機×5)scores

    public Text totalScoreText;        // 合計Scores
    public Text highScoreText;         // ハイスコア

    [Header("NEW RECORD画像")]
    public GameObject newRecordImage;

    private int highScore;
    private string highScoreKey;

    void Start()
    {
        // 現在の難易度を取得
        Difficulty currentDifficulty = GameManager.Instance.currentDifficulty;

        // 難易度ごとのキーを作成
        highScoreKey = "HIGH_SCORE_" + currentDifficulty.ToString();

        highScore = PlayerPrefs.GetInt(highScoreKey, 0);

        if (newRecordImage != null)
            newRecordImage.SetActive(false);
    }

    public void ShowResult()
    {
        int score = ScoreManager.Instance.GetScore();
        int lives = LifeManager.Instance.GetCurrentLives();

        // 現在の難易度取得
        Difficulty difficulty = GameManager.Instance.currentDifficulty;

        int lifeMultiplier = 5; // デフォルト

        switch (difficulty)
        {
            case Difficulty.Easy:
                lifeMultiplier = 3;
                break;

            case Difficulty.Normal:
                lifeMultiplier = 4;
                break;

            case Difficulty.Hard:
                lifeMultiplier = 5;
                break;
        }

        int lifeBonus = lives * lifeMultiplier;
        int total = score + lifeBonus;

        // 表示
        scoreCountText.text = "×" + score;
        scoreValueText.text = score + " scores";

        lifeCountText.text = "×" + lives;
        lifeBonusText.text = lifeBonus + " scores";

        totalScoreText.text = total + " Scores";

        bool isNewRecord = false;

        // ハイスコア更新
        if (total > highScore)
        {
            highScore = total;
            PlayerPrefs.SetInt(highScoreKey, highScore);
            PlayerPrefs.Save();

            isNewRecord = true;
        }

        highScoreText.text = highScore + " Scores!!";

        // NEW RECORD画像表示制御
        if (newRecordImage != null)
            newRecordImage.SetActive(isNewRecord);
    }
}
