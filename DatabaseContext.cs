using BuildSqliteCF.Entity;
using System.Data.Entity;
using System.Data.SQLite;


namespace Planner
{
    public class TDTDbContext : DbContext
    {
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ToDo> ToDos { get; set; }
        public virtual DbSet<Track> Tracks { get; set; }

        public TDTDbContext() :
            base(new SQLiteConnection()
            {
                //ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = "D:\\Databases\\SQLiteWithEF.db", ForeignKeys = true }.ConnectionString
                ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = (string)App.Current.Properties["destFilePath"], ForeignKeys = true }.ConnectionString
            }, true)
        {
        }

        //public TDTDbContext() :
        //    base(new SQLiteConnection()
        //    {
        //        //ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = "D:\\Databases\\SQLiteWithEF.db", ForeignKeys = true }.ConnectionString
        //        ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = "C:\\Users\\be3so\\AppData\\Local\\TDT\\TDTDb.sqlite", ForeignKeys = true }.ConnectionString
        //    }, true)
        //{
        //}



        public TDTDbContext(string theDataBase) :
            base(theDataBase)   // ,true
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
          //  modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

     //   public DbSet<EmployeeMaster> EmployeeMaster { get; set; }
    }
}

