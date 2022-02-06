using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public ModulatedPlayerMovement movement;
    private PlayerInventory Inventory;
    private HotBarController hotBar;
    public LayerMask pickUpLayer;
    public float PickUpRadius;
    private bool state = false;
    void Start()
    {
        var inventoryObj = GameObject.FindGameObjectWithTag("Inventory");
        Inventory = inventoryObj.GetComponent<PlayerInventory>();
        hotBar = inventoryObj.GetComponent<HotBarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var items = Physics2D.OverlapCircleAll(transform.position, PickUpRadius, pickUpLayer);
            PIckupable theOne = null;
            var distance = PickUpRadius + 1;
            foreach (var i in items)
            {
                var current = Vector2.Distance(i.transform.position, transform.position);
                if (current < distance)
                {
                    distance = current;
                    theOne = i.GetComponent<PIckupable>();
                }
            }
            if (theOne != null)
                theOne.PickUp(Inventory);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            state = !state;
            Inventory.Turn(state);
        }
        if (!state)
        {
            CheckHotBarSelect();
            CheckAction();
        }
    }

    private void CheckHotBarSelect()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            hotBar.TrySetSelected(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            hotBar.TrySetSelected(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            hotBar.TrySetSelected(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            hotBar.TrySetSelected(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            hotBar.TrySetSelected(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            hotBar.TrySetSelected(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            hotBar.TrySetSelected(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            hotBar.TrySetSelected(7);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            hotBar.TrySetSelected(8);
    }

    private void CheckAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var dir = mouse - transform.position;
            var item = hotBar.GetItem() as IThrowable;
            if (item != null && item.CanBeThrown)
            {
                var info = new ThrowInformation();
                info.origin = transform.position;
                info.direction = dir;
                info.thrower = gameObject;
                item.Throw(info);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            var item = hotBar.GetItem() as IDrinkable;
            if (item != null && item.CanBeDrinked)
                item.Drink(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, PickUpRadius);
    }
}
