using ComputerManagement.BO.Lib.Implement;
using ComputerManagement.BO.Lib.Interface;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options ) : base( options )
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<ComputerRoom> ComputerRooms { get; set; }
        public DbSet<Computer> Computers { get; set; }
        public DbSet<User> ScheduleBookRoom { get; set; }
        public DbSet<ComputerHistory> ComputerHistory { get; set; }
        public DbSet<SoftwareModel> SoftwareModel { get; set; }
        public DbSet<FileModel> FileModel { get; set; }
        public DbSet<MonitorSession> MonitorSession { get; set; }
        public DbSet<CommandOption> CommandOption { get; set; }
        public DbSet<ConfigOption> ConfigOption { get; set; }
        public DbSet<AgentModel> Agent { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<FileProof> FileProof { get; set; }

    }
}
