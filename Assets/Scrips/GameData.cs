using UnityEngine;
using System.Collections;

public class GameData {

    public float currentFood;
    public float valueFood;
    public float maxFood;
    public float currentStone;
    public float valueStone;
    public float maxStone;
    public float currentWood;
    public float valueWood;
    public float maxWood;
	
    public GameData()
    {
        currentFood = 0;
        currentStone = 0;
        currentWood = 0;
        maxFood = 100;
        maxStone = 10;
        maxWood = 50;
        valueFood = 1;
        valueStone = 0.5f;
        valueWood = 0.75f;
    }

    public void UpdateData(GameData data)
    {
        currentFood = data.currentFood;
        maxFood = data.maxFood;
        currentStone = data.currentStone;
        maxStone = data.maxStone;
        currentWood = data.currentWood;
        maxWood = data.maxWood;
    }

    public void Increment(Hex.Type type)
    {
        switch (type)
        {
            case Hex.Type.Food:
                currentFood += valueFood;
                break;
            case Hex.Type.Stone:
                currentStone += valueStone;
                break;
            case Hex.Type.Wood:
                currentWood += valueWood;
                break;
            case Hex.Type.Empty:
                Debug.Log("Empty type removed! How is this even possible!?");
                break;
            default:
                break;
        }
        CheckLimits();
    }

    public void CheckLimits()
    {
        if(currentFood > maxFood)
        {
            currentFood = maxFood;
        }
        if(currentStone > maxStone)
        {
            currentStone = maxStone;
        }
        if (currentWood > maxWood)
        {
            currentWood = maxWood;
        }
    }
}
