using System.Collections.Generic;
using UnityEngine;

//属性データ
public enum ElementType
{
    Fire,
    Water,
    Wind,
    Earth,
    Light,
    Dark
}
//分類アイコンデータ
public enum CategoryType
{
    Stone,
    Liquid,
    Plant,
    Insect,
    Fish,
    Bird,
    Mammal,
    Reptile
}

[System.Serializable]
public class EnvironmentData
{
    public string name; // 高温・低温など
    public string rank; // ◎〇△×-
}

[CreateAssetMenu(menuName = "Encyclopedia/Creature")]
public class CreatureData : ScriptableObject
{
    [Header("セーブ用ID（重複禁止）")]
    public string id;
    public string creatureName;
    public Sprite icon;
    public Color iconColor;
    public ElementType element;
    public CategoryType category;

    public Difficulty difficulty;

    [Header("レア問題")]
    public bool isRare;

    [TextArea]
    public string flavorText;

    // ▼ 図鑑用（完全情報）
    public List<string> habitats;
    public List<string> diets;
    public List<string> activityTimes;

    public List<EnvironmentData> environments;
    public string size;        // 小型 or ??? など

    public string GetElementName()
    {
        return element.ToString(); // Fire → "Fire"
    }

    public string GetCategoryName()
    {
        return category.ToString();
    }
}
