using UnityEngine;
using TMPro; // 如果用UGUI的Text则替换为 using UnityEngine.UI;

public class GameResultUIManager : MonoBehaviour
{
    [Header("胜利/失败文本")]
    [SerializeField] private TextMeshProUGUI wellDoneText; // 若用UGUI Text则改为 Text wellDoneText;
    [SerializeField] private TextMeshProUGUI gameOverText; // 若用UGUI Text则改为 Text gameOverText;

    private void Awake()
    {
        // 检查组件赋值
        if (wellDoneText == null || gameOverText == null)
        {
            Debug.LogError("胜利/失败文本组件未赋值！");
            return;
        }

        // 初始隐藏文本
        wellDoneText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // 注册输赢回调
        if (GameWinLoseManager.Instance != null)
        {
            GameWinLoseManager.Instance.OnGameWin += ShowWellDone;
            GameWinLoseManager.Instance.OnGameLose += ShowGameOver;
        }
        else
        {
            Debug.LogError("GameWinLoseManager 实例不存在！");
        }
    }

    private void OnDisable()
    {
        // 取消注册（防止内存泄漏）
        if (GameWinLoseManager.Instance != null)
        {
            GameWinLoseManager.Instance.OnGameWin -= ShowWellDone;
            GameWinLoseManager.Instance.OnGameLose -= ShowGameOver;
        }
    }

    // 显示胜利文本
    private void ShowWellDone()
    {
        wellDoneText.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
    }

    // 显示失败文本
    private void ShowGameOver()
    {
        gameOverText.gameObject.SetActive(true);
        wellDoneText.gameObject.SetActive(false);
    }

    // 重置文本显示（可选，用于重新开始游戏时）
    public void ResetResultUI()
    {
        wellDoneText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
    }
}
