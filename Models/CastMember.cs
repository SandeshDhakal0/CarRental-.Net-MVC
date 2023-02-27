using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDSystem.Models;

public class CastMember
{
    [ForeignKey("ActorNumber")] public int ActorNumber { get; set; }

    [ForeignKey("DVDNumber")] public int DVDNumber { get; set; }

    //Relationships
    public virtual Actor Actor { get; set; }
    public virtual DVDTitle DVDTitle { get; set; }
}