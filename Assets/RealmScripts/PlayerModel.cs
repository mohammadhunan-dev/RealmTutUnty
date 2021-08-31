
// :code-block-start: unity-player-model
// :state-start: final
// >>>> HERE
// :state-end: :state-uncomment-start: start
//// TODO: implement schema
// :state-uncomment-end:
// :code-block-end:



using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

public class PlayerModel : RealmObject
{
    [PrimaryKey]
    public string name { get; set; }

    public IList<ScoreModel> Scores { get; }
}