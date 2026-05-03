using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;

    public string Name;

    public virtual void useItem()
    {
        Debug.Log("using item: " + Name);
    }
    public virtual void PickUp()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if (ItemUIpickupController.Instance != null)
        {
            ItemUIpickupController.Instance.ShowItemPickup(Name, itemIcon);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
