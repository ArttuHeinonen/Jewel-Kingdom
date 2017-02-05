using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateGrid : MonoBehaviour {

    public Hex.Type[,] array;
    public GameObject parent;
    public GameObject hexagon;
    public GameObject newHex;
    public const int maxColumns = 6;
    public const int maxRows = 20;
    public float hexW = 0.5f;
    public float hexH = 1f;
    public List<Sprite> icons;
    public int matches;

    void Start () {
        array = new Hex.Type[maxRows, maxColumns];
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxColumns; j++)
            {
                Vector3 pos = new Vector3(parent.transform.position.x + 0.86f * j, parent.transform.position.y + (hexH * i + hexW * j % 1), 0);
                newHex = Instantiate(hexagon, pos, Quaternion.identity) as GameObject;
                newHex.transform.SetParent(parent.transform, false);
                ShuffleGrid();
                array[i, j] = newHex.GetComponent<Hex>().type;
                CheckIfMatch(i, j);
                newHex.GetComponent<Hex>().SetRowAndColumn(i, j);
                GameController.game.hexes.Add(newHex);
            }
        }
        GameController.game.UpdateArrays();
	}

    void ShuffleGrid()
    {
        int random = Random.Range(0, icons.Count);

        newHex.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = icons[random];
        newHex.GetComponent<Hex>().AssignEnumType(random);
    }

    void CheckIfMatch(int row, int column)
    {
        if(row > 2 || column > 2)
        {

        }
    }
}
