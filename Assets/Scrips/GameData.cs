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

    public void IncrementResource(Type type)
    {
        switch (type)
        {
            case Type.Food:
                res.Food.Current += res.Food.Increment;
                break;
            case Type.Stone:
                res.Stone.Current += res.Stone.Increment;
                break;
            case Type.Wood:
                res.Wood.Current += res.Wood.Increment;
                break;
            case Type.Science:
                res.Science.Current += res.Science.Increment;
                break;
            case Type.Empty:
                Debug.Log("Empty type removed! How is this even possible!?");
                break;
            default:
                break;
        }
    }

    public void UpdateData()
    {
        //ResourceTextManager.Instance.ChangeValues(res);
    }
}
