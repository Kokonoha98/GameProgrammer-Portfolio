using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject recipeBookPanel;
    public GameObject successFailureCanvas;
    public GameObject successPanel;
    public GameObject failurePanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject resultPanel;

    public Transform canvasTransform;

    public RecipeManager recipeManager;

    public ResultManager resultManager;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateUI(GameState state)
    {
        // 全部消す
        recipeBookPanel.SetActive(false);
        successFailureCanvas.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        resultPanel.SetActive(false);

        // 必要なものだけ表示
        switch (state)
        {
            case GameState.Pause:
                pausePanel.SetActive(true);
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                gameOverPanel.SetActive(true);
                break;

            case GameState.Result:
                resultPanel.SetActive(true);
                resultManager.ShowResult();
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                break;
        }
    }

    // 成功/失敗結果の表示
    public void ShowResultUI(bool isSuccess)
    {
        successFailureCanvas.SetActive(true);
        successPanel.SetActive(isSuccess);
        failurePanel.SetActive(!isSuccess);
    }

    public void HideResultUI()
    {
        successFailureCanvas.SetActive(false);

        //名前を生成
        FindFirstObjectByType<PotionNameGenerator>().GeneratePotionName();

        // レシピ生成
        recipeManager.GenerateRecipe();
        recipeManager.DisplayRecipeBook();

        // ビンの液体の色をランダムに変える
        LiquidColorManager liquidManager = FindFirstObjectByType<LiquidColorManager>();
        if (liquidManager != null)
        {
            liquidManager.SetRandomLiquidColor(recipeManager.currentRecipe);
        }

        recipeBookPanel.SetActive(true);
    }
}
