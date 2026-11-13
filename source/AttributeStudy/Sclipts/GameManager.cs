using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Panels")]
    public GameObject MemoPanel;
    public GameObject PausePanel;
    public GameObject ResultPanel;

    [Header("リザルトUI")]
    public Text correctRateText;   // ○/7
    public Text encyclopediaText;  // ○%

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        MemoPanel.SetActive(false);
        PausePanel.SetActive(false);
        ResultPanel.SetActive(false);     
    }

    //調査メモを開く
    public void OpenMemoClick()
    {
        MemoPanel.SetActive(true);
    }

    //調査メモを閉じる
    public void CloseMemoClick()
    {
        MemoPanel.SetActive(false);
    }

    //ポーズ
    public void PauseScreenClick()
    {
        PausePanel.SetActive(true);
    }

    //ポーズ解除
    public void UnpauseClick()
    {
        PausePanel.SetActive(false);
    }

    // Result表示
    public void ShowResult(int correctCount, int totalCount)
    {
        ResultPanel.SetActive(true);

        correctRateText.text = $"{correctCount}/{totalCount}";

        float rate = EncyclopediaManager.Instance.GetEncyclopediaRate();
        encyclopediaText.text = $"{Mathf.RoundToInt(rate)}%";
    }
}
