using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public string filePath = "leaderboard.txt"; // ���� � ����� � ���������
    public int maxEntries = 3; // ������������ ���������� ������� � �������
    public GameObject leaderBoard; // UI-������ ��� ����������� ������� �������
    public Text leaderBoardText; // UI-������� ��� ����������� ������� �������
    public Button newGame;// ������ ��� ������ ����� ����
    public Button openBoard; // ������ ��� �������� ������� �������
    public Button closeBoard; // ������ ��� �������� ������� �������
    public Button exit; // ������ ��� ������ �� ����
    private List<LeaderBoardEntry> entries = new List<LeaderBoardEntry>();  // ������ ������� ������� �������

    void Start()
    {
        // �������� ������� � �������
        newGame.onClick.AddListener(NewGame);
        openBoard.onClick.AddListener(OpenBoard);
        closeBoard.onClick.AddListener(Back);
        exit.onClick.AddListener(QuitGame);
    }

    // ����� ��� ������ ����� ����
    void NewGame()
    {
        SceneManager.LoadScene("Game"); // ��������� ����� ����
    }

    // ����� ��� �������� ������� �������
    void OpenBoard()
    {
        leaderBoard.SetActive(true); // ���������� UI-������ ������� �������
        LoadLeaderBoard(); // ��������� ������� ������� �� �����
        UpdateLeaderBoardUI(); // ��������� ����������� ������� �������
    }

    // ����� ��� �������� ������� �������
    void Back()
    {
        leaderBoard.SetActive(false); // ������������ UI-������ ������� �������
    }

    // ����� ��� ������ �� ����
    void QuitGame()
    {
        Application.Quit(); // �������� ����������
        Debug.Log("����� �� ����");
    }

    // �������� ������� ������� �� �����
    void LoadLeaderBoard()
    {
        entries.Clear(); // ������� ������� ������

        if (File.Exists(filePath)) // ���������, ���������� �� ����
        {
            string[] lines = File.ReadAllLines(filePath); // ������ ��� ������ �� �����

            // ������������ ������ ������, �������� ��� ������ � ��� ����
            foreach (string line in lines)
            {
                string[] parts = line.Split(':'); 
                if (parts.Length == 2 && int.TryParse(parts[1], out int score)) // ��������� ������������ ������
                {
                    entries.Add(new LeaderBoardEntry(parts[0], score)); // ��������� ������ � ������
                }
            }
        }

        Debug.Log("������� ���������");
    }

    // ���������� UI
    void UpdateLeaderBoardUI()
    {
        if (leaderBoardText == null) return; // ���������, ���� �� ������ �� UI

        leaderBoardText.text = "������� �������:\n";

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            leaderBoardText.text += $"{i + 1}. {entry.playerName} - {entry.score}\n";
        }
    }

    // ��������� ������� �������
    public List<LeaderBoardEntry> GetLeaderBoard()
    {
        return new List<LeaderBoardEntry>(entries); // ���������� ����� ������ �������
    }
}