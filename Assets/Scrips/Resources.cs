using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources {

    private Resource food;
    private Resource wood;
    private Resource stone;
    private Resource gold;


    #region Properties
    public Resource Food
    {
        get
        {
            return food;
        }

        set
        {
            food = value;
        }
    }

    public Resource Wood
    {
        get
        {
            return wood;
        }

        set
        {
            wood = value;
        }
    }

    public Resource Stone
    {
        get
        {
            return stone;
        }

        set
        {
            stone = value;
        }
    }

    public Resource Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
        }
    }
    #endregion

    public Resources()
    {
        food = new Resource();
        wood = new Resource();
        stone = new Resource();
        gold = new Resource();
    }

    public void ResetValues()
    {
        food.Name = "Food";
        food.Current = 0;
        food.MaxValue = 100;
        food.Increment = 1;
        food.Production = 0;
        food.Bonus = 1.0d;

        wood.Name = "Wood";
        wood.Current = 0;
        wood.MaxValue = 100;
        wood.Increment = 1;
        wood.Production = 0;

        stone.Name = "Stone";
        stone.Current = 0;
        stone.MaxValue = 20;
        stone.Increment = 1;
        stone.Production = 0;
        stone.Bonus = 1.0d;

        gold.Name = "Gold";
        gold.Current = 0;
        gold.MaxValue = 10;
        gold.Increment = 1;
        gold.Production = 0;
        gold.Bonus = 1.0d;
    }
}
