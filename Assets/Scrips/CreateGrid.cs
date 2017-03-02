using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateGrid : MonoBehaviour {

    public GameObject parent;
    public GameObject hexagon;
    public GameObject newHex;

    public const string SORTING_LAYER = "Grid";
    public float hexW = 0.5f;
    public float hexH = 1f;
    public const float gridOffset = 0.86f;
    public List<Sprite> icons;
    public int matches;

    public void InitGrid() {

        for (int row = 0; row < GameController.maxRows; row++)
        {
            for (int col = 0; col < GameController.maxColumns; col++)
            {
                Vector3 pos = new Vector3(parent.transform.position.x + gridOffset * col, parent.transform.position.y + (hexH * row + hexW * col % 1), 0);
                newHex = SpawnHex(pos);
                newHex.GetComponent<Hex>().SetRowAndColumn(row, col);
                GameController.Instance.hexes[row, col] = newHex;
            }
        }
        GameController.Instance.CheckForMatches();
	}

    public GameObject SpawnHex(Vector3 pos)
    {
        GameObject hex;
        int random = Random.Range(0, icons.Count);

        hex = Instantiate(hexagon, pos, Quaternion.identity) as GameObject;
        hex.transform.SetParent(parent.transform, false);
        hex.GetComponent<SpriteRenderer>().sortingLayerName = SORTING_LAYER;
        hex.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = icons[random];
        hex.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = SORTING_LAYER;
        hex.GetComponent<Hex>().AssignEnumType(random);

        return hex;
    }
}
