using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CommentManager : MonoBehaviour
{
    [Header("表示テキスト")]
    public Text mainCommentText;
    public Text subCommentText;

    [Header("閉じる対象")]
    public GameObject subCanvasRoot;

    [Header("タイプ速度")]
    public float typingSpeed = 0.05f;

    private Dictionary<string, string> commentDictionary;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullText;

    public static CommentManager Instance;

    void Awake()
    {
        Instance = this;

        commentDictionary = new Dictionary<string, string>()
        {
            { "傷薬", "討伐任務でへまこいてけがをしたんだ。\n傷薬はないかい？" },
            { "回復薬", "森で魔物にやられてしまった…。\n回復薬を頼む！" },
            { "ポーション", "長旅で疲れたよ。\nポーションをくれないか？" },
            { "軟膏", "火傷してしまったんだ。\n軟膏はあるか？" },
            { "解毒薬", "毒蛇に噛まれた！\n解毒薬を急いでくれ！" },
            { "リジェネ薬", "継続回復できる薬が欲しい。\nリジェネ薬はないか？" }
        };
    }

    public void ShowComment(string bottomWord)
    {
        if (!commentDictionary.ContainsKey(bottomWord))
            return;

        if (mainCommentText == null || subCommentText == null)
        {
            Debug.LogError("コメントTextがセットされていません");
            return;
        }

        currentFullText = commentDictionary[bottomWord];

        subCanvasRoot.SetActive(true);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(currentFullText));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;

        mainCommentText.text = "";
        subCommentText.text = "";

        foreach (char c in text)
        {
            mainCommentText.text += c;
            subCommentText.text += c;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    // 画面タップでスキップさせたい場合に呼ぶ
    public void SkipTyping()
    {
        if (!isTyping) return;

        StopCoroutine(typingCoroutine);

        mainCommentText.text = currentFullText;
        subCommentText.text = currentFullText;

        isTyping = false;
    }

    public void CloseSubCanvas()
    {
        subCanvasRoot.SetActive(false);
    }
}
