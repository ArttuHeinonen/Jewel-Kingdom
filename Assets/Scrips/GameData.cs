using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class GameData {

    public Resources res;

    public GameData()
    {
        res = new Resources();
        res.ResetValues();
    }

    public void IncrementResource(Hex.Type type)
    {
        switch (type)
        {
            case Hex.Type.Food:
                res.Food.Current += res.Food.Increment;
                break;
            case Hex.Type.Stone:
                res.Stone.Current += res.Stone.Increment;
                break;
            case Hex.Type.Wood:
                res.Wood.Current += res.Wood.Increment;
                break;

            case Hex.Type.Empty:
                Debug.Log("Empty type removed! How is this even possible!?");
                break;
            default:
                break;
        }
    }

    public void UpdateData()
    {
        ResourceTextManager.Instance.ChangeValues(res);
    }
}
