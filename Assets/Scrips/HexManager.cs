using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexManager : MonoBehaviour {

    public Type type;
    public Hexes hexes;

    private GameState state = GameState.None;
    private GameObject hitGo = null;
    private Vector2[] spawnPositions;
    public GameObject[] hexPrefabs;
    public GameObject explosionPrefab;
    public GameObject highlighter;
    public GameObject grid;
    public Vector3 highlighterDefaultPosition;
    public Vector2 gridPosition;

    void Start () {
        InitializeHexAndSpawnPositions();
    }

    public void InitializeHexAndSpawnPositions()
    {
        if (hexes != null)
            DestroyAllHexes();

        hexes = new Hexes();
        spawnPositions = new Vector2[Constants.MaxCols];
        highlighterDefaultPosition = highlighter.transform.position;
        gridPosition = new Vector2(grid.transform.position.x, grid.transform.position.y);
        for (int row = 0; row < Constants.MaxRows; row++)
        {
            for (int column = 0; column < Constants.MaxCols; column++)
            {
                GameObject newHex = GetRandomHex();

                int offset = 0;
                if(column % 2 != 0)
                {
                    offset = 1;
                }

                //Down to up
                while ((column >= 2 && row >= 1) && hexes[row - 1 + offset, column - 1].GetComponent<Hex>().IsSameType(newHex.GetComponent<Hex>())
                    && hexes[row - 1, column - 2].GetComponent<Hex>().IsSameType(newHex.GetComponent<Hex>()))
                {
                    newHex = GetRandomHex();
                }

                //Up to Down
                //while ((column >= 2 && row < Constants.MaxRows - 1) && hexes[row + offset, column - 1].GetComponent<Hex>().IsSameType(newHex.GetComponent<Hex>())
                //    && hexes[row + 1, column - 2].GetComponent<Hex>().IsSameType(newHex.GetComponent<Hex>()))
                //{
                //    newHex = GetRandomHex();
                //}

                //check if two previous vertical are of the same type
                while (row >= 2 && hexes[row - 1, column].GetComponent<Hex>().IsSameType(newHex.GetComponent<Hex>())
                    && hexes[row - 2, column].GetComponent<Hex>().IsSameType(newHex.GetComponent<Hex>()))
                {
                    newHex = GetRandomHex();
                }
                InstantiateAndPlaceNewHex(row, column, newHex);
            }
        }
        SetupSpawnPositions();
    }

    private void InstantiateAndPlaceNewHex(int row, int column, GameObject newHex)
    {
        float yPosition = row * Constants.HexSize.y;
        float xPosition = column * Constants.GridOffset;
        if (column % 2 != 0)
        {
            yPosition += 0.5f;
        }
        GameObject go = Instantiate(newHex, gridPosition + new Vector2(xPosition, yPosition), Quaternion.identity) as GameObject;
        go.transform.SetParent(grid.transform);
        go.GetComponent<Hex>().Assign(newHex.GetComponent<Hex>().type, row, column);
        hexes[row, column] = go;
    }

    private void SetupSpawnPositions()
    {
        //create the spawn positions for the new shapes (will pop from the 'ceiling')
        for (int column = 0; column < Constants.MaxCols; column++)
        {
            float yPosition = Constants.MaxRows * Constants.HexSize.y;
            float xPosition = column * Constants.GridOffset;
            if (column % 2 != 0)
            {
                yPosition += 0.5f;
            }
            spawnPositions[column] = gridPosition + new Vector2(xPosition, yPosition);
        }
    }

    void Update () {

        if (state == GameState.None)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                hitGo = hit.collider.gameObject;
                UpdateHighlighter(hitGo.transform.position);
                if (Input.GetMouseButtonDown(0))
                {
                    state = GameState.SelectionStarted;
                }
            }
            else
            {
                ResetHighlighterPosition();
            }
        }
        else if (state == GameState.SelectionStarted)
        {
            //Dragged
            if (Input.GetMouseButton(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hitGo != hit.collider.gameObject)
                {
                    //if the two shapes are diagonally aligned (different row and column), just return
                    if (!Util.AreNeighbors(hitGo.GetComponent<Hex>(), hit.collider.gameObject.GetComponent<Hex>()))
                    {
                        state = GameState.None;
                    }
                    else
                    {
                        state = GameState.Animating;
                        FixSortingLayer(hitGo, hit.collider.gameObject);
                        StartCoroutine(FindMatchesAndCollapse(hit));
                    }
                }
            }
        }
    }

    private GameObject GetRandomHex()
    {
        return hexPrefabs[Random.Range(0, hexPrefabs.Length)];
    }

    private IEnumerator FindMatchesAndCollapse(RaycastHit2D hit2)
    {
        //get the second item that was part of the swipe
        GameObject hitGo2 = hit2.collider.gameObject;
        hexes.Swap(hitGo, hitGo2);

        //move the swapped ones
        Vector3 tempPos = hitGo.transform.position;
        hitGo.transform.position = hitGo2.transform.position;
        hitGo2.transform.position = tempPos;

        //hitGo.transform.position(Constants.AnimationDuration, hitGo2.transform.position);
        //hitGo2.transform.positionTo(Constants.AnimationDuration, hitGo.transform.position);
        yield return new WaitForSeconds(Constants.AnimationDuration);

        //get the matches via the helper methods
        var hitGomatchesInfo = hexes.GetMatches(hitGo);
        var hitGo2matchesInfo = hexes.GetMatches(hitGo2);

        var totalMatches = hitGomatchesInfo.MatchedHex.Union(hitGo2matchesInfo.MatchedHex).Distinct();

        //if user's swap didn't create at least a 3-match, undo their swap
        if (totalMatches.Count() < Constants.MinMatches)
        {
            tempPos = hitGo.transform.position;
            hitGo.transform.position = hitGo2.transform.position;
            hitGo2.transform.position = tempPos;

            //hitGo.transform.position(Constants.AnimationDuration, hitGo2.transform.position);
            //hitGo2.transform.positionTo(Constants.AnimationDuration, hitGo.transform.position);
            yield return new WaitForSeconds(Constants.AnimationDuration);

            hexes.UndoSwap();
        }

        //if more than 3 matches and no Bonus is contained in the line, we will award a new Bonus
        //bool addBonus = totalMatches.Count() >= Constants.MinimumMatchesForBonus &&
        //    !BonusTypeUtilities.ContainsDestroyWholeRowColumn(hitGomatchesInfo.BonusesContained) &&
        //    !BonusTypeUtilities.ContainsDestroyWholeRowColumn(hitGo2matchesInfo.BonusesContained);

        //Hex hitGoCache = null;

        //if (addBonus)
        //{
        //    //get the game object that was of the same type
        //    var sameTypeGo = hitGomatchesInfo.MatchedCandy.Count() > 0 ? hitGo : hitGo2;
        //    hitGoCache = sameTypeGo.GetComponent<Hex>();
        //}

        int timesRun = 1;
        while (totalMatches.Count() >= Constants.MinMatches)
        {
            //    //increase score
            //    IncreaseScore((totalMatches.Count() - 2) * Constants.Match3Score);

            //    if (timesRun >= 2)
            //        IncreaseScore(Constants.SubsequentMatchScore);

            //    //soundManager.PlayCrincle();

            foreach (var item in totalMatches)
            {
                hexes.Remove(item);
                RemoveFromScene(item);
            }

            //    //check and instantiate Bonus if needed
            //    if (addBonus)
            //        CreateBonus(hitGoCache);

            //    addBonus = false;

            //get the columns that we had a collapse
            var columns = totalMatches.Select(go => go.GetComponent<Hex>().Col).Distinct();

            //the order the 2 methods below get called is important!!!
            //collapse the ones gone
            var collapsedHexInfo = hexes.Collapse(columns);
            //create new ones
            var newHexInfo = CreateNewHexInSpecificColumns(columns);

            int maxDistance = Mathf.Max(collapsedHexInfo.MaxDistance, newHexInfo.MaxDistance);

            MoveAndAnimate(newHexInfo.AlteredHex, maxDistance);
            MoveAndAnimate(collapsedHexInfo.AlteredHex, maxDistance);

            //will wait for both of the above animations
            yield return new WaitForSeconds(Constants.MoveAnimationMinDuration * maxDistance);

            //search if there are matches with the new/collapsed items
            totalMatches = hexes.GetMatches(collapsedHexInfo.AlteredHex).Union(hexes.GetMatches(newHexInfo.AlteredHex)).Distinct();

            timesRun++;
        }
    state = GameState.None;
    }

    private void DestroyAllHexes()
    {
        for (int row = 0; row < Constants.MaxRows; row++)
        {
            for (int column = 0; column < Constants.MaxCols; column++)
            {
                Destroy(hexes[row, column]);
            }
        }
    }

    private AlteredHexInfo CreateNewHexInSpecificColumns(IEnumerable<int> columnsWithMissingCandy)
    {
        AlteredHexInfo newCandyInfo = new AlteredHexInfo();

        //find how many null values the column has
        foreach (int column in columnsWithMissingCandy)
        {
            var emptyItems = hexes.GetEmptyItemsOnColumn(column);
            foreach (var item in emptyItems)
            {
                GameObject go = GetRandomHex();
                GameObject newHex = Instantiate(go, spawnPositions[column], Quaternion.identity) as GameObject;
                newHex.transform.SetParent(grid.transform);

                newHex.GetComponent<Hex>().Assign(go.GetComponent<Hex>().type, item.Row, item.Column);

                if (Constants.MaxRows - item.Row > newCandyInfo.MaxDistance)
                    newCandyInfo.MaxDistance = Constants.MaxRows - item.Row;

                hexes[item.Row, item.Column] = newHex;
                newCandyInfo.AddHex(newHex);
            }
        }
        return newCandyInfo;
    }

    private void MoveAndAnimate(IEnumerable<GameObject> movedGameObjects, int distance)
    {
        foreach (var item in movedGameObjects)
        {
            float xPosition = item.GetComponent<Hex>().Col * Constants.GridOffset;
            float yPosition = item.GetComponent<Hex>().Row * Constants.HexSize.y;
            if (item.GetComponent<Hex>().Col % 2 != 0)
            {
                yPosition += 0.5f;
            }
            StartCoroutine(Util.LerpToPoint(item, gridPosition + new Vector2(xPosition, yPosition), Constants.MoveAnimationMinDuration * distance));
        }
    }

    private void RemoveFromScene(GameObject item)
    {
        //GameObject explosion = GetRandomExplosion();
        //var newExplosion = Instantiate(explosionPrefab, item.transform.position, Quaternion.identity) as GameObject;
        //Destroy(newExplosion, Constants.ExplosionDuration);
        Destroy(item);
    }

    //private GameObject GetRandomExplosion()
    //{
    //    return ExplosionPrefabs[Random.Range(0, explosionPrefab.Length)];
    //}

    public void UpdateHighlighter(Vector3 pos)
    {
        highlighter.transform.position = pos;
    }

    public void ResetHighlighterPosition()
    {
        highlighter.transform.position = highlighterDefaultPosition;
    }

    private void FixSortingLayer(GameObject hitGo, GameObject hitGo2)
    {
        SpriteRenderer sp1 = hitGo.GetComponent<SpriteRenderer>();
        SpriteRenderer sp2 = hitGo2.GetComponent<SpriteRenderer>();
        if (sp1.sortingOrder <= sp2.sortingOrder)
        {
            sp1.sortingOrder = 1;
            sp2.sortingOrder = 0;
        }
    }
}
