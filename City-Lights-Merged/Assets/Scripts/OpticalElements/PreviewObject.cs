using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour {

    public enum Element { MIRROR, PRISM, LENS, SPLITTER };
    protected Element element;
    protected SpriteRenderer spriteRenderer;

    public bool hitByRay;
    private NetworkHelper networker;

    //public Animation animation;

    // preview graphics
    public Sprite mirror;
    public Sprite prism;
    public Sprite lens;
    public Sprite splitter;

    // Use this for initialization
    void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        networker = FindObjectOfType<NetworkHelper>();

        if (networker.connected) networker.PlayAudio("F_Create");
        else FindObjectOfType<AudioManagerFloor>().Play("Create");

        //animation.Play("wait");
    }
	
	// Update is called once per frame
	void Update () {
        // choose the right graphic for the element
		switch (element)
        {
            case Element.MIRROR:
                spriteRenderer.sprite = mirror;
                break;
            case Element.PRISM:
                spriteRenderer.sprite = prism;
                break;
            case Element.LENS:
                spriteRenderer.sprite = lens;
                break;
            case Element.SPLITTER:
                spriteRenderer.sprite = splitter;
                break;
        }

        // toggle lens / prism
        if (hitByRay && element == Element.PRISM)
        {
            element = Element.LENS;
        }
        else if (!hitByRay && element == Element.LENS)
        {
            element = Element.PRISM;
        }
    }

    public void SetElement(Element element)
    {
        this.element = element;
    }

    public Element GetElement()
    {
        return element;
    }

}
