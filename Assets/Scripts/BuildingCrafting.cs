using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingCrafting : BuildingBase, IInteractable
{
    public bool IsInteracted { get; private set; }
    // public string BuildingID { get; private set; }
    public GameObject craftingPanel; 

    [System.Serializable]
    public class CraftRecipe
    {
        public GameObject craftResult;
        public List<GameObject> requiredItems;
    }

    public List<CraftRecipe> recipes;
    
    public float dropDownDistance = 5f;
    public Sprite defaultItemSprite; // 默认物品占位图
    [Header("Sound Effect")] 
    public AudioClip createMenuSFX;
    public AudioClip SelectedSFX;
    public AudioClip completedSFX;
    public AudioClip failedSFX;
    
    // private SpriteRenderer sr;
    private GameObject currentCraftingPanel;
    private Image selectedGoalImage; // 当前选中的目标物品图片
    private Transform requiredItemsGrid; // 所需物品的Grid容器
    private Button craftButton; 
    private int selectedGoalIndex = -1; // 当前选中的目标物品索引

    void Start()
    {
        // BuildingID ??= GlobalHelper.GenerateUniqueID(gameObject);
        // sr = GetComponent<SpriteRenderer>();
        UpdateStateVisual();
        
        // initialize crafting panel
        if (craftingPanel != null)
        {
            Canvas uiCanvas = GameObject.Find("UI").GetComponent<Canvas>();
            currentCraftingPanel = Instantiate(craftingPanel, uiCanvas.transform);
            currentCraftingPanel.SetActive(false);
            InitPanelComponents();
        }
    }
    
    private void InitPanelComponents()
    {
        if (currentCraftingPanel == null) return;
        
        Transform goalItemsGrid = currentCraftingPanel.transform.Find("GoalItemsGrid");
        requiredItemsGrid = currentCraftingPanel.transform.Find("RequiredItemsGrid");
        craftButton = currentCraftingPanel.transform.Find("CraftButton").GetComponent<Button>();
        Button closeButton = currentCraftingPanel.transform.Find("CloseButton").GetComponent<Button>();
        
        // 初始化目标物品展示
        if (goalItemsGrid != null)
        {
            for (int i = 0; i < recipes.Count; i++)
            {
                int index = i;
                GameObject goalItem = recipes[i].craftResult;

                GameObject imgObj = new GameObject($"GoalItem_{i}");
                imgObj.transform.SetParent(goalItemsGrid);
                Image img = imgObj.AddComponent<Image>();

                Sprite itemSprite = goalItem.GetComponent<SpriteRenderer>()?.sprite ?? defaultItemSprite;
                img.sprite = itemSprite;
                img.preserveAspect = true;

                EventTrigger trigger = imgObj.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((data) => { OnGoalItemClick(index, img); });
                trigger.triggers.Add(entry);
            }
        }
        
        craftButton.onClick.AddListener(OnCraftButtonClick);
        craftButton.interactable = false; // 初始不可点击（未选中物品）

        // 关闭按钮点击事件
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseCraftingPanel);
        }
    }
    
    public void Interact()
    {
        if (!CanInteract()) return;
        ShowCraftingPanel();
    }
    
    public bool CanInteract()
    {
        return currentCraftingPanel == null || !currentCraftingPanel.activeSelf;
    }
    
    /// 显示合成面板
    private void ShowCraftingPanel()
    {
        AudioSource.PlayClipAtPoint(createMenuSFX, transform.position);
        if (currentCraftingPanel != null)
        {
            currentCraftingPanel.SetActive(true);
            PauseController.SetPause(true);
        }
    }
    
    /// 关闭合成面板
    private void CloseCraftingPanel()
    {
        if (currentCraftingPanel != null)
        {
            currentCraftingPanel.SetActive(false);
            PauseController.SetPause(false);
            
            // 重置选中状态
            selectedGoalIndex = -1;
            selectedGoalImage = null;
            ClearRequiredItemsGrid();
            craftButton.interactable = false;
        }
    }
    
    private void OnGoalItemClick(int index, Image img)
    {
        // 取消之前选中物品的高亮
        if (selectedGoalImage != null)
        {
            selectedGoalImage.color = Color.white;
        }
        
        // 设置新选中物品
        selectedGoalIndex = index;
        selectedGoalImage = img;
        selectedGoalImage.color = Color.yellow; // 高亮选中的物品
        AudioSource.PlayClipAtPoint(SelectedSFX, transform.position);
        
        // 显示所需物品
        ShowRequiredItems(index);
        
        // 启用合成按钮
        craftButton.interactable = true;
    }

    /// 显示选中目标物品所需的材料
    private void ShowRequiredItems(int goalIndex)
    {
        // 清空之前的所需物品展示
        ClearRequiredItemsGrid();

        if (goalIndex < 0 || goalIndex >= recipes.Count) return;

        // 获取当前目标物品所需的材料
        var requiredItems = recipes[goalIndex].requiredItems;

        foreach (var reqItem in requiredItems)
        {
            if (reqItem == null) continue;

            GameObject imgObj = new GameObject("RequiredItem");
            imgObj.transform.SetParent(requiredItemsGrid, false);
            
            RectTransform rt = imgObj.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(60, 60); 
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            Image img = imgObj.AddComponent<Image>();
            Sprite sprite = reqItem.GetComponent<SpriteRenderer>()?.sprite ?? defaultItemSprite;
            img.sprite = sprite;
            img.preserveAspect = true;
        }
    }

    // 清空所需物品展示区域
    private void ClearRequiredItemsGrid()
    {
        if (requiredItemsGrid == null) return;
        
        foreach (Transform child in requiredItemsGrid)
        {
            Destroy(child.gameObject);
        }
    }
    
    // 点击合成按钮后的逻辑
    private void OnCraftButtonClick()
    {
        if (selectedGoalIndex < 0 || selectedGoalIndex >= recipes.Count) return;
        var recipe = recipes[selectedGoalIndex];
        GameObject goalItem = recipe.craftResult;
        var requiredItems = recipe.requiredItems;

        // 检查所有材料是否都有
        bool canCraft = true;
        foreach (var req in requiredItems)
        {
            if (!ItemController.Instance.CheckItemInInventory(req))
            {
                canCraft = false;
                break;
            }
        }

        if (canCraft)
        {
            // 扣除所有材料
            foreach (var req in requiredItems)
            {
                ItemController.Instance.RemoveItemFromInventory(req);
            }

            SpawnItemCrafted(goalItem);
            // ShowInteractedVisual();
            SetBuildingState(BuildingState.Purified);
            Debug.Log($"净化建筑完成");
            AudioSource.PlayClipAtPoint(completedSFX, transform.position);
            CloseCraftingPanel();
        }
        else
        {
            StartCoroutine(ShowCraftFailedTip());
        }
    }
    
    private void SpawnItemCrafted(GameObject goalItem)
    {
        if (goalItem == null) return;
        Vector2 dropPos = (Vector2)transform.position + Vector2.down * dropDownDistance;
        Instantiate(goalItem, dropPos, Quaternion.identity);
    }
    
    // private void ShowInteractedVisual()
    // {
    //     if (sr != null)
    //     {
    //         sr.color = Color.green;
    //     }
    // }
    
    private IEnumerator ShowCraftFailedTip()
    {
        Debug.LogWarning("合成失败：物品栏材料不足！");
        AudioSource.PlayClipAtPoint(failedSFX, transform.position);
        // 可替换为UI文本提示
        yield return new WaitForSeconds(2f);
        // 清空提示（如果是UI文本需在这里处理）
    }
    
    /// 监听ESC键关闭面板
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentCraftingPanel != null && currentCraftingPanel.activeSelf)
        {
            CloseCraftingPanel();
        }
    }
    
    /// 销毁时清理面板
    private void OnDestroy()
    {
        if (currentCraftingPanel != null)
        {
            Destroy(currentCraftingPanel);
        }
    }
}
