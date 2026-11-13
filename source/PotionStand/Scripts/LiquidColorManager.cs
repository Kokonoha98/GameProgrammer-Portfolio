using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LiquidColorManager : MonoBehaviour
{
    [Header("ビンの中の液体")]
    public Image liquidImage;
    public Image liquidImage2;

    // ランダムで使う色リスト
    private List<Color> colorList = new List<Color>()
    {
        Color.red,                                      // 赤
        Color.blue,                                     // 青
        Color.yellow,                                   // 黄色
        Color.white,                                    // 白
        new Color(0f, 1f, 1f),                          // 水色
        new Color(0.5f, 0f, 0.5f),                      // 紫
        new Color(1f, 0.5f, 0f),                        // オレンジ
        new Color(0f, 1f, 0f),                          // 緑
        new Color(0.5f, 1f, 0f),                        // 黄緑
        new Color(0.7f, 0f, 0.5f),                      // 赤紫
        new Color(0.3f, 0f, 0.7f),                      // 青紫
        new Color(0.8f, 0.8f, 0.8f),                    // 薄灰色
        new Color(1f, 0.5f, 0.7f),                      // ピンク
        new Color(0f, 0.8f, 0.8f)                       // 青緑
    };

    // レシピ生成後に呼ぶ
    public void SetRandomLiquidColor(List<RecipeStep> recipe)
    {
        if (liquidImage == null && liquidImage2 == null) return;

        Color randomColor = colorList[Random.Range(0, colorList.Count)];

        if (liquidImage != null)
            liquidImage.color = randomColor;

        if (liquidImage2 != null)
            liquidImage2.color = randomColor;
    }
}
