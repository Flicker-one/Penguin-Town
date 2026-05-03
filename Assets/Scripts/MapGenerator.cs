using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    [Header("Building Prefab")]
    public GameObject farmPrefab;
    public GameObject icecreamShopPrefab;
    public GameObject bankPrefab;
    public GameObject restaurantPrefab;
    public GameObject wizardHousePrefab;
    public GameObject furnacePrefab;
    public GameObject weaponMakerPrefab;
    public GameObject supermarketPrefab;
    public GameObject magicainPenguinPrefab;
    public GameObject postOfficePrefab;
    public GameObject VendingMachinePrefab;
    public GameObject PharmacyPrefab;

    [Header("coordinates")]
    public Vector2[] mapSlots = new Vector2[16]; 

    [Header("Player")]
    public GameObject playerPrefab;
    public Vector2 playerSpawnPos;

    [Header("Enemy")]
    public EnemySpawner enemySpawner;

    [Header("Boss")] 
    public BossPolluteBuilding bossPolluter;

    // 存储地图数据：槽位索引 → 建筑信息
    public Dictionary<int, BuildingData> mapBuildingData = new Dictionary<int, BuildingData>();

    // 建筑数据结构（记录类型、状态、ID、坐标）
    [System.Serializable]
    public class BuildingData
    {
        public int slotIndex;          // 所在槽位索引
        public BuildingType buildingType;
        public BuildingState buildingState;
        public string buildingID;
        public Vector2 position;
        public GameObject buildingObj; // 对应的游戏物体
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        if (mapSlots.Length == 0 || mapSlots[0] == Vector2.zero)
        {
            Debug.LogError("MapGenerator的mapSlots未配置坐标！请在Inspector中给16个槽位赋值");
            yield break;
        }
        yield return StartCoroutine(GenerateMap());
        SpawnPlayer();
        EnableEnemySpawner();
        EnableBossPolluter();
        Debug.Log("map initialized");
    }
    
    private IEnumerator GenerateMap()
    {
        List<BuildingType> buildingTypesToSpawn = new List<BuildingType>()
        {
            BuildingType.Farm, BuildingType.Farm, // 2个Farm
            BuildingType.IcecreamShop, BuildingType.IcecreamShop, // 2个Icecream Shop
            BuildingType.Bank, BuildingType.Bank, // 2个bank
            BuildingType.Restaurant, // 1个restaurant
            BuildingType.WizardHouse, // 1个WizardHouse
            BuildingType.Furnace, BuildingType.Furnace, // 2个Furnace
            BuildingType.WeaponMaker, // 1个WeaponMaker
            BuildingType.Supermarket, // 1个Supermarket
            BuildingType.MagicainPenguin, // 1个MagicainPenguin
            BuildingType.PostOffice, // 1个PostOffice
            BuildingType.Pharmacy,
            BuildingType.VendingMachine
        };

        // 打乱建筑类型列表
        ShuffleList(buildingTypesToSpawn);

        // 遍历槽位生成建筑
        for (int i = 0; i < mapSlots.Length; i++)
        {
            if (i >= buildingTypesToSpawn.Count) break;

            Vector2 slotPos = mapSlots[i];
            BuildingType currentType = buildingTypesToSpawn[i];
            GameObject buildingPrefab = GetBuildingPrefabByType(currentType);

            if (buildingPrefab == null)
            {
                Debug.LogWarning($"didnt found {currentType} prefab！");
                continue;
            }

            // 生成建筑
            GameObject buildingObj = Instantiate(buildingPrefab, slotPos, Quaternion.identity);
            BuildingBase buildingBase = buildingObj.GetComponent<BuildingBase>();
            
            if (buildingBase != null)
            {
                // 设置建筑类型和初始状态
                buildingBase.BuildingType = currentType;
                buildingBase.SetBuildingState(BuildingState.Normal);
                
                // 记录建筑数据
                BuildingData buildingData = new BuildingData()
                {
                    slotIndex = i,
                    buildingType = currentType,
                    buildingState = BuildingState.Normal,
                    buildingID = buildingBase.BuildingID,
                    position = slotPos,
                    buildingObj = buildingObj
                };
                
                // 添加到地图数据字典
                mapBuildingData.Add(i, buildingData);
                
                Debug.Log($"槽位{i}生成建筑：{currentType}，ID：{buildingBase.BuildingID}，状态：Normal");
            }
            else
            {
                Debug.LogWarning($"{currentType}未挂载BuildingBase.cs");
                Destroy(buildingObj);
            }

            yield return null; // 每帧生成一个，避免卡顿
        }
    }

    // 根据建筑类型获取对应的预制体
    private GameObject GetBuildingPrefabByType(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Farm: return farmPrefab;
            case BuildingType.IcecreamShop: return icecreamShopPrefab;
            case BuildingType.Bank: return bankPrefab;
            case BuildingType.Restaurant: return restaurantPrefab;
            case BuildingType.WizardHouse: return wizardHousePrefab;
            case BuildingType.Furnace: return furnacePrefab;
            case BuildingType.WeaponMaker: return weaponMakerPrefab;
            case BuildingType.Supermarket: return supermarketPrefab;
            case BuildingType.MagicainPenguin: return magicainPenguinPrefab;
            case BuildingType.PostOffice: return postOfficePrefab;
            case BuildingType.Pharmacy: return PharmacyPrefab;
            case BuildingType.VendingMachine: return VendingMachinePrefab;
            default: return null;
        }
    }

    // 生成玩家
    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("玩家预制体未配置！");
            return;
        }

        GameObject player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        // 如果EnemySpawner需要玩家引用，这里赋值
        if (enemySpawner != null && enemySpawner.player == null)
        {
            enemySpawner.player = player.transform;
        }
        
        Debug.Log($"玩家生成在坐标：{playerSpawnPos}");
    }

    // 启用刷怪功能
    private void EnableEnemySpawner()
    {
        if (enemySpawner == null)
        {
            Debug.LogWarning("EnemySpawner未配置！");
            return;
        }

        enemySpawner.enabled = true;
        Debug.Log("Enemy spawner enabled");
    }

    private void EnableBossPolluter()
    {
        if (bossPolluter == null)
        {
            Debug.LogWarning("BossPolluteBuilding 未配置！");
            return;
        }

        bossPolluter.enabled = true;
        Debug.Log("Boss activated");
    }
    // 打乱列表的工具方法
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // 修改指定槽位建筑的状态
    public void ChangeBuildingState(int slotIndex, BuildingState newState)
    {
        if (mapBuildingData.ContainsKey(slotIndex))
        {
            BuildingData data = mapBuildingData[slotIndex];
            data.buildingState = newState;
            data.buildingObj.GetComponent<BuildingBase>()?.SetBuildingState(newState);
            mapBuildingData[slotIndex] = data;
            Debug.Log($"{slotIndex} BuildingState changed to：{newState}");
        }
        else
        {
            Debug.LogWarning($"槽位{slotIndex}不存在建筑！");
        }
    }

    // 获取指定槽位的建筑信息
    public BuildingData GetBuildingData(int slotIndex)
    {
        if (mapBuildingData.ContainsKey(slotIndex))
        {
            return mapBuildingData[slotIndex];
        }
        return null;
    }

    public void ChangeBuildingStateByID(string buildingID, BuildingState newState)
    {
        // 校验参数合法性
        if (string.IsNullOrEmpty(buildingID))
        {
            Debug.LogWarning("BuildingID 不能为空！");
            return;
        }

        // 遍历字典查找匹配的BuildingID
        foreach (var kvp in mapBuildingData)
        {
            BuildingData data = kvp.Value;
            if (data.buildingID == buildingID)
            {
                // 更新建筑数据中的状态
                data.buildingState = newState;

                // 更新建筑游戏物体上的状态（通过BuildingBase）
                // BuildingBase buildingBase = data.buildingObj.GetComponent<BuildingBase>();
                // if (buildingBase != null)
                // {
                //     buildingBase.SetBuildingState(newState);
                //     Debug.Log($"BuildingID {buildingID}（槽位{kvp.Key}）状态已更新为：{newState}");
                // }
                // else
                // {
                //     Debug.LogWarning($"BuildingID {buildingID} 对应的物体未挂载BuildingBase组件！");
                // }

                // 同步更新字典中的数据（值类型需重新赋值，引用类型可省略，但这里保持显式更新）
                mapBuildingData[kvp.Key] = data;
                return; // 找到后立即退出循环，提升效率
            }
        }
    }
}
