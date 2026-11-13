using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    [Header("レシピ")]
    public RecipeManager recipeManager;

    [Header("難易度設定")]
    public Difficulty currentDifficulty = Difficulty.Hard;

    [Header("ツールボタン")]
    public GameObject stirButton;
    public GameObject bellowsButton;

    [Header("ポップアップ")]
    public GameObject inPopupPrefab;
    public GameObject usePopupPrefab;

    [Header("リセットメッセージ")]
    public GameObject resetMessagePrefab;

    public List<IngredientType> playerInput;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        int diffValue = PlayerPrefs.GetInt("Difficulty", (int)Difficulty.Hard);
        currentDifficulty = (Difficulty)diffValue;

        ApplyDifficultySettings(); 

        ChangeState(GameState.Playing);

        // レシピ生成
        recipeManager.GenerateRecipe();
        recipeManager.DisplayRecipeBook();

        // ビンの液体の色をランダムに変える
        LiquidColorManager liquidManager = FindFirstObjectByType<LiquidColorManager>();
        if (liquidManager != null)
        {
            liquidManager.SetRandomLiquidColor(recipeManager.currentRecipe);
        }
        playerInput = new List<IngredientType>();

        UIManager.Instance.recipeBookPanel.SetActive(true);

        FindFirstObjectByType<PotionNameGenerator>().GeneratePotionName();
    }

    void ApplyDifficultySettings()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                stirButton.SetActive(false);
                bellowsButton.SetActive(false);
                break;

            case Difficulty.Normal:
                stirButton.SetActive(true);
                bellowsButton.SetActive(false);
                break;

            case Difficulty.Hard:
                stirButton.SetActive(true);
                bellowsButton.SetActive(true);
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        UIManager.Instance.UpdateUI(newState);
    }

    // Inspector で type を設定できる
    public void OnClickIngredient(int type)
    {
        Vector3 buttonPos = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.position;

        playerInput.Add((IngredientType)type);
        Debug.Log("入力: " + ((IngredientType)type).ToString());

        GameObject prefabToSpawn;
        if (type <= 5)
            prefabToSpawn = inPopupPrefab;
        else
            prefabToSpawn = usePopupPrefab;

        Instantiate(prefabToSpawn, buttonPos, Quaternion.identity);
    }

    public void ResetPlayerInput()
    {
        if (CurrentState != GameState.Playing) return;

        playerInput.Clear();
        Debug.Log("入力をリセットしました");

        // サークルリセット
        LongPressButton[] buttons = FindObjectsOfType<LongPressButton>();
        foreach (var button in buttons)
            button.ResetHold(); // public にしておくこと

        // リセットメッセージ表示
        ShowResetMessage();
    }

    public void ShowResetMessage()
    {
        if (resetMessagePrefab == null) return;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("Scene 内に Canvas が見つかりません");
            return;
        }

        // まず親なしで生成
        GameObject msg = Instantiate(resetMessagePrefab);

        // 位置リセットして Canvas の下に移動
        RectTransform rt = msg.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.SetParent(canvas.transform, false); // false でローカル座標リセット
            rt.localPosition = Vector3.zero;       // 中央に表示
        }

        // フェード処理
        CanvasGroup canvasGroup = msg.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            StartCoroutine(FadeOutAndDestroy(canvasGroup, 1f, 1f));
    }

    private IEnumerator FadeOutAndDestroy(CanvasGroup cg, float displayTime, float fadeTime)
    {
        cg.alpha = 1f;
        yield return new WaitForSeconds(displayTime);

        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);
            yield return null;
        }

        Destroy(cg.gameObject);
    }

    public void GoToResult()
    {
        ChangeState(GameState.Result);
    }

    public void CheckResult()
    {
        bool isSuccess = true;
        int stepIndex = 0;

        foreach (var step in recipeManager.currentRecipe)
        {
            if (step.ingredient == IngredientType.None) break;

            for (int i = 0; i < step.count; i++)
            {
                if (stepIndex >= playerInput.Count || playerInput[stepIndex] != step.ingredient)
                {
                    isSuccess = false;
                    break;
                }
                stepIndex++;
            }

            if (!isSuccess) break;
        }

        if (playerInput.Count > stepIndex)
            isSuccess = false;

        UIManager.Instance.ShowResultUI(isSuccess);
        if (isSuccess)
        {
            ScoreManager.Instance.AddScore(1);
        }
        else
        {
            LifeManager.Instance.LoseLife();
        }

        playerInput.Clear(); // 入力リセット
    }

    public void PauseGame() => ChangeState(GameState.Pause);
    public void ResumeGame() => ChangeState(GameState.Playing);

    public void ToggleRecipeBook(bool show)
    {
        UIManager.Instance.recipeBookPanel.SetActive(show);
    }

    // もう一度ボタン用
    public void RestartGame()
    {
        // 現在のシーンを再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
