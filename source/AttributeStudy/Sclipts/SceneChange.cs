using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public string SceneName;

    public void ChangeButton()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void LevelChangeButton(int difficulty)
    {
        GameSettings.currentDifficulty = (Difficulty)difficulty;

        SceneManager.LoadScene(SceneName);
    }

    public void RetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
