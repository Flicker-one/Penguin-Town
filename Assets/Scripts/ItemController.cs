using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public static ItemController Instance { get; private set; }
    
    private ItemDictionary itemDictionary;
    
    public GameObject itemPanel;

    public GameObject slotPrefab;

    public GameObject[] itemPrefabs;

    public int slotCount;

    public List<Slot> allSlots = new List<Slot>();

    public List<Slot> Allslots => new List<Slot>(allSlots);

    public void Awake()
    {
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
    
    // Start is called before the first frame update
    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        allSlots.Clear();
        InitItemSlots();
    }

    private void InitItemSlots()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, itemPanel.transform).GetComponent<Slot>();
            allSlots.Add(slot);
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
            }
        }
    }

    public bool AddItem(GameObject itemPrefab)
    {
        foreach (Slot slot in allSlots)
        {
            if (slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                return true;
            }
        }
        Debug.Log("Item is full");
        return false;
    }

    public GameObject GetItemInSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < allSlots.Count)
        {
            return allSlots[slotIndex].currentItem;
        }
        Debug.Log("Slot index out of range");
        return null;
    }

    public void ModifySlotCount(int delta)
    {
        // 1. 校验关键引用
        if (itemPanel == null || slotPrefab == null)
        {
            Debug.LogError("itemPanel 或 slotPrefab 未赋值，无法修改格子数量！");
            return;
        }
        // 2. 跳过无效delta（0）
        if (delta == 0)
        {
            Debug.Log("格子数量增量为0，无需修改");
            return;
        }
        // 3. 计算新的格子数并限制上下限
        int newSlotCount = slotCount + delta;
        newSlotCount = Mathf.Min(newSlotCount, 10);
        // 4. 若新数量与原数量一致，跳过
        if (newSlotCount == slotCount)
        {
            Debug.Log($"格子数量已达{(delta > 0 ? "上限" : "下限")}，当前数量：{slotCount}");
            return;
        }
        // 5. 处理【新增格子】（delta>0）
        if (delta > 0)
        {
            int actualAddCount = newSlotCount - slotCount; // 实际需要新增的数量（避免超上限）
            for (int i = 0; i < actualAddCount; i++)
            {
                // 新格子的全局索引 = 原格子数 + 本次新增的索引
                int globalSlotIndex = slotCount + i;
                Slot newSlot = Instantiate(slotPrefab, itemPanel.transform).GetComponent<Slot>();
                allSlots.Add(newSlot);

                // 初始物品初始化：按全局索引（和Start逻辑统一）
                if (globalSlotIndex < itemPrefabs.Length)
                {
                    GameObject item = Instantiate(itemPrefabs[globalSlotIndex], newSlot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    newSlot.currentItem = item;
                }
            }
        }
        // 6. 处理【减少格子】（delta<0）
        else
        {
            int actualRemoveCount = slotCount - newSlotCount; // 实际需要移除的数量
            // 从列表末尾移除（避免打乱前面的格子顺序）
            for (int i = 0; i < actualRemoveCount; i++)
            {
                int lastIndex = allSlots.Count - 1;
                Slot removeSlot = allSlots[lastIndex];
                // 销毁格子对象（避免内存泄漏）
                Destroy(removeSlot.gameObject);
                // 从列表移除
                allSlots.RemoveAt(lastIndex);
            }
        }

        // 7. 更新最终的slotCount
        slotCount = newSlotCount;
        Debug.Log($"格子数量修改完成，当前数量：{slotCount}");
    }

    public bool CheckItemInInventory(GameObject itemPrefab)
    {
        if (itemPrefab == null) { Debug.LogError("材料预制体为空"); return false; }
        if (ItemDictionary.Instance == null) { Debug.LogError("无字典"); return false; }

        int targetID = ItemDictionary.Instance.GetItemID(itemPrefab);
        Debug.Log($"【合成检查】目标物品ID = " + targetID + " 名称：" + itemPrefab.name);

        foreach (Slot slot in allSlots)
        {
            if (slot.currentItem != null)
            {
                int slotID = ItemDictionary.Instance.GetItemID(slot.currentItem);
                Debug.Log("背包里物品ID = " + slotID + " 名称：" + slot.currentItem.name);

                if (slotID == targetID)
                {
                    Debug.Log("<color=green>找到匹配材料！</color>");
                    return true;
                }
            }
            else
            {
                Debug.Log("格子是空的");
            }
        }

        Debug.Log("<color=red>没找到材料！</color>");
        return false;
    }

    public bool RemoveItemFromInventory(GameObject itemPrefab)
    {
        if (itemPrefab == null) return false;
        if (ItemDictionary.Instance == null)
        {
            Debug.LogError("ItemDictionary单例未初始化！");
            return false;
        }

        // 获取目标物品的ID（预制体的ID）
        int targetItemID = ItemDictionary.Instance.GetItemID(itemPrefab);
        if (targetItemID == -1) return false;

        // 遍历物品栏，通过ID匹配并移除
        foreach (Slot slot in allSlots)
        {
            if (slot.currentItem != null)
            {
                int slotItemID = ItemDictionary.Instance.GetItemID(slot.currentItem);
                if (slotItemID == targetItemID)
                {
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                    return true; // 移除成功
                }
            }
        }
        return false;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
