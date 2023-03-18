using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Bestiary : MonoBehaviour
{
    public Sprite[] mobImages;
    public string[] mobDescriptions;

    public TMP_Text leftText;
    public TMP_Text rightText;

    public Image leftImage;
    public Image rightImage;

    private int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay();
    }

    public void Next()
    {
        if (currentIndex < mobImages.Length / 2 -1 )
        {
            currentIndex++;
            UpdateDisplay();
        }
        
    }

    public void Prev()
    {
        
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateDisplay();
        }
        
    }
    
    void UpdateDisplay()
    {
        leftImage.sprite = mobImages[currentIndex * 2];
        rightImage.sprite = mobImages[currentIndex * 2 + 1];
        leftText.text = mobDescriptions[currentIndex * 2];
        rightText.text = mobDescriptions[currentIndex * 2 + 1];
    }

}
