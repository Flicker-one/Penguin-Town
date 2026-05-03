using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public RectTransform mapRoot;
    public int gridRow = 4;
    public int gridCol = 4;
    public float gridSize = 32f;
    public float gridSpacing = 2f;
    public Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    
    private Image[,] gridBackgrounds;
    private Image[,] gridIcons;

    private MapGenerator mapGenerator;
    private bool isMapInited = false;

    private void Awake()
    {
        gridBackgrounds = new Image[gridRow, gridCol];
        gridIcons = new Image[gridRow, gridCol];
        mapRoot.anchorMin = new Vector2(0, 1);
        mapRoot.anchorMax = new Vector2(0, 1);
        mapRoot.pivot = new Vector2(0, 1);
        mapRoot.anchoredPosition = new Vector2(-100, 50);

        GenerateGridUI();
        
        StartCoroutine(WaitForMapGenerator());
    }
    
    private void GenerateGridUI()
    {
        float totalW = gridCol * (gridSize + gridSpacing);
        float totalH = gridRow * (gridSize + gridSpacing);
        mapRoot.sizeDelta = new Vector2(totalW, totalH);

        for (int row = 0; row < gridRow; row++)
        {
            for (int col = 0; col < gridCol; col++)
            {
                GameObject bg = new GameObject($"BG_{row}_{col}");
                bg.transform.SetParent(mapRoot, false);
                Image bgImg = bg.AddComponent<Image>();
                bgImg.color = defaultColor;
                RectTransform bgRt = bg.GetComponent<RectTransform>();
                bgRt.sizeDelta = new Vector2(gridSize, gridSize);
                bgRt.anchoredPosition = new Vector2(col * (gridSize + gridSpacing), -row * (gridSize + gridSpacing));
                gridBackgrounds[row, col] = bgImg;
                // show building's sprite
                GameObject icon = new GameObject($"Icon_{row}_{col}");
                icon.transform.SetParent(bg.transform, false);
                Image iconImg = icon.AddComponent<Image>();
                iconImg.color = Color.white;
                RectTransform iconRt = icon.GetComponent<RectTransform>();
                iconRt.sizeDelta = new Vector2(gridSize * 0.8f, gridSize * 0.8f); // 图标略小
                iconRt.anchoredPosition = Vector2.zero;
                gridIcons[row, col] = iconImg;
            }
        }
    }

    private IEnumerator WaitForMapGenerator()
    {
        yield return new WaitUntil(() => MapGenerator.Instance != null);
        mapGenerator = MapGenerator.Instance;
        yield return new WaitUntil(() => mapGenerator.mapBuildingData.Count >= 16);
        yield return null;
        StartCoroutine(InitAllBuildings());
    }

    // initialize icon and coor
    private IEnumerator InitAllBuildings()
    {
        while (mapGenerator.mapBuildingData.Count == 0)
            yield return new WaitForSeconds(0.1f);

        for (int slot = 0; slot < 16; slot++)
        {
            int row = slot / 4;
            int col = slot % 4;

            if (mapGenerator.mapBuildingData.TryGetValue(slot, out var data))
            {
                // get sprite component
                if (data.buildingObj != null)
                {
                    var buildingSprite = data.buildingObj.GetComponentInChildren<SpriteRenderer>()?.sprite;
                    gridIcons[row, col].sprite = buildingSprite;
                    gridIcons[row, col].enabled = buildingSprite != null;
                }

                UpdateGridColor(row, col, data.buildingState);
            }
            else
            {
                gridIcons[row, col].sprite = null;
                gridIcons[row, col].enabled = false;
                gridBackgrounds[row, col].color = defaultColor;
            }
        }

        isMapInited = true;
        StartCoroutine(UpdateLoop());
    }

    // Update color every 0.1s interval
    private IEnumerator UpdateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (!isMapInited) continue;

            for (int slot = 0; slot < 16; slot++)
            {
                if (mapGenerator.mapBuildingData.TryGetValue(slot, out var data))
                {
                    int row = slot / 4;
                    int col = slot % 4;
                    UpdateGridColor(row, col, data.buildingState);
                }
            }
        }
    }
    
    private void UpdateGridColor(int row, int col, BuildingState state)
    {
        Color color = state switch
        {
            BuildingState.Normal => Color.white,
            BuildingState.Polluted => Color.red,
            BuildingState.Purified => Color.green,
            _ => defaultColor
        };
        gridBackgrounds[row, col].color = color;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
