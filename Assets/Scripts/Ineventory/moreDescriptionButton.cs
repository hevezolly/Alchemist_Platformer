using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moreDescriptionButton : MonoBehaviour
{
    public GameObject HidingObject;

    public void ResetClick()
    {
        transform.localScale = Vector3.one;
        HidingObject.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,
            transform.localScale.z);
        HidingObject.SetActive(!HidingObject.activeSelf);
    }
}
