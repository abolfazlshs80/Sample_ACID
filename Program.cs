using Microsoft.EntityFrameworkCore;
using Sample_ACID.Data.Context;

namespace Sample_ACID
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new AppDbContext();
            context.Database.EnsureCreated();
            // Atomicity Example
            AtomicTransfer(context);

            // Consistency Example
            ConsistentWithdraw(context);

            // Isolation Example
            IsolationLevelDemo(context);

            // Durability is ensured by SQL Server after Commit
        }

        static void AtomicTransfer(AppDbContext context)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var from = context.Accounts.Find(1);
                var to = context.Accounts.Find(2);

                from.Balance -= 100;
                to.Balance += 100;

                context.SaveChanges();
                transaction.Commit();
                Console.WriteLine("Atomic transfer succeeded.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("Atomic transfer failed: " + ex.Message);
            }
        }

        static void ConsistentWithdraw(AppDbContext context)
        {
            var account = context.Accounts.Find(1);

            if (account.Balance >= 200)
            {
                account.Balance -= 200;
                context.SaveChanges();
                Console.WriteLine("Consistent withdraw succeeded.");
            }
            else
            {
                Console.WriteLine("Insufficient funds - consistency preserved.");
            }
        }

        static void IsolationLevelDemo(AppDbContext context)
        {
            using var conn = context.Database.GetDbConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE";
            cmd.ExecuteNonQuery();

            using var transaction = context.Database.BeginTransaction();
            var account = context.Accounts.Find(1);
            if (account.Balance > 0)
            {
                account.Balance -= 1;
                context.SaveChanges();
                transaction.Commit();
                Console.WriteLine("Isolation-level transfer succeeded.");
            }
            else
            {
                transaction.Rollback();
                Console.WriteLine("No balance to withdraw - transaction rolled back.");
            }
        }
    }
}
