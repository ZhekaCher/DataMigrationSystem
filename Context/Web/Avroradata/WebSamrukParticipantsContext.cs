using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebSamrukParticipantsContext:WebAvroradataContext
    { 
        public DbSet<SamrukParticipants> SamrukParticipantses { get; set; }
    }
}