using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    // 新增单例实例
    public static ItemDictionary Instance { get; private set; }

    public List<Item> itemPrefabs;

    private Dictionary<int, GameObject> itemIDToPrefab;
    private Dictionary<GameObject, int> itemToID = new Dictionary<GameObject, int>();

    private void Awake()
    {
        // 单例初始化（确保全局唯一）
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 可选：跨场景保留
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 原有ID映射逻辑保留，补充空值校验
        itemIDToPrefab = new Dictionary<int, GameObject>();
        itemToID.Clear();

        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if (itemPrefabs[i] == null) continue;

            Item itemComp = itemPrefabs[i].GetComponent<Item>();
            if (itemComp == null)
            {
                Debug.LogWarning($"预制体 {itemPrefabs[i].name} 没有Item组件！");
                continue;
            }

            // 强制用预制体组件里的ID作为字典ID，避免不一致
            int id = itemComp.ID;
            itemIDToPrefab[id] = itemPrefabs[i].gameObject;
            itemToID[itemPrefabs[i].gameObject] = id;
        }
    }

    // 优化GetItemID：兼容实例化物品（带Clone）和预制体
    public int GetItemID(GameObject item)
    {
        if (item == null)
        {
            Debug.LogError("传入的物品对象为空！");
            return -1;
        }

        // // 步骤1：获取物品对应的原始预制体（解决Clone后缀问题）
        // GameObject originalPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(item);
        //
        // // 步骤2：优先从预制体映射中找ID
        // if (originalPrefab != null && itemToID.ContainsKey(originalPrefab))
        // {
        //     Debug.Log($"从映射找到ID"+ itemToID[originalPrefab]);
        //     return itemToID[originalPrefab];
        // }
        
        // 步骤3：兼容直接传入预制体的情况
        if (itemToID.ContainsKey(item))
        {
            Debug.Log($"直接传入预制体找到ID"+ itemToID[item]);
            return itemToID[item];
        }

        // 步骤4：最后尝试从Item组件直接取ID（兜底逻辑）
        Item itemComponent = item.GetComponent<Item>();
        if (itemComponent != null)
        {
            Debug.Log($"从item组件找到ID"+ itemComponent.ID);
            return itemComponent.ID;
        }

        Debug.LogError($"物品 {item.name} 未在ItemDictionary中注册，且无Item组件！");
        return -1;
    }

    public GameObject GetItemPrefab(int itemID)
    {
        if (itemIDToPrefab.TryGetValue(itemID, out GameObject prefab))
        {
            return prefab;
        }
        Debug.LogWarning($"ID为{itemID}的物品未在字典中找到！");
        return null;
    }

    void Start() { }
    void Update() { }
}
