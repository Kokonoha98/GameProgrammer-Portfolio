using UnityEngine;

public class HowToPlayManager : MonoBehaviour
{
    [Header("チュートリアルブック")]
    public GameObject book1;
    public GameObject book2;

    void Start()
    {
        // 最初はbook1だけ表示
        ShowBook1();
    }

    // book1だけ表示
    public void ShowBook1()
    {
        if (book1 != null) book1.SetActive(true);
        if (book2 != null) book2.SetActive(false);
    }

    // book2だけ表示
    public void ShowBook2()
    {
        if (book1 != null) book1.SetActive(false);
        if (book2 != null) book2.SetActive(true);
    }
}
