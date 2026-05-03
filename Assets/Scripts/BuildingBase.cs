using UnityEngine;

public enum BuildingState
{
    Normal,     
    Polluted,   
    Purified    
}

public enum BuildingType
{
    Farm,
    IcecreamShop,
    Bank,
    Restaurant,
    WizardHouse,
    Furnace,
    WeaponMaker,
    Supermarket,
    MagicainPenguin,
    PostOffice,
    Pharmacy,
    VendingMachine
}


public class BuildingBase : MonoBehaviour
{
    public string BuildingID { get; private set; }
    public BuildingType BuildingType { get; set; }
    public BuildingState CurrentState { get; set; } = BuildingState.Normal;

    public static event System.Action<string, BuildingState> OnBuildingStateChanged;
    
    protected SpriteRenderer sr;

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
        }
        BuildingID = GlobalHelper.GenerateUniqueID(gameObject);
        UpdateStateVisual();
    }
    public void UpdateStateVisual()
    {
        if (sr == null) return;
        
        switch (CurrentState)
        {
            case BuildingState.Normal:
                sr.color = Color.white;
                break;
            case BuildingState.Polluted:
                sr.color = Color.red;
                break;
            case BuildingState.Purified:
                sr.color = Color.green;
                break;
        }
    }
    
    public void SetBuildingState(BuildingState newState)
    {
        CurrentState = newState;
        MapGenerator.Instance.ChangeBuildingStateByID(BuildingID, CurrentState);
        UpdateStateVisual();
        OnBuildingStateChanged?.Invoke(BuildingID, newState);
    }
    
}