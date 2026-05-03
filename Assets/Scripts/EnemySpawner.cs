using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject enemyPrefab;       // 你的敌人预制体
    public Transform player;             // 玩家
    public float spawnRange = 10f;       // 以玩家为中心的生成范围
    public float minSpawnDistance = 3f;  // 离玩家至少多远才生成
    public int maxEnemyCount = 10;       // 最大敌人数量

    [Header("阻挡层（不能生成的东西）")]
    public LayerMask buildingLayer;      // 建筑层 Collision
    public LayerMask bossLayer;          // BOSS层
    public LayerMask mapBorderLayer;     // 地图外围/边界层

    [Header("生成间隔")]
    public float spawnInterval = 2f;     // 每隔几秒生成一次
    private float spawnTimer;

    // 保存当前所有敌人
    private List<GameObject> currentEnemies = new List<GameObject>();

    void Awake()
    {
        enabled = false;
    }

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        if (player == null || enemyPrefab == null) return;

        if (!PauseController.IsGamePaused)
        {
            spawnTimer -= Time.deltaTime;
        }

        // 达到上限就不生成
        if (currentEnemies.Count >= maxEnemyCount)
            return;

        // 到时间就生成
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        // 最多尝试15次找安全位置（避免卡死）
        for (int i = 0; i < 15; i++)
        {
            // 1. 在玩家周围随机一个位置
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float randomDist = Random.Range(minSpawnDistance, spawnRange);
            Vector2 spawnPos = (Vector2)player.position + randomDir * randomDist;

            // 2. 判断这个位置能不能生成
            if (CanSpawnHere(spawnPos))
            {
                // 生成敌人
                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                currentEnemies.Add(enemy);

                // 敌人死亡时自动从列表移除
                EnemyVisionChaser enemyScript = enemy.GetComponent<EnemyVisionChaser>();
                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => currentEnemies.Remove(enemy);
                }
                break;
            }
        }
    }

    // 判断位置是否安全（无建筑、无BOSS、不在地图外）
    bool CanSpawnHere(Vector2 pos)
    {
        // 检测半径：根据你的敌人大小调整，0.5~0.8都可以
        float checkRadius = 0.6f;

        // 有建筑 / 有BOSS / 在地图外 → 不能生成
        bool hasBuilding = Physics2D.OverlapCircle(pos, checkRadius, buildingLayer);
        bool hasBoss = Physics2D.OverlapCircle(pos, checkRadius, bossLayer);
        bool outOfMap = Physics2D.OverlapCircle(pos, checkRadius, mapBorderLayer);

        return !hasBuilding && !hasBoss && !outOfMap;
    }

    // 编辑器里画范围方便调试
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, spawnRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minSpawnDistance);
    }
}