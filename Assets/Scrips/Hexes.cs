using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hexes {

    private GameObject[,] hexes = new GameObject[Constants.MaxRows, Constants.MaxCols];

    private GameObject backupG1;
    private GameObject backupG2;

    public GameObject this[int row, int column]
    {
        get
        {
            try
            {
                return hexes[row, column];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        set
        {
            hexes[row, column] = value;
        }
    }

    public void Swap(GameObject g1, GameObject g2)
    {
        backupG1 = g1;
        backupG2 = g2;

        Hex h1 = g1.GetComponent<Hex>();
        Hex h2 = g2.GetComponent<Hex>();

        int h1Row = h1.Row;
        int h1Column = h1.Col;
        int h2Row = h2.Row;
        int h2Column = h2.Col;

        GameObject temp = hexes[h1Row, h1Column];
        hexes[h1Row, h1Column] = hexes[h2Row, h2Column];
        hexes[h2Row, h2Column] = temp;

        Hex.SwapColumnRow(h1, h2);
    }

    public void UndoSwap()
    {
        if (backupG1 == null || backupG2 == null)
            throw new Exception("Backup is null");

        Swap(backupG1, backupG2);
    }

    public IEnumerable<GameObject> GetMatches(IEnumerable<GameObject> gos)
    {
        List<GameObject> matches = new List<GameObject>();
        foreach (var go in gos)
        {
            matches.AddRange(GetMatches(go).MatchedHex);
        }
        return matches.Distinct();
    }

    public MatchesInfo GetMatches(GameObject go)
    {
        MatchesInfo matchesInfo = new MatchesInfo();

        //var horizontalDownUpMatches = GetMatchesHorizontallyDownUp(go);
        //matchesInfo.AddObjectRange(horizontalMatches);

        var verticalMatches = GetMatchesVertically(go);
        matchesInfo.AddObjectRange(verticalMatches);

        return matchesInfo;
    }

    private IEnumerable<GameObject> GetMatchesVertically(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var hex = go.GetComponent<Hex>();
        //check bottom
        if (hex.Row != 0)
            for (int row = hex.Row - 1; row >= 0; row--)
            {
                if (hexes[row, hex.Col] != null &&
                    hexes[row, hex.Col].GetComponent<Hex>().IsSameType(hex))
                {
                    matches.Add(hexes[row, hex.Col]);
                }
                else
                    break;
            }

        //check top
        if (hex.Row != Constants.MaxRows - 1)
            for (int row = hex.Row + 1; row < Constants.MaxRows; row++)
            {
                if (hexes[row, hex.Col] != null &&
                    hexes[row, hex.Col].GetComponent<Hex>().IsSameType(hex))
                {
                    matches.Add(hexes[row, hex.Col]);
                }
                else
                    break;
            }

        if (matches.Count < Constants.MinMatches)
            matches.Clear();

        return matches.Distinct();
    }


    public void Remove(GameObject item)
    {
        hexes[item.GetComponent<Hex>().Row, item.GetComponent<Hex>().Col] = null;
    }

    public AlteredHexInfo Collapse(IEnumerable<int> columns)
    {
        AlteredHexInfo collapseInfo = new AlteredHexInfo();


        ///search in every column
        foreach (var column in columns)
        {
            //begin from bottom row
            for (int row = 0; row < Constants.MaxRows - 1; row++)
            {
                //if you find a null item
                if (hexes[row, column] == null)
                {
                    //start searching for the first non-null
                    for (int row2 = row + 1; row2 < Constants.MaxRows; row2++)
                    {
                        //if you find one, bring it down (i.e. replace it with the null you found)
                        if (hexes[row2, column] != null)
                        {
                            hexes[row, column] = hexes[row2, column];
                            hexes[row2, column] = null;

                            //calculate the biggest distance
                            if (row2 - row > collapseInfo.MaxDistance)
                                collapseInfo.MaxDistance = row2 - row;

                            //assign new row and column (name does not change)
                            hexes[row, column].GetComponent<Hex>().Row = row;
                            hexes[row, column].GetComponent<Hex>().Col = column;

                            collapseInfo.AddHex(hexes[row, column]);
                            break;
                        }
                    }
                }
            }
        }
        return collapseInfo;
    }

    public IEnumerable<HexInfo> GetEmptyItemsOnColumn(int column)
    {
        List<HexInfo> emptyItems = new List<HexInfo>();
        for (int row = 0; row < Constants.MaxRows; row++)
        {
            if (hexes[row, column] == null)
                emptyItems.Add(new HexInfo() { Row = row, Column = column });
        }
        return emptyItems;
    }
}
