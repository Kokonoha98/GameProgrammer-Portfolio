using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPager : MonoBehaviour
{
    [Header("ページ")]
    public GameObject[] pages;

    [Header("矢印ボタン")]
    public Button leftButton;
    public Button rightButton;

    [Header("ページ番号テキスト")]
    public TextMeshProUGUI pageText;

    private int currentPage = 0;

    void Start()
    {
        ShowPage(currentPage);
    }

    // 次のページ
    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    // 前のページ
    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    // ページ更新
    void ShowPage(int pageIndex)
    {
        // 全ページ非表示
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }

        // 対象ページ表示
        pages[pageIndex].SetActive(true);

        // ボタン有効/無効
        leftButton.interactable = (pageIndex != 0);
        rightButton.interactable = (pageIndex != pages.Length - 1);

        // ページ番号表示
        pageText.text = (pageIndex + 1) + " / " + pages.Length;
    }
}
