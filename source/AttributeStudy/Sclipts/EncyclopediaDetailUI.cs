using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaDetailUI : MonoBehaviour
{
    public static EncyclopediaDetailUI Instance;

    public GameObject panel;

    public Text nameText;  //名前
    public Image elementIconImage;//属性
    public Image categoryIconImage;  //属性アイコン
    public Text habitatText;  //生息地
    public Text environmentText;  //適性
    public Text dietText;  //食性
    public Text sizeText;  //大きさ
    public Text activityText;  //活動時
    public Text flavorText;  //フレーバーテキスト

    [Header("属性アイコン")]
    public Sprite fireIcon;
    public Sprite waterIcon;
    public Sprite windIcon;
    public Sprite earthIcon;
    public Sprite lightIcon;
    public Sprite darkIcon;
    public Sprite unknownIcon;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        panel.SetActive(false);
    }

    public void Show(CreatureData data)
    {
        panel.SetActive(true);

        bool isDiscovered = EncyclopediaManager.Instance.IsDiscovered(data);
        bool isUnlocked = EncyclopediaManager.Instance.IsUnlocked(data);

        // 未発見
        if (!isDiscovered)
        {
            SetAllUnknown();
            return;
        }

        // 名前とアイコンは表示
        nameText.text = data.creatureName;
        categoryIconImage.sprite = data.icon;
        categoryIconImage.color = data.iconColor;

        // 発見のみ
        if (!isUnlocked)
        {
            SetDetailUnknown();
            return;
        }

        // 完全解放
        elementIconImage.sprite = GetElementIcon(data.element);
        habitatText.text = string.Join(" / ", data.habitats);
        environmentText.text = FormatEnvironment(data.environments);
        dietText.text = string.Join(" / ", data.diets);
        sizeText.text = data.size;
        activityText.text = string.Join(" / ", data.activityTimes);
        flavorText.text = data.flavorText;
    }

    void SetAllUnknown()
    {
        nameText.text = "？？？";
        categoryIconImage.sprite = null;

        SetDetailUnknown();
    }

    void SetDetailUnknown()
    {
        habitatText.text = "？？？";
        flavorText.text = "？？？";
        elementIconImage.sprite = unknownIcon;
        environmentText.text = "？？？";
        dietText.text = "？？？";
        sizeText.text = "？？？";
        activityText.text = "？？？";
    }

    Sprite GetElementIcon(ElementType e)
    {
        switch (e)
        {
            case ElementType.Fire: return fireIcon;
            case ElementType.Water: return waterIcon;
            case ElementType.Wind: return windIcon;
            case ElementType.Earth: return earthIcon;
            case ElementType.Light: return lightIcon;
            case ElementType.Dark: return darkIcon;
            default: return unknownIcon;
        }
    }

    string FormatEnvironment(List<EnvironmentData> envs)
    {
        Dictionary<string, string> map = new Dictionary<string, string>();

        foreach (var e in envs)
        {
            map[e.name] = e.rank;
        }

        string Get(string key)
        {
            return map.ContainsKey(key) ? $"{key}:{map[key]}" : $"{key}:?";
        }

        string[] order = { "高温", "低温", "暗闇", "湿潤", "乾燥", "衝撃", "呪毒", "回復" };

        string line1 = $"{Get(order[0])}　{Get(order[1])}　{Get(order[2])}　{Get(order[3])}";
        string line2 = $"{Get(order[4])}　{Get(order[5])}　{Get(order[6])}　{Get(order[7])}";

        return line1 + "\n" + line2;
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}
