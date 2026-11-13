using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // ScoreManagerをシングルトン化

    private int currentScore; // 現在のスコアを保存

    private void Awake()
    {
        Instance = this;
    }

    // 現在のスコアを設定
    public void SetCurrentScore(int score)
    {
        currentScore = score;
    }

    // 現在のスコアを取得
    public int GetCurrentScore()
    {
        return currentScore;
    }
}
