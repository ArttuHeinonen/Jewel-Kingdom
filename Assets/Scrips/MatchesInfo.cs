using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchesInfo {

    private List<GameObject> matchedCandies;

    /// <summary>
    /// Returns distinct list of matched candy
    /// </summary>
    public IEnumerable<GameObject> MatchedHex
    {
        get
        {
            return matchedCandies.Distinct();
        }
    }

    public void AddObject(GameObject go)
    {
        if (!matchedCandies.Contains(go))
            matchedCandies.Add(go);
    }

    public void AddObjectRange(IEnumerable<GameObject> gos)
    {
        foreach (GameObject item in gos)
        {
            AddObject(item);
        }
    }

    public MatchesInfo()
    {
        matchedCandies = new List<GameObject>();
        BonusesContained = BonusType.None;
    }

    public BonusType BonusesContained { get; set; }
}
