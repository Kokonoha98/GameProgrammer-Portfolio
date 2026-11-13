using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

public class FlagSpawner : MonoBehaviour
{
    [Header("スポーン地点")]
    public Transform[] spawnPoints; // 旗のスポーン地点を設定

    [Header("旗プレハブ")]
    public AssetReferenceGameObject flagPrefab; // 旗のプレハブを設定

    [Header("最低距離")]
    public float minDistance = 10f; // プレイヤーからの最低距離を設定

    private int lastSpawnIndex = -1; // 前回のスポーン地点を記録

    private GameObject currentFlag; // 現在の旗を記録

    private bool isSpawning = false; // 旗を生成中かどうか

    private void Start()
    {
        SpawnFlag();
    }

    // 旗をスポーン
    public void SpawnFlag()
    {
        // すでに旗を生成中なら何もしない
        if (isSpawning)
        {
            return;
        }

        if (currentFlag != null)
        {
            Addressables.ReleaseInstance(currentFlag); // 現在の旗を削除
            currentFlag = null;
        }

        List<int> candidates = new(); // 条件を満たすスポーン地点を保存

        Transform player = PlayerManager.Instance.PlayerTransform; // プレイヤーのTransformを取得

        // スポーン地点を順番に確認
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // 前回と同じ場所を除外
            if (i == lastSpawnIndex)
                continue;

            // プレイヤーとの距離を計算
            float distance = Vector3.Distance(
                player.position,
                spawnPoints[i].position
            );

            // 最低距離を満たしているか確認
            if (distance >= minDistance)
            {
                // スポーン候補に追加
                candidates.Add(i);
            }
        }

        // スポーン候補があるか確認
        if (candidates.Count == 0)
        {
            // 警告を表示
            Debug.LogWarning("条件を満たすスポーン地点がありません");
            return;
        }

        int selectedIndex =
            candidates[Random.Range(0, candidates.Count)]; // スポーン地点をランダムに選択

        lastSpawnIndex = selectedIndex; // 今回のスポーン地点を記録

        // 旗の生成開始
        isSpawning = true;

        // Addressableから旗を生成
        flagPrefab.InstantiateAsync(
            spawnPoints[selectedIndex].position,
            spawnPoints[selectedIndex].rotation
        ).Completed += handle =>
        {
            isSpawning = false;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                currentFlag = handle.Result;
            }
            else
            {
                Debug.LogError("旗の読み込みに失敗しました");
            }
        };
    }
    private void OnDestroy()
    {
        // Addressablesで生成した旗を解放
        if (currentFlag != null)
        {
            Addressables.ReleaseInstance(currentFlag);
            currentFlag = null;
        }
    }
}
