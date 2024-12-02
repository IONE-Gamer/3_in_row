using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int rows = 8; // Количество строк
    public int cols = 8; // Количество колонок
    public GameObject tilePrefab; // Префаб для всех плиток
    public Sprite[] tileSprites; // Массив спрайтов для плиток
    private GameObject[,] grid; // Массив для хранения ссылок на элементы

    public Text scoreText; // Ссылка на UI для отображения счета
    public Text highScoreText; // Ссылка на UI для отображения рекорда
    public int currentScore = 10; // Текущий счет
    public int bestScore = 0; // Рекорд
    public int T = 0; // количество ходов

    public GameObject gameOverObj; // Ссылка на LeaderBoard
    public Text yourScore;  // UI для отображения текущего счета в конце игры

    void Start()
    {
        UpdateScoreDisplay(); // Обновление UI

        // Инициализация игрового поля
        grid = new GameObject[rows, cols]; // Убираем +1, так как поле не нуждается в дополнительной строке

        GenerateGameBoard();

        gameOverObj.SetActive(false);
    }

    // Метод для генерации игрового поля
    void GenerateGameBoard()
    {
        // Цикл для создания игрового поля
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Создание плитки и установка случайного спрайта
                grid[row, col] = CreateTile(row, col);
            }
        }
    }

    // Создание одной плитки
    GameObject CreateTile(int row, int col)
    {
        Vector3 position = new Vector3(col, row, 0);

        // Создаем префаб
        GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);

        // Назначаем случайный спрайт
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (tileSprites.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = tileSprites[Random.Range(0, tileSprites.Length)];
        }

        return tile;
    }

    // Корутина для проверки падения плиток
    IEnumerator CheckTilesAndFall()
    {
        // Цикл для каждой колонки
        for (int col = 0; col < cols; col++)
        {
            // Падение фишек в текущей колонке
            for (int row = rows - 1; row >= 1; row--)
            {
                if (grid[row, col] != null && grid[row - 1, col] == null)
                {
                    // Перемещаем фишку вниз
                    StartCoroutine(MoveTileDown(row, col));
                    yield return new WaitForSeconds(0.1f); // Даем время для падения
                }
            }

            // Падение плиток, находящихся за пределами поля (если они были перемещены)
            for (int row = rows; row < grid.GetLength(0); row++)
            {
                if (grid[row, col] != null && grid[row - 1, col] == null)
                {
                    // Перемещаем фишку вниз
                    StartCoroutine(MoveTileDown(row, col));
                    yield return new WaitForSeconds(0.1f); // Даем время для падения
                }
            }
        }

        // После заполнения всех колонок проверяем снова
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(CheckTilesAndFall());
    }



    // Корутина для перемещения плитки вниз
    IEnumerator MoveTileDown(int row, int col, GameObject tile = null)
    {
        if (tile == null)
        {
            tile = grid[row, col];
            grid[row, col] = null; // Освобождаем старую клетку
        }

        // Новая позиция фишки
        Vector3 startPosition = tile.transform.position;
        Vector3 endPosition = new Vector3(col, row - 1, 0);

        // Привязываем фишку к целевой клетке до начала анимации
        grid[row - 1, col] = tile;

        // Анимация падения
        float elapsedTime = 0f;
        float moveDuration = 0.2f;

        while (elapsedTime < moveDuration)
        {
            tile.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Финальная позиция
        tile.transform.position = endPosition;
    }

    //-----------------------------------------------УПРАВЛЕНИЕ-----------------------------------------------

    void Update()
    {
        if (currentScore != 0)
        {
            // Для отладки: следим за курсором
            if (Input.GetMouseButtonDown(0)) // Для моб. устройств используйте Touch
            {
                // Получаем позицию мыши в мировых координатах
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0; // Позиция мыши на плоскости 2D

                // Строим луч из позиции мыши в игровом мире
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    GameObject selectedTile = hit.collider.gameObject; // Получаем объект, на который попадал луч
                    Vector2 gridPos = new Vector2(Mathf.Floor(hit.collider.transform.position.x), Mathf.Floor(hit.collider.transform.position.y));

                    if (gridPos.x >= 0 && gridPos.x < cols && gridPos.y >= 0 && gridPos.y < rows)
                    {
                        HandleTileSelection((int)gridPos.y, (int)gridPos.x, selectedTile); // Обрабатываем выбор плитки без корутины
                    }
                }
            }
        }
        else
        {
            gameOverObj.SetActive(true);
            yourScore.text = "Ваш счет - " + bestScore;
        }
    }

    // Обработка выбора плитки
    void HandleTileSelection(int row, int col, GameObject selectedTile)
    {
        Sprite tileSprite = selectedTile.GetComponent<SpriteRenderer>().sprite; // Получаем спрайт выбранной плитки
        var connectedTiles = FindConnectedTiles(row, col, tileSprite);

        // Переносим все найденные плитки наверх и обновляем их спрайты
        foreach (var tile in connectedTiles)
        {
            ReuseTile(tile);
        }

        // Увеличиваем счёт
        currentScore += connectedTiles.Count - (3 + T);
        T = T + 1;

        if (currentScore < 0)
        {
            currentScore = 0;
        }

        // Обновляем рекорд, если нужно
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("Рекорд", bestScore); // Сохраняем новый рекорд
        }

        UpdateScoreDisplay(); // Обновляем отображение счета

        // Обновляем поле (плитки "падают" вниз и заполняются пустоты)
        StartCoroutine(CheckTilesAndFall());
    }

    // Перенос фишки наверх с обновлением спрайта
    void ReuseTile(GameObject tile)
    {
        int col = Mathf.FloorToInt(tile.transform.position.x); // Колонка текущей фишки
        int row = Mathf.FloorToInt(tile.transform.position.y); // Строка текущей фишки

        // Убираем текущую фишку из сетки
        grid[row, col] = null;

        // Находим первую свободную строку выше верхней границы
        int targetRow = FindFirstFreeRowAbove(col);

        // Переносим фишку в найденную строку
        tile.transform.position = new Vector3(col, targetRow, 0);

        // Назначаем новый случайный спрайт
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (tileSprites.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = tileSprites[Random.Range(0, tileSprites.Length)];
        }

        // Ставим фишку в сетку на найденной позиции
        grid[targetRow, col] = tile;
    }


    // Метод для поиска первой свободной строки выше верхней границы
    int FindFirstFreeRowAbove(int col)
    {
        int maxRows = rows + 10; // Задаем буфер (на случай, если фишки поднимаются слишком высоко)

        for (int row = rows; row < maxRows; row++)
        {
            if (row >= grid.GetLength(0))
            {
                ExpandGrid(); // Расширяем сетку, если вышли за её пределы
            }

            if (grid[row, col] == null) // Если ячейка пустая
            {
                return row;
            }
        }

        return rows; // Возвращаем стандартное значение, если ничего не нашли (не должно происходить)
    }

    // Метод для расширения сетки при необходимости
    void ExpandGrid()
    {
        int newRows = grid.GetLength(0) + 10; // Увеличиваем количество строк на 10
        GameObject[,] newGrid = new GameObject[newRows, cols];

        // Копируем старую сетку в новую
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < cols; col++)
            {
                newGrid[row, col] = grid[row, col];
            }
        }

        grid = newGrid; // Обновляем ссылку на сетку
    }



    // Поиск всех плиток с одинаковым тегом, которые соединены с исходной плиткой
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

            // Проверяем, что плитка имеет нужный спрайт
            if (currentTile != null && currentTile.GetComponent<SpriteRenderer>().sprite == sprite)
            {
                connectedTiles.Add(currentTile);

                // Смотрим соседей (по горизонтали и вертикали)
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


    //-----------------------------------------------СЧЁТ-----------------------------------------------

    // Обновление отображения счета и рекорда на экране
    void UpdateScoreDisplay()
    {
        scoreText.text = "Счёт: " + currentScore; // Отображаем текущий счет
        highScoreText.text = "Рекорд: " + bestScore; // Отображаем рекорд
    }
}