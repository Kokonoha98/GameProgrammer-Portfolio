using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    public string SceneName;

    public void ChangeButton()
    {
        SceneManager.LoadScene(SceneName);
    }

    // 難易度選択ボタン用
    public void SelectEasy()
    {
        SetDifficultyAndLoadScene(Difficulty.Easy);
    }

    public void SelectNormal()
    {
        SetDifficultyAndLoadScene(Difficulty.Normal);
    }

    public void SelectHard()
    {
        SetDifficultyAndLoadScene(Difficulty.Hard);
    }

    // PlayerPrefs に保存してからゲームシーンに移動
    private void SetDifficultyAndLoadScene(Difficulty diff)
    {
        PlayerPrefs.SetInt("Difficulty", (int)diff);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneName);
    }
}
