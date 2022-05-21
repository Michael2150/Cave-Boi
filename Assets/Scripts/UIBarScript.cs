using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarScript : MonoBehaviour
{
    [SerializeField] int maxNumIcons = 10;
    [SerializeField] Image barStatImage;
    [SerializeField] Vector3 offSet = new Vector3(50,0,0);
    [SerializeField] bool visible = true;
    private ArrayList childrenImages = new ArrayList(); //The list of the icons as Image objects

    // Start is called before the first frame update
    void Start()
    {
        Vector3 scale = barStatImage.gameObject.GetComponent<RectTransform>().localScale;
        Vector3 pos = barStatImage.gameObject.GetComponent<RectTransform>().position;

        for (int i = 0; i < maxNumIcons; i++)
        {
            Image newImg = Instantiate<Image>(barStatImage);
            newImg.transform.SetParent(gameObject.transform);
            newImg.gameObject.GetComponent<RectTransform>().localScale = scale;
            newImg.gameObject.GetComponent<RectTransform>().position = pos - offSet * i;
            childrenImages.Add(newImg);
        }

        barStatImage.gameObject.SetActive(false);
    }

    //statVal needs to be between 0 and the maxNumIcons (default 10)
    public void updateStat(float statVal)
    {
        if (visible)
        {
            int shouldBeOn = Mathf.RoundToInt(statVal);

            int i = 0;
            foreach (Image img in childrenImages)
            {
                img.gameObject.SetActive(i < (statVal));
                i++;
            }
        }
    }

    public int getMaxStat()
    {
        return maxNumIcons;
    }

    public void setVisible(bool _visible)
    {
        visible = _visible;
        if (!visible)
        {
            foreach (Image img in childrenImages)
            {
                img.gameObject.SetActive(false);
            }
        }
    }
}
