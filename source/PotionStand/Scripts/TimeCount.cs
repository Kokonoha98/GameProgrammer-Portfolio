using UnityEngine;
using UnityEngine.UI;

public class TimeCount : MonoBehaviour
{
    public float timeLimitSeconds = 300f; // 5分
    private float timeRemaining;

    public Text timerText;

    private bool isTimeUp = false;

    void Start()
    {
        timeRemaining = timeLimitSeconds;
        UpdateTimerUI();
    }

    void Update()
    {
        if (isTimeUp) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isTimeUp = true;
            TimeUp();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        isTimeUp = true;
    }

    void TimeUp()
    {
        Debug.Log("時間切れ！");
        UIManager.Instance.UpdateUI(GameState.Result);
        Time.timeScale = 0f;
    }
}
