using UnityEngine;
using System.Collections;

public class Hex : MonoBehaviour {

	public enum Type : int { Food = 0, Stone = 1, Wood = 2, Empty = 999};
    public Type type;
    public int row;
    public int column;
    public Vector3 startingPoint;
    public Vector3 maxDragDistance = new Vector3(1.25f, 1.25f, 0);
    public Vector3 targetPosition;
    public bool shouldFall = false;
    public bool toBeFilled;
    public bool hasReachedTarget;
    public bool isDragged;
    public int targetRow;

    public const string GRID_LAYER = "Grid";
    public const string HIGH_LAYER = "Highlighter";

    void OnMouseEnter()
    {
        //Debug.Log("Entered: " + row + ", " + column + ". Pos: " + this.transform.position);
        GameController.Instance.UpdateHighlighter(this.transform.position);
    }

    void OnMouseDown()
    {
        startingPoint = this.transform.position;
        isDragged = true;
        SetToHighlighterLayer();
    }

    void OnMouseDrag()
    {
        if (IsDraggedTooFar())
        {
            OnMouseUp();
        }
        else if(isDragged)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            this.transform.position = curPosition;
            GameController.Instance.UpdateHighlighter(this.transform.position);
            Debug.Log("Hex: " + row + ", " + column + ". Mouse: " + curPosition);
        }
    }

    void OnMouseUp()
    {
        if (isDragged)
        {
            SetToDefaultLayer();
            this.transform.position = startingPoint;
            GameController.Instance.UpdateHighlighter(this.transform.position);
            isDragged = false;
        }
    }

    public void SetRowAndColumn(int r, int c)
    {
        row = r;
        column = c;
    }

    public void AssignEnumType(int givenType)
    {
        switch (givenType)
        {
            case 0:
                this.type = Type.Food;
                break;
            case 1:
                this.type = Type.Stone;
                break;
            case 2:
                this.type = Type.Wood;
                break;
            default:
                break;
        }
    }

    bool IsDraggedTooFar()
    {
        if(Mathf.Abs(this.transform.position.x - startingPoint.x) > maxDragDistance.x)
        {
            return true;
        }
        else if (Mathf.Abs(this.transform.position.y - startingPoint.y) > maxDragDistance.y)
        {
            return true;
        }
        return false;
    }

    void SetToHighlighterLayer()
    {
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = HIGH_LAYER;
        gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = HIGH_LAYER;
    }

    void SetToDefaultLayer()
    {
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = GRID_LAYER;
        gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = GRID_LAYER;
    }
}
