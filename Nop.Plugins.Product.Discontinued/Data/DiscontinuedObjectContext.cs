using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Nop.Core;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Plugin.Product.Discontinued.Domain;

namespace Nop.Plugin.Product.Discontinued.Data
{
    public class DiscontinuedObjectContext : DbContext, IDbContext
    {
        public DiscontinuedObjectContext(DbContextOptions<DiscontinuedObjectContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DiscontinuedMap());
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public string GenerateCreateScript()
        {
            return this.Database.GenerateCreateScript();
        }

        public IQueryable<TQuery> QueryFromSql<TQuery>(string sql) where TQuery : class
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity
        {
            return this.Set<TEntity>().FromSql(CreateSqlWithParameters(sql, parameters), parameters);
        }

        public int ExecuteSqlCommand(RawSqlString sql, bool doNotEnsureTransaction = false, int? timeout = null,
            params object[] parameters)
        {
            using (var transaction = this.Database.BeginTransaction())
            {
                var result = this.Database.ExecuteSqlCommand(sql, parameters);
                transaction.Commit();

                return result;
            }
        }

        public void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }
        protected virtual string CreateSqlWithParameters(string sql, params object[] parameters)
        {
            //add parameters to sql
            for (var i = 0; i <= (parameters?.Length ?? 0) - 1; i++)
            {
                if (!(parameters[i] is DbParameter parameter))
                    continue;

                sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";

                //whether parameter is output
                if (parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output)
                    sql = $"{sql} output";
            }

            return sql;
        }
        public void Install()
        {
            this.ExecuteSqlScript(this.GenerateCreateScript());
            this.ExecuteSqlScript("ALTER TABLE dbo.DiscontinuedStatus ADD CONSTRAINT FK_Product_DiscontinuedStatus FOREIGN KEY(ProductId) REFERENCES dbo.Product(Id) ON DELETE CASCADE ON UPDATE CASCADE");
            this.ExecuteSqlScriptFromFile("E:\\Nop\\ProductLoadAllPaged.sql");

        }
        public void Uninstall()
        {
            this.DropPluginTable(nameof(DiscontinuedStatus));
            this.ExecuteSqlScriptFromFile("E:\\Nop\\ProductLoadAllPagedOriginal.sql");
        }
    }
}
