using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using XamarinLOBFormService.DataObjects;

namespace XamarinLOBFormService.Models
{
    public class XamarinLOBFormContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to alter your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        private const string connectionStringName = "Name=MS_TableConnectionString";

        public XamarinLOBFormContext() : base(connectionStringName)
        {
        }

        public DbSet<MyUser> MyUsers { get; set; }
        public DbSet<LeaveCategory> LeaveCategories { get; set; }
        public DbSet<LeaveAppForm> LeaveAppForms { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<WorkingLog> WorkingLogs { get; set; }
        public DbSet<OnCallPhone> OnCallPhones { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
        }
    }

}
