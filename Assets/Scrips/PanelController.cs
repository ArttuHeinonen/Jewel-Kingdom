using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour {

    public GameObject grid;
    public GameObject gridPanel;
    public GameObject resourcesPanel;
    public GameObject villagePanel;
    public GameObject craftPanel;

    public GameObject gridButton;
    public GameObject resourceButton;
    public GameObject villageButton;
    public GameObject craftButton;

    public Color32 deactiveColor;
    public Color32 activeColor;

    void Start () {
        deactiveColor = new Color32(48, 48, 48, 255);
        activeColor = new Color32(128, 128, 128, 255);
        ButtonPressed("Grid");
    }
	
	

    public void ButtonsReset()
    {
        gridButton.GetComponent<Image>().color = deactiveColor;
        resourceButton.GetComponent<Image>().color = deactiveColor;
        villageButton.GetComponent<Image>().color = deactiveColor;
        craftButton.GetComponent<Image>().color = deactiveColor;
    }

    public void PanelsReset()
    {
        grid.SetActive(false);
        gridPanel.SetActive(false);
        resourcesPanel.SetActive(false);
        villagePanel.SetActive(false);
        craftPanel.SetActive(false);
        GameController.Instance.hexManager.highlighter.SetActive(false);
    }

    public void ButtonPressed(string name)
    {
        ButtonsReset();
        PanelsReset();

        switch (name)
        {
            case "Grid":
                gridButton.GetComponent<Image>().color = activeColor;
                grid.SetActive(true);
                gridPanel.SetActive(true);
                GameController.Instance.hexManager.highlighter.SetActive(true);
                break;
            case "Resource":
                resourceButton.GetComponent<Image>().color = activeColor;
                resourcesPanel.SetActive(true);
                break;
            case "Village":
                villageButton.GetComponent<Image>().color = activeColor;
                villagePanel.SetActive(true);
                break;
            case "Craft":
                craftButton.GetComponent<Image>().color = activeColor;
                craftPanel.SetActive(true);
                break;
            default:
                break;
        }
    }
}
