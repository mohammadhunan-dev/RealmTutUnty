// :code-block-start: unity-score-model
// :state-start: final
// >>>> HERE
// :state-end: :state-uncomment-start: start
//// TODO: implement schema
// :state-uncomment-end:
// :code-block-end:


using System;
using System.Linq;
using MongoDB.Bson;
using Realms;

public class ScoreModel : RealmObject
{
    [MapTo("_id")]
    [PrimaryKey]
    public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

    public DateTimeOffset Time { get; private set; } = DateTimeOffset.Now;

    [MapTo("points")]
    public int Points { get; set; }

    [MapTo("enemiesDefeated")]
    public int EnemiesDefeated { get; set; }

    [MapTo("tokensCollected")]
    public int TokensCollected { get; set; }

    [MapTo("scoreOwner")]
    public PlayerModel ScoreOwner { get; set; }

    public ScoreModel()
    {

    }

    //public ScoreModel(PlayerModel scoreOwner, int points, int enemiesDefeated, int tokensCollected)
    //{
    //    this.ScoreOwner = scoreOwner;
    //    this.Points = points;
    //    this.EnemiesDefeated = enemiesDefeated;
    //    this.TokensCollected = tokensCollected;
    //}
}