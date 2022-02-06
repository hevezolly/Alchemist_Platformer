using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CookingAnimationActivator : MonoBehaviour
{
    public Button button;
    public Animator animator;

    public List<ChannelToIndex> indexes;

    private Dictionary<InventoryCellCord, Image> images;

    private bool isAnimating;
    private bool isCooking;

    private void Start()
    {
        images = new Dictionary<InventoryCellCord, Image>();
        foreach (var index in indexes)
        {
            var cord = new InventoryCellCord(index.index, CellType.Potions);
            images[cord] = index.channel;
        }
    }

    public void OnAnimationFinish()
    {
        isAnimating = false;
        if (!isCooking)
            button.interactable = true;
    }

    public void CookSuccessfull(Dictionary<InventoryCellCord, Texture2D> colors, float fillTime)
    {
        button.interactable = false;
        foreach (var cord in colors.Keys)
        {
            images[cord].material.SetTexture("_FilledColor", colors[cord]);
        }
        StartCoroutine(FillChannels(colors.Keys.Select(k => images[k]).ToList(), fillTime));
        isAnimating = true;
        animator.SetTrigger("Turn");
    }

    public void CookFail()
    {
        button.interactable = false;
        isAnimating = true;
        animator.SetTrigger("Turn");
    }

    private IEnumerator FillChannels(List<Image> images, float fillTime)
    {
        isCooking = true;
        var initialDirections = images.Select(i => i.material.GetVector("_Direction")).ToList();
        var finalTime = Time.time + fillTime / 2;
        while (Time.time < finalTime)
        {
            var t = Mathf.Clamp01((finalTime - Time.time) / (fillTime / 2));
            images.ForEach(i => i.material.SetFloat("_FillPercent",
                Mathf.Lerp(1, 0, t)));
            yield return new WaitForEndOfFrame();
        }
        images.ForEach(i => i.material.SetVector("_Direction", -i.material.GetVector("_Direction")));
        finalTime = Time.time + fillTime / 2;
        while (Time.time < finalTime)
        {
            var t = Mathf.Clamp01((finalTime - Time.time) / (fillTime / 2));
            images.ForEach(i => i.material.SetFloat("_FillPercent",
                Mathf.Lerp(0, 1, t)));
            yield return new WaitForEndOfFrame();
        }
        for (var i = 0; i < initialDirections.Count; i++)
        {
            images[i].material.SetVector("_Direction", initialDirections[i]);
            images[i].material.SetFloat("_FillPercent", 0);
            images[i].material.SetColor("_FilledColor", Color.white);
        }
        isCooking = false;
        if (!isAnimating)
            button.interactable = true;
    }
}

[System.Serializable]
public class ChannelToIndex
{
    public int index;
    public Image channel;
}
