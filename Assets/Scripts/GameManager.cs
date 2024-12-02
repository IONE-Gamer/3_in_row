using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int rows = 8; // ���������� �����
    public int cols = 8; // ���������� �������
    public GameObject tilePrefab; // ������ ��� ���� ������
    public Sprite[] tileSprites; // ������ �������� ��� ������
    private GameObject[,] grid; // ������ ��� �������� ������ �� ��������

    public Text scoreText; // ������ �� UI ��� ����������� �����
    public Text highScoreText; // ������ �� UI ��� ����������� �������
    public int currentScore = 10; // ������� ����
    public int bestScore = 0; // ������
    public int T = 0; // ���������� �����

    public GameObject gameOverObj; // ������ �� LeaderBoard
    public Text yourScore;  // UI ��� ����������� �������� ����� � ����� ����

    void Start()
    {
        UpdateScoreDisplay(); // ���������� UI

        // ������������� �������� ����
        grid = new GameObject[rows, cols]; // ������� +1, ��� ��� ���� �� ��������� � �������������� ������

        GenerateGameBoard();

        gameOverObj.SetActive(false);
    }

    // ����� ��� ��������� �������� ����
    void GenerateGameBoard()
    {
        // ���� ��� �������� �������� ����
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // �������� ������ � ��������� ���������� �������
                grid[row, col] = CreateTile(row, col);
            }
        }
    }

    // �������� ����� ������
    GameObject CreateTile(int row, int col)
    {
        Vector3 position = new Vector3(col, row, 0);

        // ������� ������
        GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);

        // ��������� ��������� ������
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (tileSprites.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = tileSprites[Random.Range(0, tileSprites.Length)];
        }

        return tile;
    }

    // �������� ��� �������� ������� ������
    IEnumerator CheckTilesAndFall()
    {
        // ���� ��� ������ �������
        for (int col = 0; col < cols; col++)
        {
            // ������� ����� � ������� �������
            for (int row = rows - 1; row >= 1; row--)
            {
                if (grid[row, col] != null && grid[row - 1, col] == null)
                {
                    // ���������� ����� ����
                    StartCoroutine(MoveTileDown(row, col));
                    yield return new WaitForSeconds(0.1f); // ���� ����� ��� �������
                }
            }

            // ������� ������, ����������� �� ��������� ���� (���� ��� ���� ����������)
            for (int row = rows; row < grid.GetLength(0); row++)
            {
                if (grid[row, col] != null && grid[row - 1, col] == null)
                {
                    // ���������� ����� ����
                    StartCoroutine(MoveTileDown(row, col));
                    yield return new WaitForSeconds(0.1f); // ���� ����� ��� �������
                }
            }
        }

        // ����� ���������� ���� ������� ��������� �����
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(CheckTilesAndFall());
    }



    // �������� ��� ����������� ������ ����
    IEnumerator MoveTileDown(int row, int col, GameObject tile = null)
    {
        if (tile == null)
        {
            tile = grid[row, col];
            grid[row, col] = null; // ����������� ������ ������
        }

        // ����� ������� �����
        Vector3 startPosition = tile.transform.position;
        Vector3 endPosition = new Vector3(col, row - 1, 0);

        // ����������� ����� � ������� ������ �� ������ ��������
        grid[row - 1, col] = tile;

        // �������� �������
        float elapsedTime = 0f;
        float moveDuration = 0.2f;

        while (elapsedTime < moveDuration)
        {
            tile.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��������� �������
        tile.transform.position = endPosition;
    }

    //-----------------------------------------------����������-----------------------------------------------

    void Update()
    {
        if (currentScore != 0)
        {
            // ��� �������: ������ �� ��������
            if (Input.GetMouseButtonDown(0)) // ��� ���. ��������� ����������� Touch
            {
                // �������� ������� ���� � ������� �����������
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0; // ������� ���� �� ��������� 2D

                // ������ ��� �� ������� ���� � ������� ����
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    GameObject selectedTile = hit.collider.gameObject; // �������� ������, �� ������� ������� ���
                    Vector2 gridPos = new Vector2(Mathf.Floor(hit.collider.transform.position.x), Mathf.Floor(hit.collider.transform.position.y));

                    if (gridPos.x >= 0 && gridPos.x < cols && gridPos.y >= 0 && gridPos.y < rows)
                    {
                        HandleTileSelection((int)gridPos.y, (int)gridPos.x, selectedTile); // ������������ ����� ������ ��� ��������
                    }
                }
            }
        }
        else
        {
            gameOverObj.SetActive(true);
            yourScore.text = "��� ���� - " + bestScore;
        }
    }

    // ��������� ������ ������
    void HandleTileSelection(int row, int col, GameObject selectedTile)
    {
        Sprite tileSprite = selectedTile.GetComponent<SpriteRenderer>().sprite; // �������� ������ ��������� ������
        var connectedTiles = FindConnectedTiles(row, col, tileSprite);

        // ��������� ��� ��������� ������ ������ � ��������� �� �������
        foreach (var tile in connectedTiles)
        {
            ReuseTile(tile);
        }

        // ����������� ����
        currentScore += connectedTiles.Count - (3 + T);
        T = T + 1;

        if (currentScore < 0)
        {
            currentScore = 0;
        }

        // ��������� ������, ���� �����
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("������", bestScore); // ��������� ����� ������
        }

        UpdateScoreDisplay(); // ��������� ����������� �����

        // ��������� ���� (������ "������" ���� � ����������� �������)
        StartCoroutine(CheckTilesAndFall());
    }

    // ������� ����� ������ � ����������� �������
    void ReuseTile(GameObject tile)
    {
        int col = Mathf.FloorToInt(tile.transform.position.x); // ������� ������� �����
        int row = Mathf.FloorToInt(tile.transform.position.y); // ������ ������� �����

        // ������� ������� ����� �� �����
        grid[row, col] = null;

        // ������� ������ ��������� ������ ���� ������� �������
        int targetRow = FindFirstFreeRowAbove(col);

        // ��������� ����� � ��������� ������
        tile.transform.position = new Vector3(col, targetRow, 0);

        // ��������� ����� ��������� ������
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (tileSprites.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = tileSprites[Random.Range(0, tileSprites.Length)];
        }

        // ������ ����� � ����� �� ��������� �������
        grid[targetRow, col] = tile;
    }


    // ����� ��� ������ ������ ��������� ������ ���� ������� �������
    int FindFirstFreeRowAbove(int col)
    {
        int maxRows = rows + 10; // ������ ����� (�� ������, ���� ����� ����������� ������� ������)

        for (int row = rows; row < maxRows; row++)
        {
            if (row >= grid.GetLength(0))
            {
                ExpandGrid(); // ��������� �����, ���� ����� �� � �������
            }

            if (grid[row, col] == null) // ���� ������ ������
            {
                return row;
            }
        }

        return rows; // ���������� ����������� ��������, ���� ������ �� ����� (�� ������ �����������)
    }

    // ����� ��� ���������� ����� ��� �������������
    void ExpandGrid()
    {
        int newRows = grid.GetLength(0) + 10; // ����������� ���������� ����� �� 10
        GameObject[,] newGrid = new GameObject[newRows, cols];

        // �������� ������ ����� � �����
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < cols; col++)
            {
                newGrid[row, col] = grid[row, col];
            }
        }

        grid = newGrid; // ��������� ������ �� �����
    }



    // ����� ���� ������ � ���������� �����, ������� ��������� � �������� �������
    private List<GameObject> FindConnectedTiles(int row, int col, Sprite sprite)
    {
        List<GameObject> connectedTiles = new List<GameObject>();
        bool[,] visited = new bool[rows, cols];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        stack.Push(new Vector2Int(row, col));
        visited[row, col] = true;

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            GameObject currentTile = grid[current.x, current.y];

            // ���������, ��� ������ ����� ������ ������
            if (currentTile != null && currentTile.GetComponent<SpriteRenderer>().sprite == sprite)
            {
                connectedTiles.Add(currentTile);

                // ������� ������� (�� ����������� � ���������)
                foreach (Vector2Int direction in new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) })
                {
                    int newRow = current.x + direction.x;
                    int newCol = current.y + direction.y;

                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols && !visited[newRow, newCol])
                    {
                        visited[newRow, newCol] = true;
                        stack.Push(new Vector2Int(newRow, newCol));
                    }
                }
            }
        }

        return connectedTiles;
    }


    //-----------------------------------------------�ר�-----------------------------------------------

    // ���������� ����������� ����� � ������� �� ������
    void UpdateScoreDisplay()
    {
        scoreText.text = "����: " + currentScore; // ���������� ������� ����
        highScoreText.text = "������: " + bestScore; // ���������� ������
    }
}