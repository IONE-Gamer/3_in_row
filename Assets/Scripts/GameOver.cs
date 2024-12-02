using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public string filePath = "leaderboard.txt"; // Путь к файлу с рекордами
    public int maxEntries = 10; // Максимальное количество записей в таблице
    public InputField playerNameInput; // Поле для ввода имени игрока
    public Button saveScoreButton; // Кнопка для сохранения рекорда
    public Button back; // Кнопка для возвращения в главное меню
    public GameObject addToLeaderBoard; // UI-объект для отображения формы ввода имени
    public GameManager gameManager; // Ссылка на объект GameManager для получения лучшего счёта
    private List<LeaderBoardEntry> entries = new List<LeaderBoardEntry>(); // Список записей таблицы лидеров

    void Start()
    {
        LoadLeaderBoard();
        saveScoreButton.onClick.AddListener(SavePlayerScore);
        back.onClick.AddListener(Back);

        // Проверяем, попал ли новый рекорд в таблицу лидеров
        if (entries.Count < maxEntries || gameManager.bestScore >= entries[entries.Count - 1].score)
        {
            addToLeaderBoard.SetActive(true);
            Debug.Log($"Счёт {gameManager.bestScore} попал в топ-3!");
            back.gameObject.SetActive(false);
        }
        else
        {
            addToLeaderBoard.SetActive(false);
            Debug.Log($"Счёт {gameManager.bestScore} не попал в топ-3.");
        }
    }

    // Метод для сохранения рекорда
    public void SavePlayerScore()
    {
        string playerName = playerNameInput.text.Trim(); // Получаем введенное имя
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Имя не может быть пустым!");
            return;
        }

        AddNewScore(playerName, gameManager.bestScore); // Добавляем новый рекорд
        playerNameInput.text = ""; // Очищаем поле ввода
    }

    // Метод для добавления нового рекорда
    public void AddNewScore(string playerName, int score)
    {
        entries.Add(new LeaderBoardEntry(playerName, score));
        entries.Sort((a, b) => b.score.CompareTo(a.score)); // Сортируем по убыванию очков

        // Ограничиваем список
        if (entries.Count > maxEntries)
        {
            entries.RemoveAt(entries.Count - 1);
        }

        SaveLeaderBoard(); // Сохраняем изменения
    }

    // Загрузка таблицы лидеров из файла
    void LoadLeaderBoard()
    {
        entries.Clear();

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                {
                    entries.Add(new LeaderBoardEntry(parts[0], score));
                }
            }
        }
    }

    // Сохранение таблицы лидеров в файл
    void SaveLeaderBoard()
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var entry in entries)
            {
                writer.WriteLine($"{entry.playerName}:{entry.score}");
            }
        }

        SceneManager.LoadScene("Menu");
    }

    // Получение таблицы лидеров (например, для отображения)
    public List<LeaderBoardEntry> GetLeaderBoard()
    {
        return new List<LeaderBoardEntry>(entries);
    }

    void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}

// Класс для представления записи в таблице лидеров
[System.Serializable]
public class LeaderBoardEntry
{
    public string playerName;
    public int score;

    public LeaderBoardEntry(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}
