using UnityEngine;
using UnityEngine.UI;

public class PotionNameGenerator : MonoBehaviour
{
    public Text potionNameText;

    // 下語（共通）
    private string[] bottomWords = new string[]
    {
        "回復薬",
        "ポーション",
        "傷薬",
        "軟膏",
        "解毒薬",
        "リジェネ薬"
    };

    // 難易度別 上語
    private string[] easyTopWords = new string[]
    {
        "小",
        "量産",
        "簡易"
    };

    private string[] normalTopWords = new string[]
    {
        "小",
        "量産",
        "簡易",
        "中",
        "いい",
        "普通の"
    };

    private string[] hardTopWords = new string[]
    {
        "中",
        "いい",
        "普通の",
        "大",
        "凄い",
        "万能"
    };

    public void GeneratePotionName()
    {
        string[] selectedTopWords = GetTopWordsByDifficulty();

        string top = selectedTopWords[Random.Range(0, selectedTopWords.Length)];
        string bottom = bottomWords[Random.Range(0, bottomWords.Length)];

        //コメント生成
        if (CommentManager.Instance != null)
        {
            CommentManager.Instance.ShowComment(bottom);
        }

        string finalName = top + bottom;

        potionNameText.text = finalName;
    }

    private string[] GetTopWordsByDifficulty()
    {
        switch (GameManager.Instance.currentDifficulty)
        {
            case Difficulty.Easy:
                return easyTopWords;

            case Difficulty.Normal:
                return normalTopWords;

            case Difficulty.Hard:
                return hardTopWords;

            default:
                return normalTopWords;
        }
    }
}
