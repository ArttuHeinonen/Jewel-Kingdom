using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTextManager : MonoBehaviour {

    public static ResourceTextManager Instance;

    public Text foodName;
    public Text foodCurrent;
    public Text foodMax;
    public Text foodPro;

    public Color32 maxColor;

    public Color32 defaultColor;
    public Color32 almostFullColor;
    public Color32 fullColor;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start ()
    {
        maxColor = new Color32(165, 165, 165, 255);
        defaultColor = new Color32(255, 255, 255, 255);
        almostFullColor = new Color32(255, 128, 0, 255);
        fullColor = new Color32(255, 255, 0, 255);
        Init();
    }

    public void Init()
    {
        foodName.text = "Food:";
        foodMax.color = maxColor;
    }

    public void ChangeValues(Resources res)
    {
        foodName.text = res.Food.Name + ":";
        foodCurrent.text = res.Food.Current.ToString();
        foodMax.text = "/" + res.Food.MaxValue.ToString();
        foodPro.text = "(+" + res.Food.Production.ToString() + "/tick)";
    }

    public void ChangeCurrentValues(Resources res)
    {
        foodCurrent.text = res.Food.Increment.ToString();
    }

}
