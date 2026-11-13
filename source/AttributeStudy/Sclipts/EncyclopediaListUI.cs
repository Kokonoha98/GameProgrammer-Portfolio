using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EncyclopediaListUI : MonoBehaviour
{
    [Header("UIスロット（8個）")]
    public List<EncyclopediaButtonUI> slots;

    private List<CreatureData> filteredList = new List<CreatureData>();

    private int currentPage = 0;
    private int maxPage;
    private int pageSize = 8;

    public Text pageText;
    public Button nextButton;
    public Button prevButton;

    private CategoryType currentCategory;

    void Start()
    {
        ShowCategory(CategoryType.Stone); // 初期表示
    }

    void OnEnable()
    {
        RefreshPage(); // ★毎回更新
    }

    // 🔽 カテゴリ切り替え本体
    public void ShowCategory(CategoryType category)
    {
        currentCategory = category;
        currentPage = 0;

        filteredList = EncyclopediaManager.Instance
        .GetAllCreatures()
        .FindAll(c => c.category == category);

        RefreshPage();
    }

    // 🔘 ラッパー関数（ボタン用）
    public void ShowStone() { ShowCategory(CategoryType.Stone); }
    public void ShowLiquid() { ShowCategory(CategoryType.Liquid); }
    public void ShowPlant() { ShowCategory(CategoryType.Plant); }
    public void ShowInsect() { ShowCategory(CategoryType.Insect); }
    public void ShowFish() { ShowCategory(CategoryType.Fish); }
    public void ShowBird() { ShowCategory(CategoryType.Bird); }
    public void ShowMammal() { ShowCategory(CategoryType.Mammal); }
    public void ShowReptile() { ShowCategory(CategoryType.Reptile); }

    // ▶ 次のページ
    public void NextPage()
    {
        int maxPage = Mathf.CeilToInt((float)filteredList.Count / pageSize) - 1;

        if (currentPage < maxPage)
        {
            currentPage++;
            RefreshPage();
        }
    }

    // ◀ 前のページ
    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            RefreshPage();
        }
    }

    //  ページ更新
    void RefreshPage()
    {
        int totalCount = filteredList.Count;
        maxPage = Mathf.Max(0, Mathf.CeilToInt((float)totalCount / pageSize) - 1);

        // ページ表示更新（◯/◯）
        pageText.text = $"{currentPage + 1} / {maxPage + 1}";

        // ボタン制御
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < maxPage;

        // スロット更新
        for (int i = 0; i < slots.Count; i++)
        {
            int dataIndex = currentPage * pageSize + i;

            if (dataIndex < totalCount)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].Setup(filteredList[dataIndex]);
            }
            else
            {
                slots[i].gameObject.SetActive(false); // ←ここ重要（空は消す）
            }
        }
    }
}
