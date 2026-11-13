using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance;

    [Header("残機設定")]
    public int maxLives = 3;
    private int currentLives;

    [Header("残機画像（左→右の順でセット）")]
    public Image[] lifeImages;

    [Header("タイマー")]
    public TimeCount timeCount; // TimeCountをセット

    private bool isGameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentLives = maxLives;
        UpdateLifeUI();
    }

    // 失敗時に呼ぶ
    public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;

        if (currentLives < 0)
            currentLives = 0;

        UpdateLifeUI();

        if (currentLives == 0)
        {
            isGameOver = true;

            // タイマー停止
            if (timeCount != null)
                timeCount.StopTimer();
        }
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    void UpdateLifeUI()
    {
        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].enabled = i < currentLives;
        }
    }

    // 失敗画面の「解除ボタン」から呼ぶ
    public void OnFailureClosed()
    {
        if (isGameOver)
        {
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
    }
}
