using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    [Header("材料データ")]
    public List<IngredientData> ingredientDataList;

    [Header("レシピ本UI")]
    public Text recipeBookText;

    public List<RecipeStep> currentRecipe { get; private set; }

    // ランダムレシピ生成
    public void GenerateRecipe()
    {
        currentRecipe = new List<RecipeStep>();

        IngredientType[] powders = { IngredientType.PowderA, IngredientType.PowderB, IngredientType.PowderC, IngredientType.PowderD };
        IngredientType[] tools = { IngredientType.Stir, IngredientType.Bellows };

        Difficulty diff = GameManager.Instance.currentDifficulty;

        int minStep = 4;
        int maxStep = 8;

        switch (diff)
        {
            case Difficulty.Easy:
                minStep = 4;
                maxStep = 6;
                break;

            case Difficulty.Normal:
                minStep = 5;
                maxStep = 7;
                break;

            case Difficulty.Hard:
                minStep = 6;
                maxStep = 8;
                break;
        }

        int totalSteps = Random.Range(minStep, maxStep);
        IngredientType lastIngredient = IngredientType.Water;

        // 最初は水
        currentRecipe.Add(new RecipeStep
        {
            ingredient = IngredientType.Water,
            count = 1,
            description = "水を入れる"
        });

        for (int i = 1; i < totalSteps; i++)
        {
            List<IngredientType> candidates = new List<IngredientType>();
            candidates.AddRange(powders);

            if (i >= 2)
            {
                if (diff == Difficulty.Hard)
                {
                    candidates.AddRange(tools);
                }
                else if (diff == Difficulty.Normal)
                {
                    candidates.Add(IngredientType.Stir);
                }
            }

            candidates.Remove(lastIngredient);

            IngredientType selected = candidates[Random.Range(0, candidates.Count)];
            int count = System.Array.Exists(powders, p => p == selected) ? Random.Range(1, 3) : 1;

            currentRecipe.Add(new RecipeStep
            {
                ingredient = selected,
                count = count,
                description = GetStepText(selected, count)
            });

            lastIngredient = selected;
        }

        currentRecipe.Add(new RecipeStep
        {
            ingredient = IngredientType.None,
            count = 1,
            description = "完成！"
        });
    }

    string GetStepText(IngredientType type, int count)
    {
        switch (type)
        {
            case IngredientType.Water: return "水を入れる";
            case IngredientType.PowderA: return $"白い粉を{count}回入れる";
            case IngredientType.PowderB: return $"赤い粉を{count}回入れる";
            case IngredientType.PowderC: return $"青い粉を{count}回入れる";
            case IngredientType.PowderD: return $"黄色い粉を{count}回入れる";
            case IngredientType.Stir: return "鍋を混ぜる";
            case IngredientType.Bellows: return "火力を上げる";
            default: return "";
        }
    }

    public void DisplayRecipeBook()
    {
        if (recipeBookText == null)
        {
            Debug.LogError("recipeBookText がセットされていません！");
            return;
        }

        StringBuilder sb = new StringBuilder();
        int stepNumber = 1;

        foreach (var step in currentRecipe)
        {
            sb.AppendLine($"{stepNumber}. {step.description}");
            stepNumber++;
        }

        recipeBookText.text = sb.ToString();
        recipeBookText.gameObject.SetActive(true);
    }

    public IngredientData GetIngredientData(IngredientType type)
    {
        var data = ingredientDataList.Find(x => x.type == type);
        if (data == null)
            Debug.LogError("IngredientDataが設定されていません: " + type);
        return data;
    }
}
