using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorage : MonoBehaviour
{
    [SerializeField]
    private List<ElementIcons> elements;

    private Dictionary<Element, Sprite> blank;
    private Dictionary<Element, Sprite> colorful;

    public Indexer<Element, Sprite> BlankElementsSprites;
    public Indexer<Element, Sprite> ColorfulElementsSprites;

    private void Awake()
    {
        blank = new Dictionary<Element, Sprite>();
        colorful = new Dictionary<Element, Sprite>();
        foreach (var element in elements)
        {
            blank[element.element] = element.BlankImage;
            colorful[element.element] = element.ColorImage;
        }
        BlankElementsSprites = new Indexer<Element, Sprite>(e => blank[e]);
        ColorfulElementsSprites = new Indexer<Element, Sprite>(e => colorful[e]);
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

public class Indexer<Key, Value>
{
    private System.Func<Key, Value> getter;
    public Indexer(System.Func<Key, Value> getter)
    {
        this.getter = getter;
    }

    public Value this[Key key]
    {
        get => getter(key);
    }
}

[System.Serializable]
public class ElementIcons
{
    public Element element;
    public Sprite ColorImage;
    public Sprite BlankImage;
}
