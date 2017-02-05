using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    public static GameController game;

    public GameData data;
    public SaveAndLoad saveAndLoad;
    public GameObject highlighter;

    public Hex.Type[,] grid;
    public Hex.Type type;
    public List<GameObject> hexes;

    public Text FoodText;
    public Text StoneText;
    public Text WoodText;

    public int matches;
    public const int maxColumns = 6;
    public const int maxRows = 8;
    int row;
    int col;
    public List<Vector2> upToRemoval;
    public bool needForUpdate = false;

    void Awake()
    {
        if (game == null)
        {
            DontDestroyOnLoad(gameObject);
            game = this;
        }
        else if (game != this)
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
        grid = new Hex.Type[maxRows, maxColumns];
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
        FoodText.text = data.currentFood + " / " + data.maxFood;
        StoneText.text = data.currentStone + " / " + data.maxStone;
        WoodText.text = data.currentWood + " / " + data.maxWood;
    }	
	
	void Update ()
    {

        if (needForUpdate)
        {
            FallDownHexes();
        }
	}

    void CheckForMatches()
    {

        CheckForStraight();
        CheckForDownToUp();
        CheckForUpToDown();
    }

    public void FallDownHexes()
    {
        for(row = 1; row < maxRows; row++)
        {
            for (col = 0; col < maxColumns; col++)
            {
                
            }
        }
    }

    void CheckForStraight()
    {
        for (col = 0; col < maxColumns; col++)
        {
            matches = 0;
            type = Hex.Type.Empty;
            for (row = 0; row < maxRows; row++)
            {
                if (grid[row, col] == type)
                {
                    matches++;
                    upToRemoval.Add(new Vector2(row, col));
                }
                if (grid[row, col] != type || row == maxRows - 1)
                {
                    if (matches >= 3)
                    {
                        RemoveMatches(type, upToRemoval);
                    }
                    type = grid[row, col];
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
            if (grid[n, col] == type)
            {
                matches++;
                upToRemoval.Add(new Vector2(n, col));
            }
            if (grid[n, col] != type || n == maxRows - 1)
            {
                if (matches >= 3)
                {
                    RemoveMatches(type, upToRemoval);
                }
                type = grid[n, col];
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
                if (grid[n, col] == type)
                {
                    matches++;
                    upToRemoval.Add(new Vector2(n, col));
                }
                if (grid[n, col] != type || n == maxRows - 1)
                {
                    if (matches >= 3)
                    {
                        RemoveMatches(type, upToRemoval);
                    }
                    type = grid[n, col];
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
    
    public void UpdateArrays()
    {
        int n = 0;
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxColumns; j++)
            {
                grid[i, j] = hexes[n].GetComponent<Hex>().type;
                n++;
            }
        }
        CheckForMatches();
    }

    public void RemoveMatches(Hex.Type type, List<Vector2> remove)
    {
        data.Increment(type);
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
                        Destroy(hex);
                    }
                }
            }
        }

        foreach (GameObject item in removalList)
        {
            hexes.Remove(item);
        }

    }
}
