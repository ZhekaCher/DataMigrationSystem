using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebSamrukParticipantsContext:WebContext
    {
        public DbSet<SamrukParticipants> SamrukParticipantses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}