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

public class LeadboardManager : MonoBehaviour
{
    private Realm realm;
    public ListView listView;
    public Button toggleButton;
    public Label displayTitle;
    public bool isUIVisible = true;
    public int currentPlayerHighestPoints;
    // Start is called before the first frame update
    void Start()
    {

        // open a Realm
        realm = Realm.GetInstance();

        getPlayerTopScore();


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

        var items = new List<string>(itemCount);
        items.Add("Your highest points:" + currentPlayerHighestPoints);

        for (int i = 1; i <= itemCount; i++)
            items.Add("SampleSyncedCharacter" + i + ": " + i * 125 + " points");

        // The "makeItem" function is called when the
        // ListView needs more items to render.
        var label = new Label();
        label.AddToClassList("list-item-game-name-label");
        Func<VisualElement> makeItem = () => new Label();

        Action<VisualElement, int> bindItem = (e, i) => {
            (e as Label).text = items[i];
            (e as Label).AddToClassList("list-item-game-name-label");
        };

        // Provide the list view with an explict height for every row
        // so it can calculate how many items to actually display
        const int itemHeight = 5;

        listView = new ListView(items, itemHeight, makeItem, bindItem);
        listView.AddToClassList("list-view");
    }

    void getPlayerTopScore()
    {
        var player = realm.Find<PlayerModel>("OfflinePlayer1");
        currentPlayerHighestPoints = realm.All<ScoreModel>().Filter("scoreOwner.name = 'OfflinePlayer1' SORT(points DESC)").First().Points;
    }

}
