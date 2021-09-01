
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

public class PlayerModel : RealmObject { 

    [PrimaryKey]
    [MapTo("_id")]
    [Required]
    public string Id { get; set; }

    [MapTo("_partition")]
    [Required]
    public string Partition { get; set; }

    [MapTo("scores")]
    public IList<ScoreModel> Scores { get; }

    [MapTo("name")]
    [Required]
    public string Name { get; set; }

}