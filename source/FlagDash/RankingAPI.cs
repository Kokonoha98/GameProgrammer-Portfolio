using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RankingAPI : MonoBehaviour
{
    // スコア保存PHP
    private string saveScoreURL = "http://localhost/ranking/save_score.php";

    // ランキング取得PHP
    private string getRankingURL = "http://localhost/ranking/get_ranking.php";

    // スコアを送信
    public IEnumerator SaveScore(string name, int score)
    {
        WWWForm form = new WWWForm();

        // PHPへ送るデータ
        form.AddField("name", name);
        form.AddField("score", score);

        // PHPへPOST送信
        using (UnityWebRequest request =
            UnityWebRequest.Post(saveScoreURL, form))
        {
            yield return request.SendWebRequest();

            // 通信結果を確認
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("PHP応答：" + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("通信エラー：" + request.error);
            }
        }
    }

    // ランキングを取得
    public IEnumerator GetRanking(
    System.Action<RankingData[]> onComplete)
    {
        using (UnityWebRequest request =
            UnityWebRequest.Get(getRankingURL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;

                Debug.Log("ランキングJSON：" + json);

                // JSONをRankingDataの配列に変換
                RankingData[] ranking =
                    JsonHelper.FromJson<RankingData>(json);

                // RankingUIへランキングを渡す
                onComplete?.Invoke(ranking);
            }
            else
            {
                Debug.LogError(
                    "ランキング取得エラー：" + request.error
                );
            }
        }
    }

    public IEnumerator CheckScore(
        int score,
        System.Action<bool> onComplete)
    {
        WWWForm form = new WWWForm();

        // PHPへスコアを送信
        form.AddField("score", score);

        using (UnityWebRequest request =
            UnityWebRequest.Post(
                "http://localhost/ranking/check_score.php",
                form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response =
                    request.downloadHandler.text.Trim();

                Debug.Log("ランキング判定：" + response);

                bool isHighScore =
                    response == "true";

                onComplete?.Invoke(isHighScore);
            }
            else
            {
                Debug.LogError(
                    "ランキング判定エラー：" + request.error
                );
            }
        }
    }
}

[System.Serializable]
public class RankingData
{
    public string name;
    public float score;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrappedJson = "{\"items\":" + json + "}";

        Wrapper<T> wrapper =
            JsonUtility.FromJson<Wrapper<T>>(wrappedJson);

        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}