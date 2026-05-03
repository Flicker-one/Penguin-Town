using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BOSS污染建筑的核心逻辑（精简版）
/// 定时随机污染单个建筑，自动启动循环
/// </summary>
public class BossPolluteBuilding : MonoBehaviour
{
    public float polluteInterval = 5f;
    // 缓存MapGenerator实例
    public MapGenerator mapGenerator;
    // 污染循环计时器
    private Coroutine _polluteLoopCoroutine;

    private void Awake()
    {
        // 获取MapGenerator单例
        mapGenerator = MapGenerator.Instance;
        if (mapGenerator == null)
        {
            Debug.LogError("[BossPolluteBuilding] MapGenerator实例未找到！请检查场景中是否有该组件");
            enabled = false;
            return;
        }

        enabled = false;
        // 默认自动启动污染循环
        StartPolluteLoop();
    }

    /// <summary>
    /// 启动定时污染循环
    /// </summary>
    public void StartPolluteLoop()
    {
        if (mapGenerator == null) return;
        if (_polluteLoopCoroutine != null)
        {
            StopCoroutine(_polluteLoopCoroutine);
        }
        
        _polluteLoopCoroutine = StartCoroutine(PolluteBuildingLoop());
        Debug.Log("[BossPolluteBuilding] 已启动定时污染循环，间隔：" + polluteInterval + "秒");
    }

    /// <summary>
    /// 停止所有污染操作
    /// </summary>
    public void StopPolluteLoop()
    {
        if (_polluteLoopCoroutine != null)
        {
            StopCoroutine(_polluteLoopCoroutine);
            _polluteLoopCoroutine = null;
        }
        Debug.Log("[BossPolluteBuilding] 已停止定时污染循环");
    }

    /// <summary>
    /// 定时污染单个建筑的核心协程
    /// </summary>
    private IEnumerator PolluteBuildingLoop()
    {
        while (true)
        {
            // 检测游戏暂停状态，暂停时等待直到恢复
            while (PauseController.IsGamePaused)
            {
                yield return null; // 每一帧检查一次暂停状态
            }
            
            // 执行单次污染（默认随机选择目标）
            PolluteSingleBuilding();
            
            // 等待指定间隔（等待期间也检测暂停）
            float waitTimer = 0f;
            while (waitTimer < polluteInterval)
            {
                if (!PauseController.IsGamePaused)
                {
                    waitTimer += Time.deltaTime;
                }
                yield return null;
            }
        }
    }

    /// <summary>
    /// 随机污染单个建筑（核心逻辑）
    /// </summary>
    private void PolluteSingleBuilding()
    {
        // 获取所有建筑槽位
        List<int> allSlotIndices = new List<int>(mapGenerator.mapBuildingData.Keys);
        if (allSlotIndices.Count == 0)
        {
            Debug.LogWarning("[BossPolluteBuilding] 地图中无任何建筑");
            return;
        }

        // 筛选未被污染的建筑
        List<int> availableSlots = new List<int>();
        foreach (int slotIndex in allSlotIndices)
        {
            MapGenerator.BuildingData buildingData = mapGenerator.GetBuildingData(slotIndex);
            if (buildingData == null || buildingData.buildingObj == null)
                continue;

            if (buildingData.buildingState != BuildingState.Polluted)
                availableSlots.Add(slotIndex);
        }

        if (availableSlots.Count == 0)
        {
            Debug.LogWarning("[BossPolluteBuilding] 无未被污染的建筑");
            return;
        }

        // 随机选择目标槽位
        int targetSlotIndex = availableSlots[Random.Range(0, availableSlots.Count)];
        
        // 修改建筑状态为污染
        mapGenerator.ChangeBuildingState(targetSlotIndex, BuildingState.Polluted);
        
        // 日志输出
        MapGenerator.BuildingData targetData = mapGenerator.GetBuildingData(targetSlotIndex);
        Debug.Log($"[BossPolluteBuilding] 污染建筑成功 | 槽位：{targetSlotIndex} | 类型：{targetData.buildingType}");
    }

    /// <summary>
    /// 净化所有被污染建筑（保留基础净化功能）
    /// </summary>
    public void TriggerPurifyBuilding()
    {
        if (mapGenerator == null) return;

        // 筛选出污染状态的建筑
        List<int> pollutedSlots = new List<int>();
        foreach (var slotIndex in mapGenerator.mapBuildingData.Keys)
        {
            MapGenerator.BuildingData buildingData = mapGenerator.GetBuildingData(slotIndex);
            if (buildingData != null && buildingData.buildingState == BuildingState.Polluted)
            {
                pollutedSlots.Add(slotIndex);
            }
        }

        if (pollutedSlots.Count == 0)
        {
            Debug.LogWarning("[BossPolluteBuilding] 当前无被污染的建筑");
            return;
        }

        // 执行净化
        foreach (int slotIndex in pollutedSlots)
        {
            mapGenerator.ChangeBuildingState(slotIndex, BuildingState.Purified);
        }

        Debug.Log($"[BossPolluteBuilding] 净化建筑完成 | 数量：{pollutedSlots.Count}");
    }

    private void OnDestroy()
    {
        // 销毁时停止污染循环
        StopPolluteLoop();
    }
}
