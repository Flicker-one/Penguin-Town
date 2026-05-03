using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class PlayerStatusController : MonoBehaviour
{
    public GameObject playerPanel;
    
    public GameObject statusTextPrefab;
    public TextMeshProUGUI hpText;
    

    public float playerHealth = 100f;
    public float moveSpeed = 5f;
    public float skillCDRate = 1f;
    
    private readonly Dictionary<string, TextMeshProUGUI> statusTextDict = new Dictionary<string, TextMeshProUGUI>();
    
    void Start()
    {
        InitPlayerStatusPanel();
        if (hpText == null)
        {
            Debug.LogError("hpText not created");
        }
        UpdateHealthUI();
    }

    /// <summary>
    /// 
    /// </summary>
    private void InitPlayerStatusPanel()
    {
        if (playerPanel == null)
        {
            Debug.LogError("Player Panel未赋值！");
            return;
        }

        if (statusTextPrefab == null)
        {
            Debug.LogError("Status Text Prefab未赋值！");
            return;
        }

        GameObject textObj = Instantiate(statusTextPrefab, playerPanel.transform);
        textObj.name = "PlayerStatus_All";
        TextMeshProUGUI statusText = textObj.GetComponent<TextMeshProUGUI>();
        if (statusText != null)
        {
            statusText.text = $"HEALTH: {playerHealth}\nMOVIN SPEED: {moveSpeed}\nSKILL CD RATE: {skillCDRate}";
            statusTextDict.Add("AllStatus", statusText);
        }
        else
        {
            Debug.LogError("预制体缺少TextMeshProUGUI组件！");
            Destroy(textObj);
        }

        // CreateStatusText("Health", "Health: " + playerHealth);
        //
        // CreateStatusText("MoveSpeed", "MoveSpeed: " + moveSpeed);
        //
        // CreateStatusText("SkillCDRate", "Skill CD Rate: " + skillCDRate);
    }

    /// <summary>

    /// </summary>
    /// <param name="key"></param>
    /// <param name="content"></param>
    private void CreateStatusText(string key, string content)
    {
        GameObject textObj = Instantiate(statusTextPrefab, playerPanel.transform);
        textObj.name = "Status_" + key; 
        
        TextMeshProUGUI statusText = textObj.GetComponent<TextMeshProUGUI>();

        if (statusText != null)
        {
            statusText.text = content;
            statusTextDict.Add(key, statusText); 
        }
        else
        {
            Debug.LogError($"创建{key}文本失败，预制体缺少TextMeshProUGUI组件！");
            Destroy(textObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerHealth = PlayerStatus.Instance.CurrentHealth;
        moveSpeed = PlayerStatus.Instance.MoveSpeed;
        skillCDRate = PlayerStatus.Instance.SkillCDRate;
        UpdateHealthUI();
        if (statusTextDict.ContainsKey("AllStatus"))
        {
            statusTextDict["AllStatus"].text = $"HEALTH: {playerHealth}\nMOVIN SPEED: {moveSpeed}\nSKILL CD RATE: {skillCDRate}";
        }
    }

    private void UpdateHealthUI()
    {
        hpText.text = $"HP:  {playerHealth:F1}  /  100.0";
    }
}
