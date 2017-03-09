using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AlteredHexInfo {

    private List<GameObject> newHex { get; set; }
    public int MaxDistance { get; set; }

    /// <summary>
    /// Returns distinct list of altered candy
    /// </summary>
    public IEnumerable<GameObject> AlteredHex
    {
        get
        {
            return newHex.Distinct();
        }
    }

    public void AddHex(GameObject go)
    {
        if (!newHex.Contains(go))
            newHex.Add(go);
    }

    public AlteredHexInfo()
    {
        newHex = new List<GameObject>();
    }
}
