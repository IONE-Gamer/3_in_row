using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public string filePath = "leaderboard.txt"; // Путь к файлу с рекордами
    public int maxEntries = 3; // Максимальное количество записей в таблице
    public GameObject leaderBoard; // UI-объект для отображения таблицы лидеров
    public Text leaderBoardText; // UI-элемент для отображения таблицы лидеров
    public Button newGame;// Кнопка для начала новой игры
    public Button openBoard; // Кнопка для открытия таблицы лидеров
    public Button closeBoard; // Кнопка для закрытия таблицы лидеров
    public Button exit; // Кнопка для выхода из игры
    private List<LeaderBoardEntry> entries = new List<LeaderBoardEntry>();  // Список записей таблицы лидеров

    void Start()
    {
        // Привязка событий к кнопкам
        newGame.onClick.AddListener(NewGame);
        openBoard.onClick.AddListener(OpenBoard);
        closeBoard.onClick.AddListener(Back);
        exit.onClick.AddListener(QuitGame);
    }

    // Метод для начала новой игры
    void NewGame()
    {
        SceneManager.LoadScene("Game"); // Загружаем сцену игры
    }

    // Метод для открытия таблицы лидеров
    void OpenBoard()
    {
        leaderBoard.SetActive(true); // Активируем UI-объект таблицы лидеров
        LoadLeaderBoard(); // Загружаем таблицу лидеров из файла
        UpdateLeaderBoardUI(); // Обновляем отображение таблицы лидеров
    }

    // Метод для закрытия таблицы лидеров
    void Back()
    {
        leaderBoard.SetActive(false); // Деактивируем UI-объект таблицы лидеров
    }

    // Метод для выхода из игры
    void QuitGame()
    {
        Application.Quit(); // Закрытие приложения
        Debug.Log("Выход из игры");
    }

    // Загрузка таблицы лидеров из файла
    void LoadLeaderBoard()
    {
        entries.Clear(); // Очищаем текущие записи

        if (File.Exists(filePath)) // Проверяем, существует ли файл
        {
            string[] lines = File.ReadAllLines(filePath); // Читаем все строки из файла

            // Обрабатываем каждую строку, разделяя имя игрока и его очки
            foreach (string line in lines)
            {
                string[] parts = line.Split(':'); 
                if (parts.Length == 2 && int.TryParse(parts[1], out int score)) // Проверяем корректность данных
                {
                    entries.Add(new LeaderBoardEntry(parts[0], score)); // Добавляем запись в список
                }
            }
        }

        Debug.Log("Таблица загружена");
    }

    // Обновление UI
    void UpdateLeaderBoardUI()
    {
        if (leaderBoardText == null) return; // Проверяем, есть ли ссылка на UI

        leaderBoardText.text = "Таблица лидеров:\n";

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            leaderBoardText.text += $"{i + 1}. {entry.playerName} - {entry.score}\n";
        }
    }

    // Получение таблицы лидеров
    public List<LeaderBoardEntry> GetLeaderBoard()
    {
        return new List<LeaderBoardEntry>(entries); // Возвращаем копию списка лидеров
    }
}