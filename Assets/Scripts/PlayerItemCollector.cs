using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    // Start is called before the first frame update
    private ItemController itemController;
    public AudioClip pickUpSFX;
    void Start()
    {
        itemController = FindObjectOfType<ItemController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                bool itemAdded = itemController.AddItem(collision.gameObject);
                if (itemAdded)
                {
                    AudioSource.PlayClipAtPoint(pickUpSFX, transform.position);
                    item.PickUp();
                    Destroy(collision.gameObject);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
