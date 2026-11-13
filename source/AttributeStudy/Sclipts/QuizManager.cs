using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("難易度表示UI")]
    public Text difficultyText;

    [Header("問題データ")]
    public List<CreatureData> allBeginnerCreatures;
    public List<CreatureData> allIntermediateCreatures;
    public List<CreatureData> allAdvancedCreatures;

    private static HashSet<string> recentQuestionIds = new HashSet<string>();

    private List<CreatureData> questions;
    private int currentIndex = 0;
    private int correctCount = 0;

    private ElementType currentAnswer;

    private bool isAnswering = false;


    [Header("UI（問題表示）")]
    public Text nameText;
    public Text nameText2;
    public Image iconImage;
    public Image iconImage2;
    public Text habitatText;
    public Text environmentText;
    public Text dietText;
    public Text sizeText;
    public Text activityText;
    //public Text resultText;

    [Header("回答演出")]
    public AnswerEffectPlayer effectPlayer;

    [Header("演出UI（学者）")]
    public Image scholarImage;
    public Sprite normalFace;
    public Sprite worriesFace;
    public Sprite happyFace;
    public Sprite surprisedFace;

    [Header("演出UI（吹き出し）")]
    public GameObject speechBubble;
    public Image resultMark;
    public Sprite correctSprite; // ○
    public Sprite wrongSprite;   // ×

    [Header("レア演出")]
    public Image rareNoiseImage;
    public GameObject scholarMessageRoot;
    public Text scholarMessageText;

    private bool canSpawnRare = false;

    [Header("進行表示UI")]
    public Text questionProgressText;

    [Header("ボタン")]
    public Button[] answerButtons;

    void Start()
    {
        speechBubble.SetActive(false);
        scholarImage.sprite = normalFace;

        // レア演出初期化
        if (rareNoiseImage != null)
            rareNoiseImage.color = Color.clear;

        // 学者メッセージ初期化
        if (scholarMessageRoot != null)
            scholarMessageRoot.SetActive(false);

        if (scholarMessageText != null)
            scholarMessageText.text = "";

        StartGame();
    }

    List<CreatureData> GetCurrentDifficultyList()
    {
        switch (GameSettings.currentDifficulty)
        {
            case Difficulty.Beginner:
                return allBeginnerCreatures;

            case Difficulty.Intermediate:
                return allIntermediateCreatures;

            case Difficulty.Advanced:
                return allAdvancedCreatures;

            default:
                return allBeginnerCreatures;
        }
    }

    string GetDifficultyName(Difficulty diff)
    {
        switch (diff)
        {
            case Difficulty.Beginner:
                return "初級";

            case Difficulty.Intermediate:
                return "中級";

            case Difficulty.Advanced:
                return "上級";

            default:
                return "";
        }
    }

    void StartGame()
    {
        var sourceList = GetCurrentDifficultyList();

        // 通常問題（レア除外）
        var normalList = sourceList
            .Where(c => !c.isRare)
            .ToList();

        questions = CreateWeightedQuestions(normalList, 7);

        // =========================
        // レア問題抽選
        // =========================

        List<CreatureData> rareList = sourceList
            .Where(c => c.isRare)
            .ToList();

        // 30%で出現
        if (rareList.Count > 0 && Random.value < 0.3f)
        {
            CreatureData rare = null;

            // 未発見レア優先
            List<CreatureData> undiscoveredRare = rareList
                .Where(c => !EncyclopediaManager.Instance.IsDiscovered(c))
                .ToList();

            // 発見済み未解放
            List<CreatureData> discoveredNotUnlockedRare = rareList
                .Where(c =>
                    EncyclopediaManager.Instance.IsDiscovered(c) &&
                    !EncyclopediaManager.Instance.IsUnlocked(c))
                .ToList();

            if (undiscoveredRare.Count > 0)
            {
                rare = undiscoveredRare[
                    Random.Range(0, undiscoveredRare.Count)
                ];
            }
            else if (discoveredNotUnlockedRare.Count > 0)
            {
                rare = discoveredNotUnlockedRare[
                    Random.Range(0, discoveredNotUnlockedRare.Count)
                ];
            }
            else
            {
                // 全部解放済みならランダム
                rare = rareList[
                    Random.Range(0, rareList.Count)
                ];
            }

            // 問題に差し込み
            int insertIndex = Random.Range(3, questions.Count);

            questions[insertIndex] = rare;

            // 重複防止
            if (!questions.Contains(rare))
            {
                questions.Add(rare);
            }
        }

        currentIndex = 0;
        correctCount = 0;

        difficultyText.text = GetDifficultyName(GameSettings.currentDifficulty);

        UpdateProgressUI();

        SetQuestion(questions[currentIndex]);
    }

    void SetQuestion(CreatureData data)
    {
        isAnswering = false;
        SetButtons(true);

        //出題→発見
        EncyclopediaManager.Instance.Discover(data);

        RuntimeQuestion q = CreateQuestion(data);

        nameText.text = q.name;
        nameText2.text = q.name;
        iconImage.sprite = q.icon;
        iconImage.color = data.iconColor;
        iconImage2.sprite = q.icon;
        iconImage2.color = data.iconColor;
        habitatText.text = q.habitat;
        environmentText.text = q.environment;
        dietText.text = q.diet;
        sizeText.text = q.size;
        activityText.text = q.activityTime;

        currentAnswer = q.answer;

        //resultText.text = "";

        // 演出リセット
        speechBubble.SetActive(false);
        scholarImage.sprite = normalFace;
        iconImage.gameObject.SetActive(true);
        iconImage.transform.rotation = Quaternion.identity;
        iconImage.transform.localScale = Vector3.one;

        UpdateProgressUI();


        // レア演出
        if (data.isRare && canSpawnRare)
        {
            StartCoroutine(PlayRareEffect());
        }
        else
        {
            HideScholarMessage();
        }
    }

    RuntimeQuestion CreateQuestion(CreatureData data)
    {
        RuntimeQuestion q = new RuntimeQuestion();

        q.name = data.creatureName;
        q.icon = data.icon;
        iconImage.sprite = q.icon;
        iconImage.color = data.iconColor;
        iconImage2.sprite = q.icon;
        iconImage2.color = data.iconColor;

        var list = GetRandomMultiple(data.environments, 3);

        string line1 = "";
        string line2 = "";

        if (list.Count >= 1)
            line1 += $"{list[0].name}：{list[0].rank}";

        if (list.Count >= 2)
            line1 += "　　" + $"{list[1].name}：{list[1].rank}"; // スペースで横並び

        if (list.Count >= 3)
            line2 = $"{list[2].name}：{list[2].rank}";

        q.environment = line1 + "\n" + line2;

        q.answer = data.element;

        q.habitat = GetRandom(data.habitats);
        q.diet = GetRandom(data.diets);

        if (data.activityTimes.Contains("常時"))
        {
            q.activityTime = GetRandom(new List<string> { "昼", "夜", "雨" });
        }
        else
        {
            q.activityTime = GetRandom(data.activityTimes);
        }

        if (data.size == "???")
        {
            q.size = GetRandom(new List<string> { "小型", "中型", "大型" });
        }
        else
        {
            q.size = data.size;
        }

        return q;
    }

    // レアメッセージ表示
    void ShowScholarMessage(string message)
    {
        if (scholarMessageRoot != null)
            scholarMessageRoot.SetActive(true);

        if (scholarMessageText != null)
            scholarMessageText.text = message;
    }

    // レアメッセージ非表示
    void HideScholarMessage()
    {
        if (scholarMessageRoot != null)
            scholarMessageRoot.SetActive(false);

        if (scholarMessageText != null)
            scholarMessageText.text = "";
    }

    // レア演出
    IEnumerator PlayRareEffect()
    {
        // 学者反応
        scholarImage.sprite = surprisedFace;

        ShowScholarMessage("……！？");

        // ノイズ演出（属性ヒントなし）
        if (rareNoiseImage != null)
        {
            Color noiseColor = new Color(0, 0, 0, 0.6f);

            rareNoiseImage.color = noiseColor;
            yield return new WaitForSeconds(0.1f);

            rareNoiseImage.color = Color.clear;
            yield return new WaitForSeconds(0.05f);

            rareNoiseImage.color = noiseColor;
            yield return new WaitForSeconds(0.1f);

            rareNoiseImage.color = Color.clear;
        }

        yield return new WaitForSeconds(0.6f);

        ShowScholarMessage("未知の反応を検出…");

        yield return new WaitForSeconds(1.0f);

        ShowScholarMessage("レア個体…判断に注意");
    }

    List<EnvironmentData> GetRandomMultiple(List<EnvironmentData> list, int count)
    {
        return list.OrderBy(x => Random.value).Take(count).ToList();
    }

    string GetRandom(List<string> list)
    {
        if (list == null || list.Count == 0) return "不明";
        return list[Random.Range(0, list.Count)];
    }

    //  回答処理（演出メイン）
    public void Answer(int elementIndex)
    {
        if (isAnswering) return;

        StartCoroutine(AnswerRoutine(elementIndex));
    }

    // 進行表示
    void UpdateProgressUI()
    {
        questionProgressText.text = $"{currentIndex + 1}/{questions.Count}";
    }

    void NextQuestion()
    {
        currentIndex++;

        HideScholarMessage();

        if (rareNoiseImage != null)
            rareNoiseImage.color = Color.clear;

        if (currentIndex >= questions.Count)
        {
            ShowResult();
            return;
        }

        // レア問題出題タイミング
        canSpawnRare = (currentIndex >= 3);

        UpdateProgressUI();

        SetQuestion(questions[currentIndex]);

        isAnswering = false;
    }

    void SetButtons(bool active)
    {
        foreach (var b in answerButtons)
        {
            b.interactable = active;
        }
    }

    void ShowResult()
    {
        GameManager.Instance.ShowResult(correctCount, questions.Count);

        //resultText.text = $"調査完了\n正解数：{correctCount} / {questions.Count}";
    }

    IEnumerator AnswerRoutine(int elementIndex)
    {
        isAnswering = true;

        SetButtons(false);

        ElementType answer = (ElementType)elementIndex;

        bool isCorrect = answer == currentAnswer;

        Button pressedButton = answerButtons[elementIndex];

        yield return new WaitForSeconds(0.1f);
        scholarImage.sprite = worriesFace;

        // 演出再生
        yield return StartCoroutine(
            effectPlayer.PlayEffect(
                iconImage,
                pressedButton,
                isCorrect
            )
        );

        // 判定後UI
        speechBubble.SetActive(true);

        if (isCorrect)
        {
            correctCount++;

            if (EncyclopediaManager.Instance != null)
            {
                EncyclopediaManager.Instance.Unlock(
                    questions[currentIndex]
                );
            }

            scholarImage.sprite = happyFace;
            resultMark.sprite = correctSprite;
        }
        else
        {
            scholarImage.sprite = surprisedFace;
            resultMark.sprite = wrongSprite;
        }

        yield return new WaitForSeconds(1.3f);

        NextQuestion();
    }

    List<CreatureData> CreateWeightedQuestions(List<CreatureData> sourceList, int count)
    {
        List<CreatureData> result = new List<CreatureData>();

        // 未出題（50%）
        List<CreatureData> undiscovered = sourceList
            .Where(c => !EncyclopediaManager.Instance.IsDiscovered(c))
            .ToList();

        // 出題済み未解放（30%）
        List<CreatureData> discoveredNotUnlocked = sourceList
            .Where(c =>
                EncyclopediaManager.Instance.IsDiscovered(c) &&
                !EncyclopediaManager.Instance.IsUnlocked(c))
            .ToList();

        // 解放済み（20%）
        List<CreatureData> unlocked = sourceList
            .Where(c => EncyclopediaManager.Instance.IsUnlocked(c))
            .ToList();

        int safety = 0;

        while (result.Count < count && safety < 500)
        {
            safety++;

            float rand = Random.value;

            List<CreatureData> targetList;

            // 50%
            if (rand < 0.5f)
            {
                targetList = undiscovered;
            }
            // 30%
            else if (rand < 0.8f)
            {
                targetList = discoveredNotUnlocked;
            }
            // 20%
            else
            {
                targetList = unlocked;
            }

            // 空なら全体
            if (targetList.Count == 0)
            {
                targetList = sourceList;
            }

            // =========================
            // 最近出た問題を除外
            // =========================

            List<CreatureData> filtered = targetList
                .Where(c => !recentQuestionIds.Contains(c.id))
                .ToList();

            if (filtered.Count > 0)
            {
                targetList = filtered;
            }

            CreatureData picked =
                targetList[Random.Range(0, targetList.Count)];

            // 重複防止
            if (!result.Contains(picked))
            {
                result.Add(picked);
            }

            // 問題不足対策
            if (result.Count >= sourceList.Count)
            {
                break;
            }
        }

        // 今回問題を保存
        recentQuestionIds.Clear();

        foreach (var q in result)
        {
            recentQuestionIds.Add(q.id);
        }

        return result;
    }

    // =========================
    // ❌ 不正解コメント
    // =========================
    /*string GetWrongComment(ElementType correct)
    {
        switch (correct)
        {
            case ElementType.Fire:
                return "高温耐性はあるが、燃焼反応は確認されていない…？";
            case ElementType.Water:
                return "液体だが、水属性と断定するには根拠が弱い…";
            case ElementType.Wind:
                return "軽量だが、風の流動性とは異なる挙動…";
            case ElementType.Earth:
                return "地表付近に存在するが、地属性の性質とは一致しない…";
            case ElementType.Light:
                return "発光はあるが、光属性由来かは不明…";
            case ElementType.Dark:
                return "夜間活動はあるが、闇属性とは限らない…";
            default:
                return "判定不能…";
        }
    }*/
}
