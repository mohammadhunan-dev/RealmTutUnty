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
    private IDisposable listenerToken;
    public ListView listView;
    public Button toggleButton;
    public Label displayTitle;
    public int currentPlayerHighestPoints = 0; // 0 til it's set
    public Realms.Sync.User user;
    public bool isLeaderboardGUICreated = false; 
    public static LeaderboardManager Instance;
    public bool isUIVisible;
    public int maximumAmountOfTopScores;
    public List<ScoreModel> topScores;
    public VisualElement root;


    // 
    void Awake()
    {
        Instance = this;
    }

    public async void setLoggedInUser(Realms.Sync.User loggedInUser)
    {
        user = loggedInUser;
        var syncConfiguration = new SyncConfiguration("UnityTutorialPartition", user);
        realm = await Realm.GetInstanceAsync(syncConfiguration);

        // only create the leaderboard on the first run, consecutive restarts/reruns will already have a leaderboard created
        if(isLeaderboardGUICreated == false)
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            createLeaderboardUI();
            root.Add(toggleButton);
            root.Add(displayTitle);
            root.Add(listView);
            isUIVisible = true;

            toggleUIVisible();

            toggleButton.clicked += () =>
            {
                toggleUIVisible();
            };
            setScoreListener(); // start listening for score changes once the leaderboard GUI has launched
            isLeaderboardGUICreated = true;
        }
    }

    public int getRealmPlayerTopScore()
    {
        var realmPlayer = realm.Find<PlayerModel>(user.Id);
        var realmPlayerTopScore = realmPlayer.Scores.OrderByDescending(s => s.Points).First().Points;
        return realmPlayer.Scores.OrderByDescending(s => s.Points).First().Points;
    }

    void toggleUIVisible()
    {
        Debug.Log("isUIVisible -" + isUIVisible);
        if (isUIVisible == true)
        {

            Debug.Log("A Called!");
            // if ui is already visible, and the toggle button is pressed, then hide it
            displayTitle.RemoveFromClassList("visible");
            displayTitle.AddToClassList("hide");
            listView.RemoveFromClassList("visible");
            listView.AddToClassList("hide");
            isUIVisible = false;
        }
        else
        {
            Debug.Log("B Called");
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


        topScores = realm.All<ScoreModel>().OrderByDescending(s => s.Points).ToList();
        createTopScoreListView();


    }
    public void createTopScoreListView()
    {
        if (topScores.Count > 4)
        {
            maximumAmountOfTopScores = 5;
        }
        else
        {
            maximumAmountOfTopScores = topScores.Count;
        }


        var topScoresListItems = new List<string>();

        topScoresListItems.Add("Your top points: " + getRealmPlayerTopScore());


        for (int i = 0; i < maximumAmountOfTopScores; i++)
        {
            if (topScores[i].Points > 1) // if there's not many players there may not be 5 top scores yet
            {
                topScoresListItems.Add($"{topScores[i].ScoreOwner.Name}: {topScores[i].Points} points");
            }
        };

        // Create a new label for each top score
        var label = new Label();
        label.AddToClassList("list-item-game-name-label");
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
    public void setScoreListener()
    {
        // Observe collection notifications. Retain the token to keep observing.
        listenerToken = realm.All<ScoreModel>()
            .SubscribeForNotifications((sender, changes, error) =>
            {

                Debug.Log("new change!!!");
                if (error != null)
                {
                    // Show error message
                    Debug.Log("an error occurred while listening for score changes :" + error);
                    return;
                }
                // we only need to check for inserted because scores can't be modified or deleted after the run is complete
                foreach (var i in changes.InsertedIndices)
                {
                    // ... handle insertions ...
                    var newScore = realm.All<ScoreModel>().ElementAt(i);
                    Debug.Log("inserted boi ----" + newScore.Points);
                    Debug.Log("new score owner ----" + newScore.ScoreOwner.Name);

                    for (var scoreIndex = 0; scoreIndex < topScores.Count; scoreIndex++)
                    {
                        if (topScores.ElementAt(scoreIndex).Points < newScore.Points)
                        {
                            if (topScores.Count > 4)
                            { // An item shouldnt be removed if its the leaderboard is less than 5 items
                                topScores.RemoveAt(topScores.Count - 1);
                            }
                            topScores.Insert(scoreIndex, newScore);
                            root.Remove(listView); // remove the old listView
                            createTopScoreListView(); // create a new listView
                            root.Add(listView); // add the new listView to the UI
                            break;
                        }
                    }




                }
            });
    }

    void OnDisable()
    {
        if (realm != null)
        {
            realm.Dispose();
        }

        if(listenerToken != null)
        {
            realm.Dispose();
        }
    }

}
