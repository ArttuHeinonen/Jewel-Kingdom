using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource {

    private string name;
    private double current;
    private double maxValue;
    private double increment;
    private double production;
    private double bonus;

    #region Property
    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public double Current
    {
        get
        {
            return current;
        }

        set
        {
            current = CheckLimit(value, maxValue);
        }
    }

    public double MaxValue
    {
        get
        {
            return maxValue;
        }

        set
        {
            maxValue = value;
        }
    }

    public double Increment
    {
        get
        {
            return increment;
        }

        set
        {
            increment = value;
        }
    }

    public double Production
    {
        get
        {
            return production;
        }

        set
        {
            production = value;
        }
    }

    public double Bonus
    {
        get
        {
            return bonus;
        }

        set
        {
            bonus = value;
        }
    }
    #endregion

    public Resource()
    {

    }

    private double CheckLimit(double value, double max)
    {
        if (value > max)
        {
            value = max;
        }
        else if (value < 0)
        {
            value = 0;
        }
        return value;
    }
}
