﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScores : MonoBehaviour
{
    const string privateCode = "eYKNAHiiYk-wuLcujd6QBQIeJCJc7YJEygBLuKhxOMLA";
    const string publicCode = "594d32bf758d2603446cde29";
    const string webURL = "https://dreamlo.com/lb/";

    public Highscore[] highscoreList;
    DisplayHighScores highscoresDisplay;
    public GameObject leaderboard;
    public GameObject username;

    void Awake()
    {
        highscoresDisplay = GetComponent<DisplayHighScores>();
    }


    public void AddNewHighScore(string username, int score)
    {
        StartCoroutine(UploadNewHighScore(username, score));
    }

    IEnumerator UploadNewHighScore(string username, int score)
    {
        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
            print("Upload Successful");
        else
        {
            print("Error uploading:" + www.error);
        }
    }

    public void DownloadHighScores()
    {
        StartCoroutine("DownloadHighscoresFromDatabase");
    }

    public void showHighScores()
    {
        DownloadHighScores();
        leaderboard.SetActive(true);
    }

    public void hideHighScores()
    {
        leaderboard.SetActive(false);
    }

    IEnumerator DownloadHighscoresFromDatabase()
    {
        WWW www = new WWW(webURL + publicCode + "/pipe/");
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            FormatHighscores(www.text);
            highscoresDisplay.OnHighscoresDownloaded(highscoreList);
        }
        else
        {
            print("Error downloading scores");
        }


    }

    void FormatHighscores(string textStream)
    {
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        highscoreList = new Highscore[entries.Length];
        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            highscoreList[i] = new Highscore(username, score);
        }
    }
}

public struct Highscore
{
    public string username;
    public int score;

    public Highscore(string _username, int _score)
    {
        username = _username;
        score = _score;
    }
}
