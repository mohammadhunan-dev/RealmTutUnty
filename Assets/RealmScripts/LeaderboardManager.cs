// Leaderboard Manager finds all your scores + displays them 
// FOR SYNC enabled:
// Leaderboard Manager listens for changes on all scores + displays top 5 scores

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Realms;
using System.Linq;
using Realms.Sync;

public class LeaderboardManager : MonoBehaviour
{
    private Realm realm;
    public ListView listView;
    public Button toggleButton;
    public Label displayTitle;
    public bool isUIVisible = true;
    public int currentPlayerHighestPoints = 0; // 0 til it's set
    public Realms.Sync.User user;

    public static LeaderboardManager Instance;


    // 
    void Awake()
    {
        Instance = this;
        Debug.Log("Awake");
    }

    // Start is called before the first frame update
    void Start()
    {
    }
    public async void setLoggedInUser(Realms.Sync.User loggedInUser)
    {
        user = loggedInUser;
        var syncConfiguration = new SyncConfiguration("UnityTutorialPartition", user);
        realm = await Realm.GetInstanceAsync(syncConfiguration);
        createLeaderboardGui();

        //getTopScores();
    }

    public void createLeaderboardGui()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        createLeaderboardUI();
        root.Add(toggleButton);
        root.Add(displayTitle);
        root.Add(listView);

        toggleButton.clicked += () =>
        {
            toggleUIVisible();
        };
    }

    public int getRealmPlayerTopScore()
    {
        var realmPlayer = realm.Find<PlayerModel>(user.Id);
        var realmPlayerTopScore = realmPlayer.Scores.OrderByDescending(s => s.Points).First().Points;
        return realmPlayer.Scores.OrderByDescending(s => s.Points).First().Points;
    }

    public void listenToScoreChanges()
    {

    }


    //public void getTopScores()
    //{
    //    //var topScores = realm.All<ScoreModel>().Where((e) => e.TokensCollected > 1 );

    //    var topScores = realm.All<ScoreModel>().OrderByDescending(s => s.Points).ToList();
    //    ///.First().Points;

    //    for (int i = 0; i < 5; i++)
    //    {
    //        Debug.Log("Score : " + topScores[i].Points + topScores[i].ScoreOwner.Name);
    //    };
    //}

    void toggleUIVisible()
    {
        if (isUIVisible == true)
        {
            // if ui is already visible, and the toggle button is pressed, then hide it
            displayTitle.RemoveFromClassList("visible");
            displayTitle.AddToClassList("hide");
            listView.RemoveFromClassList("visible");
            listView.AddToClassList("hide");
            isUIVisible = false;
        }
        else
        {
            // if ui is not visible, and the toggle button is pressed, then show it
            displayTitle.RemoveFromClassList("hide");
            displayTitle.AddToClassList("visible");
            listView.RemoveFromClassList("hide");
            listView.AddToClassList("visible");
            isUIVisible = true;
        }
    }
    void createLeaderboardUI()
    {
        // create toggle button
        toggleButton = new Button();
        toggleButton.text = "Toggle Leaderboard & Settings";
        toggleButton.AddToClassList("toggle-button");

        // create leaderboard title
        displayTitle = new Label();
        displayTitle.text = "Leaderboard:";
        displayTitle.AddToClassList("display-title");


        // Create a list of data. In this case, numbers from 1 to 1000.
        const int itemCount = 5;

        var topScoresListItems = new List<string>(itemCount);

        topScoresListItems.Add("Your top points: " + getRealmPlayerTopScore());

        var topScores = realm.All<ScoreModel>().OrderByDescending(s => s.Points).ToList();

        for (int i = 0; i < 5; i++)
        {
            if(topScores[i].Points > 1) // if there's not many players there may not be 5 top scores yet
            {
                topScoresListItems.Add($"{topScores[i].ScoreOwner.Name}: {topScores[i].Points} points");
            }
        };





        // Create a new label for each top score
        //var label = new Label();
        //label.AddToClassList("list-item-game-name-label");
        Func<VisualElement> makeItem = () => new Label();

        // Bind Scores to the UI
        Action<VisualElement, int> bindItem = (e, i) => {
            (e as Label).text = topScoresListItems[i];
            (e as Label).AddToClassList("list-item-game-name-label");
        };

        // Provide the list view with an explict height for every row
        // so it can calculate how many items to actually display
        const int itemHeight = 5;

        listView = new ListView(topScoresListItems, itemHeight, makeItem, bindItem);
        listView.AddToClassList("list-view");
    }

    // works for local
    //void getPlayerTopScore()
    //{
    //    var player = realm.Find<PlayerModel>("OfflinePlayer1");
    //    currentPlayerHighestPoints = realm.All<ScoreModel>().Filter("scoreOwner.name = 'OfflinePlayer1' SORT(points DESC)").First().Points;
    //}

}
