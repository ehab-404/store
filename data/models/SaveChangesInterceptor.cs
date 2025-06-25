using GSF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using testRestApi.Controllers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

using Microsoft.AspNetCore.Mvc;
using testRestApi.data;




namespace testRestApi.data.models
{
    public class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _IHttpcontextAccessor;
        private readonly List<InfrastructureTransaction> _PendingLogs = new List<InfrastructureTransaction>();


        public AuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor)
        {

            _IHttpcontextAccessor = httpContextAccessor;

        }


        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,

            CancellationToken cancellationToken = default)
        {









            //  var changer = _contextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


            var context = eventData.Context;
            if (context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

            var entries = context.ChangeTracker.Entries().Where(l => l.Entity is Category || l.Entity is item)
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted).ToList();




           
            ;
            bool BulkOPeration = entries.Count > 1;
            

            foreach (var entry in entries)
            {
                if (entry.Entity is InfrastructureTransaction) continue;
                var tablename = entry.Metadata.GetTableName() ?? "UnKnownTable";


                var primarykey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString() ?? "N/A.";

                //pk of inserted



                if (entry.State == EntityState.Added)
                {



                    decimal nextId;

                    var sql = $"SELECT IDENT_CURRENT('{tablename}')+IDENT_INCR('{tablename}')";


                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = sql;
                        context.Database.OpenConnection();
                        var res = command.ExecuteScalar();

                        if (res != null)
                        {
                            nextId = Convert.ToDecimal(res);
                            primarykey = nextId.ToString();
                        }
                        ;






                    }
                }

                        object? data = entry.State switch
                        {
                            EntityState.Added => entry.CurrentValues.ToObject(),
                            EntityState.Modified => entry.CurrentValues.ToObject(),
                            EntityState.Deleted => entry.OriginalValues.ToObject(),
                            _ => null
                        };



                        bool isSoftDelete = false;

                        if (entry.State == EntityState.Deleted) { isSoftDelete = true; entry.CurrentValues["IsDeleted"] = true; entry.State = EntityState.Modified; } ;

                        var json = JsonSerializer.Serialize(data);

                        var user = _IHttpcontextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


                        if (user == null) { user = "system"; }
                        ;


                        _PendingLogs.Add(new InfrastructureTransaction
                        {
                            TableName = tablename,
                            OperationType = isSoftDelete ? "SoftDelete" : entry.State.ToString(),
                            PrimaryKeyValue = primarykey,
                            EntityDataJson = json,/* TransactionId = transactionId,*/
                            Timestamp = DateTime.UtcNow,
                            UserName = user,
                            IsSoftDelete = isSoftDelete,
                            IsBulkOPeration = BulkOPeration
                        });





             }



                    if (_PendingLogs.Any())
                    {

                        context.Set<InfrastructureTransaction>().AddRange(_PendingLogs);

                        context.SaveChanges();

                        _PendingLogs.Clear();

                    }


                    return await base.SavingChangesAsync(eventData, result, cancellationToken);


                }
            }
        }











