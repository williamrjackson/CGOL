using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGOL : MonoBehaviour
{
    public int rows = 64;
    public int columns = 64;
    public int rate = 24;
    public SpriteRenderer unitPrototype;

    private bool[,] board;
    private SpriteRenderer[,] sprites;
    private Coroutine mainLoop;

    // Start is called before the first frame update
    void Start()
    {
        // Set up board...
        board = new bool[rows, columns];
        sprites = new SpriteRenderer[rows, columns];
        Transform boardParent = unitPrototype.transform.parent;

        float xOffset = (-rows * .5f) + .5f;
        float yOffset = (-columns * .5f) + .5f;
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                sprites[x, y] = Instantiate(unitPrototype);
                sprites[x, y].transform.localPosition = new Vector3(xOffset + x, yOffset + y, 0f);
                sprites[x, y].transform.parent = boardParent;
                sprites[x, y].gameObject.SetActive(true);
                board[x, y] = (Random.Range(0, 2) == 1);
            }
        }
        float boardScaleValue = 0.08f;
        boardParent.localScale = Vector3.one * boardScaleValue;

        StartWithRandom();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartWithRandom();
        }
    }
    private void StartWithRandom()
    {
       for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                board[x, y] = (Random.Range(0, 2) == 1);
            }
        }    
        DrawBoard();
        // Run
        if (mainLoop != null)
        {
            StopCoroutine(mainLoop);
            mainLoop = null;
        }
        mainLoop = StartCoroutine(Run(1f / rate));
    }
    
    private IEnumerator Run(float delay)
    {
        while (true)
        {
            bool[,] next = new bool[rows, columns];
            for (int x = 1; x < rows - 1; x++)
            {
                for (int y = 1; y < columns - 1; y++)
                {
                    int neighbors = CountNeighbors(x, y);
                    next[x, y] = RuleOfLife(board[x, y], neighbors);
                }
            }
            board = next;
            DrawBoard();
            yield return new WaitForSeconds(delay);
        }
    }
    private void DrawBoard()
    {
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                sprites[x, y].color = (board[x, y]) ? Color.white : Color.black;
            }
        }
    }
    private int CountNeighbors(int x, int y)
    {
        int neighbors = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                neighbors += (board[x + i, y + j]) ? 1 : 0;
            }
        }
        // exclude self
        neighbors -= (board[x, y]) ? 1 : 0;
        return neighbors;
    }
    private bool RuleOfLife(bool status, int neighbors)
    {
        if (status && neighbors > 3)
        {
            return false;
        }
        else if (status && neighbors < 2)
        {
            return false;
        }
        else if (!status && neighbors == 3)
        {
            return true;
        }
        else
        {
            return status;
        }
    }
}
