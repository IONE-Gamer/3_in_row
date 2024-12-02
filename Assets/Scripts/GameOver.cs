using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public string filePath = "leaderboard.txt"; // ���� � ����� � ���������
    public int maxEntries = 10; // ������������ ���������� ������� � �������
    public InputField playerNameInput; // ���� ��� ����� ����� ������
    public Button saveScoreButton; // ������ ��� ���������� �������
    public Button back; // ������ ��� ����������� � ������� ����
    public GameObject addToLeaderBoard; // UI-������ ��� ����������� ����� ����� �����
    public GameManager gameManager; // ������ �� ������ GameManager ��� ��������� ������� �����
    private List<LeaderBoardEntry> entries = new List<LeaderBoardEntry>(); // ������ ������� ������� �������

    void Start()
    {
        LoadLeaderBoard();
        saveScoreButton.onClick.AddListener(SavePlayerScore);
        back.onClick.AddListener(Back);

        // ���������, ����� �� ����� ������ � ������� �������
        if (entries.Count < maxEntries || gameManager.bestScore >= entries[entries.Count - 1].score)
        {
            addToLeaderBoard.SetActive(true);
            Debug.Log($"���� {gameManager.bestScore} ����� � ���-3!");
            back.gameObject.SetActive(false);
        }
        else
        {
            addToLeaderBoard.SetActive(false);
            Debug.Log($"���� {gameManager.bestScore} �� ����� � ���-3.");
        }
    }

    // ����� ��� ���������� �������
    public void SavePlayerScore()
    {
        string playerName = playerNameInput.text.Trim(); // �������� ��������� ���
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("��� �� ����� ���� ������!");
            return;
        }

        AddNewScore(playerName, gameManager.bestScore); // ��������� ����� ������
        playerNameInput.text = ""; // ������� ���� �����
    }

    // ����� ��� ���������� ������ �������
    public void AddNewScore(string playerName, int score)
    {
        entries.Add(new LeaderBoardEntry(playerName, score));
        entries.Sort((a, b) => b.score.CompareTo(a.score)); // ��������� �� �������� �����

        // ������������ ������
        if (entries.Count > maxEntries)
        {
            entries.RemoveAt(entries.Count - 1);
        }

        SaveLeaderBoard(); // ��������� ���������
    }

    // �������� ������� ������� �� �����
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

    // ���������� ������� ������� � ����
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

    // ��������� ������� ������� (��������, ��� �����������)
    public List<LeaderBoardEntry> GetLeaderBoard()
    {
        return new List<LeaderBoardEntry>(entries);
    }

    void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}

// ����� ��� ������������� ������ � ������� �������
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
