using Microsoft.EntityFrameworkCore;
using System.Linq;
using belt2.Models;

namespace belt2
{

    public class DojoActivityCenterContext : DbContext{
        
        // base() calls the parent class' constructor passing the "options" parameter along
        public DojoActivityCenterContext(DbContextOptions<DojoActivityCenterContext> options):base(options){

        }

        // This DbSet contains "Person" objects and is called "Users"
        public DbSet<User> Users { get; set; }         
        public DbSet<Event> Events { get; set; }  
        public DbSet<Participant> Participants { get; set; } 

    }
}