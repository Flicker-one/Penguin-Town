using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Playing,    
    Win,        
    Lose        
}

public class GameWinLoseManager : MonoBehaviour
{
    public static GameWinLoseManager Instance { get; private set; }

    public GameState CurrentGameState { get; private set; } = GameState.Playing;

    // 输赢回调
    public event System.Action OnGameWin;
    public event System.Action OnGameLose;

    private void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 启动游戏状态检测
        StartCoroutine(GameStateCheckCoroutine());
    }

    // 每帧检测游戏状态（协程避免卡顿）
    private IEnumerator GameStateCheckCoroutine()
    {
        yield return new WaitForSeconds(1f);
        while (CurrentGameState == GameState.Playing)
        {
            // 1. 检测玩家血量是否为0
            if (CheckPlayerHealthZero())
            {
                SetGameState(GameState.Lose);
                yield break;
            }

            // 2. 检测是否有整行/整列建筑全被净化（胜利）
            if (CheckAnyRowOrColumnPurified())
            {
                SetGameState(GameState.Win);
                yield break;
            }

            // 3. 检测是否有整行/整列建筑全被污染（失败）
            if (CheckAnyRowOrColumnPolluted())
            {
                SetGameState(GameState.Lose);
                yield break;
            }

            yield return null;
        }
    }

    // 检测玩家血量是否归0
    private bool CheckPlayerHealthZero()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("PlayerStatus Instance 未初始化！");
            return false;
        }

        return Mathf.Approximately(PlayerStatus.Instance.CurrentHealth, 0f);
    }

    // 检测是否有任意一行/一列的建筑全为Purified
    private bool CheckAnyRowOrColumnPurified()
    {
        return CheckAnyRowOrColumnState(BuildingState.Purified);
    }

    // 检测是否有任意一行/一列的建筑全为Polluted
    private bool CheckAnyRowOrColumnPolluted()
    {
        return CheckAnyRowOrColumnState(BuildingState.Polluted);
    }

    // 通用检测：是否有任意一行/一列的建筑全为指定状态
    private bool CheckAnyRowOrColumnState(BuildingState targetState)
    {
        if (MapGenerator.Instance == null)
        {
            Debug.LogWarning("MapGenerator Instance 未初始化！");
            return false;
        }

        // 4x4地图的行列定义
        int rows = 4;
        int cols = 4;

        // 1. 检测每一行
        for (int row = 0; row < rows; row++)
        {
            bool isRowAllTargetState = true;
            for (int col = 0; col < cols; col++)
            {
                int slotIndex = row * cols + col; // 计算槽位索引（行优先）
                if (!IsSlotInTargetState(slotIndex, targetState))
                {
                    isRowAllTargetState = false;
                    break;
                }
            }
            if (isRowAllTargetState)
            {
                Debug.Log($"第{row + 1}行所有建筑状态为{targetState}！");
                return true;
            }
        }

        // 2. 检测每一列
        for (int col = 0; col < cols; col++)
        {
            bool isColAllTargetState = true;
            for (int row = 0; row < rows; row++)
            {
                int slotIndex = row * cols + col; // 计算槽位索引（行优先）
                if (!IsSlotInTargetState(slotIndex, targetState))
                {
                    isColAllTargetState = false;
                    break;
                }
            }
            if (isColAllTargetState)
            {
                Debug.Log($"第{col + 1}列所有建筑状态为{targetState}！");
                return true;
            }
        }

        return false;
    }

    // 检测指定槽位的建筑是否为目标状态
    private bool IsSlotInTargetState(int slotIndex, BuildingState targetState)
    {
        // 检查槽位索引是否有效
        if (slotIndex < 0 || slotIndex >= 16)
        {
            Debug.LogWarning($"无效的槽位索引：{slotIndex}");
            return false;
        }

        // 检查该槽位是否有建筑数据
        if (!MapGenerator.Instance.mapBuildingData.ContainsKey(slotIndex))
        {
            Debug.LogWarning($"槽位{slotIndex}没有对应的建筑数据！");
            return false;
        }

        // 获取建筑状态并对比
        MapGenerator.BuildingData buildingData = MapGenerator.Instance.mapBuildingData[slotIndex];
        return buildingData.buildingState == targetState;
    }

    // 设置游戏状态并触发回调
    private void SetGameState(GameState newState)
    {
        if (CurrentGameState != GameState.Playing) return;

        CurrentGameState = newState;
        Debug.Log($"游戏状态变更：{newState}");

        // 停止所有游戏相关逻辑（可选）
        StopAllCoroutines();
        DisableEnemySpawner();
        DisableBossPolluter();

        // 触发输赢回调
        if (newState == GameState.Win)
        {
            OnGameWin?.Invoke();
            Debug.Log("游戏胜利！");
        }
        else if (newState == GameState.Lose)
        {
            OnGameLose?.Invoke();
            Debug.Log("游戏失败！");
        }
        Time.timeScale = 0;
        StartCoroutine(ExitAfterDelay(3f));
    }
    
    private IEnumerator ExitAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // 不受 timeScale 影响
        Application.Quit();
    }

    // 禁用敌人生成器
    private void DisableEnemySpawner()
    {
        if (MapGenerator.Instance?.enemySpawner != null)
        {
            MapGenerator.Instance.enemySpawner.enabled = false;
        }
    }

    // 禁用Boss
    private void DisableBossPolluter()
    {
        if (MapGenerator.Instance?.bossPolluter != null)
        {
            MapGenerator.Instance.bossPolluter.enabled = false;
        }
    }

    // 重置游戏状态
    public void ResetGameState()
    {
        CurrentGameState = GameState.Playing;
        StartCoroutine(GameStateCheckCoroutine());
        
        // 可选：恢复敌人和Boss
        if (MapGenerator.Instance?.enemySpawner != null)
        {
            MapGenerator.Instance.enemySpawner.enabled = true;
        }
        if (MapGenerator.Instance?.bossPolluter != null)
        {
            MapGenerator.Instance.bossPolluter.enabled = true;
        }
        
        Debug.Log("游戏状态已重置，重新开始检测");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // 按T手动触发检测
        {
            bool isWin = CheckAnyRowOrColumnPurified();
            bool isLose = CheckAnyRowOrColumnPolluted();
            Debug.Log($"手动检测：胜利={isWin}，失败={isLose}");
        }
    }
}