using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    public static GameController Instance;

    public GameData data;
    public CreateGrid createGrid;

    public SaveAndLoad saveAndLoad;
    public GameObject highlighter;

    public Hex.Type type;
    public GameObject[,] hexes;

    public int matches;
    public const int maxColumns = 6;
    public const int maxRows = 8;
    int row;
    int col;
    public List<Vector2> upToRemoval;
    public bool needForUpdate = false;

    public float hexFallSpeed = 0.1f; 

    void Awake()
    {
        hexes = new GameObject[maxRows, maxColumns];
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        data = new GameData();
        saveAndLoad = new SaveAndLoad();
        if (File.Exists(Application.persistentDataPath + "/data.dat"))
        {
            LoadGame();
        }
        UpdateData();
        createGrid.InitGrid();
    }

    public void SaveGame()
    {
        saveAndLoad.SaveGame(data);
    }

    public void LoadGame()
    {
        data = saveAndLoad.LoadGame();
    }

    void UpdateData()
    {
        data.UpdateData();
    }	
	
	void Update ()
    {

        if (needForUpdate)
        {
            FindHexToFall();
            StartCoroutine(HexesHaveReachedTarget());
            needForUpdate = false;
        }
	}

    public void CheckForMatches()
    {

        CheckForStraight();
        CheckForDownToUp();
        CheckForUpToDown();
    }

    public void FindHexToFall()
    {
        for (col = 0; col < maxColumns; col++)
        {
            GetTargetHexes();
            SpawnNewHexes();
            FallDownHexes();
        }
    }

    private void SpawnNewHexes()
    {
        for (col = 0; col < maxColumns; col++)
        {
            for (row = 0; row < maxRows; row++)
            {
                if(hexes[row, col].GetComponent<Hex>().toBeFilled)
                {
                    hexes[row, col] = Instantiate(createGrid.SpawnHex(hexes[row, col].transform.position));
                    hexes[row, col].SetActive(true);
                    hexes[row, col].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                }
            }
        }
    }

    private void GetTargetHexes()
    {
        int hexTarget = 0;
        int swapTarget = maxRows - 1;

        for (int row = 0; row < maxRows; row++)
        {
            if(!hexes[row, col].GetComponent<Hex>().toBeFilled)
            {
                hexes[row, col].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
                hexes[row, col].GetComponent<Hex>().shouldFall = true;
                hexes[row, col].GetComponent<Hex>().targetPosition = hexes[hexTarget, col].transform.position;
                hexes[row, col].GetComponent<Hex>().targetRow = hexTarget;
                hexTarget++;
            }
        }

        for (int row = 0; row < maxRows; row++)
        {
            if (hexes[row, col].GetComponent<Hex>().toBeFilled)
            {
                hexes[row, col].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                hexes[row, col].GetComponent<Hex>().targetPosition = hexes[swapTarget, col].transform.position;
                hexes[row, col].GetComponent<Hex>().targetRow = swapTarget;
                
                swapTarget--;
            }
        }
        this.row = maxRows;
    }

    private void FallDownHexes()
    {
        for (int row = 0; row < maxRows; row++)
        {
            if(hexes[row, col].GetComponent<Hex>().shouldFall)
            {
                StartCoroutine(FallDownHex(row, col));
            }
            else if (hexes[row, col].GetComponent<Hex>().toBeFilled)
            {
                hexes[row, col].transform.position = hexes[row, col].GetComponent<Hex>().targetPosition;
                hexes[row, col].GetComponent<Hex>().hasReachedTarget = true;
            }
        }
    }

    private IEnumerator FallDownHex(int tempRow, int col)
    {
        while (Vector3.Distance(hexes[tempRow, col].transform.position, hexes[tempRow, col].GetComponent<Hex>().targetPosition) > 0.02f)
        {
            hexes[tempRow, col].transform.position = Vector3.Lerp(hexes[tempRow, col].transform.position, hexes[tempRow, col].GetComponent<Hex>().targetPosition, hexFallSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        hexes[tempRow, col].transform.position = hexes[tempRow, col].GetComponent<Hex>().targetPosition;
        hexes[tempRow, col].GetComponent<Hex>().shouldFall = false;
        hexes[tempRow, col].GetComponent<Hex>().row = hexes[tempRow, col].GetComponent<Hex>().targetRow;
        hexes[tempRow, col].GetComponent<Hex>().hasReachedTarget = true;
        yield return null;
    }

    private IEnumerator HexesHaveReachedTarget()
    {
        bool reached = false;

        while (!reached)
        {
            reached = true;
            for (col = 0; col < maxColumns; col++)
            {
                for (row = 0; row < maxRows; row++)
                {
                    if (!hexes[row, col].GetComponent<Hex>().hasReachedTarget)
                    {
                        reached = false;
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("All hexes reached their target!");
        yield return null;
    }

    void CheckForStraight()
    {
        for (col = 0; col < maxColumns; col++)
        {
            matches = 0;
            type = Hex.Type.Empty;
            for (row = 0; row < maxRows; row++)
            {
                if (hexes[row, col].GetComponent<Hex>().type == type)
                {
                    matches++;
                    upToRemoval.Add(new Vector2(row, col));
                }
                if (hexes[row, col].GetComponent<Hex>().type != type || row == maxRows - 1)
                {
                    if (matches >= 3)
                    {
                        RemoveMatches(type, upToRemoval);
                    }
                    type = hexes[row, col].GetComponent<Hex>().type;
                    upToRemoval.Clear();
                    upToRemoval.Add(new Vector2(row, col));
                    matches = 1;
                }
            }
        }
    }

    void CheckForDownToUp()
    {
        int n = 0;
        matches = 0;
        type = Hex.Type.Empty;

        //Check for tiny section at bottom
        for (col = 2; col < 6; col++)
        {
            if((col % 2 == 0) && col != 2)
            {
                n++;
            }
            if (hexes[n, col].GetComponent<Hex>().type == type)
            {
                matches++;
                upToRemoval.Add(new Vector2(n, col));
            }
            if (hexes[n, col].GetComponent<Hex>().type != type || n == maxRows - 1)
            {
                if (matches >= 3)
                {
                    RemoveMatches(type, upToRemoval);
                }
                type = hexes[n, col].GetComponent<Hex>().type;
                upToRemoval.Clear();
                upToRemoval.Add(new Vector2(n, col));
                matches = 1;
            }
        }
        //Resuming to huge area check
        for (row = 0; row < maxRows; row++)
        {
            matches = 0;
            type = Hex.Type.Empty;
            n = row;
            for (col = 0; col < maxColumns; col++)
            {
                if((col % 2 == 0) && col != 0 && n < maxRows - 1)
                {
                    n++;
                }
                if (hexes[n, col].GetComponent<Hex>().type == type)
                {
                    matches++;
                    upToRemoval.Add(new Vector2(n, col));
                }
                if (hexes[n, col].GetComponent<Hex>().type != type || n == maxRows - 1)
                {
                    if (matches >= 3)
                    {
                        RemoveMatches(type, upToRemoval);
                    }
                    type = hexes[n, col].GetComponent<Hex>().type;
                    upToRemoval.Clear();
                    upToRemoval.Add(new Vector2(n, col));
                    matches = 1;
                }
            }
        }
    }

    void CheckForUpToDown()
    {
        int n = 0;
        matches = 0;
        type = Hex.Type.Empty;

        //Check for tiny section at bottom




        //Resuming to huge area check
    }

    public void UpdateHighlighter(Vector3 pos)
    {
        highlighter.transform.position = pos;
    }

    public void RemoveMatches(Hex.Type type, List<Vector2> remove)
    {
        data.IncrementResource(type);
        RemoveGameObjectHexes(remove);
        UpdateData();
        needForUpdate = true;
    }

    public void RemoveGameObjectHexes(List<Vector2> remove)
    {
        List<GameObject> removalList = new List<GameObject>();

        foreach (Vector2 pos in remove)
        {
            foreach (GameObject hex in hexes)
            {
                if (hex.GetComponent<Hex>().row == pos.x)
                {
                    if(hex.GetComponent<Hex>().column == pos.y)
                    {
                        removalList.Add(hex);
                        //hex.SetActive(false);
                        hex.GetComponent<Hex>().toBeFilled = true;
                    }
                }
            }
        }
    }
}
