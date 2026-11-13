using UnityEngine;
using UnityEngine.UI;

public class HighScoreDisplay : MonoBehaviour
{
    public Text easyScoreText;
    public Text normalScoreText;
    public Text hardScoreText;

    void Start()
    {
        ShowHighScores();
    }

    void ShowHighScores()
    {
        // PlayerPrefsから各難易度のハイスコアを取得
        int easyScore = PlayerPrefs.GetInt("HIGH_SCORE_Easy", 0);
        int normalScore = PlayerPrefs.GetInt("HIGH_SCORE_Normal", 0);
        int hardScore = PlayerPrefs.GetInt("HIGH_SCORE_Hard", 0);

        // 指定フォーマットで表示
        if (easyScoreText != null)
            easyScoreText.text = $"最高売上げ: {easyScore} スコア";

        if (normalScoreText != null)
            normalScoreText.text = $"最高売上げ: {normalScore} スコア";

        if (hardScoreText != null)
            hardScoreText.text = $"最高売上げ: {hardScore} スコア";
    }
}
