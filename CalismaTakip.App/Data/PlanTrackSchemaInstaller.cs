using System.Data;
using Microsoft.EntityFrameworkCore;

namespace CalismaTakip.Data;

/// <summary>
/// Eski günlük checkbox tablolarını kaldırır ve plan şablonu / günlük takip tablolarını oluşturur.
/// Mevcut haftalık plan (TimeSlots) tablolarına dokunmaz.
/// </summary>
public static class PlanTrackSchemaInstaller
{
    public static void EnsureInstalled(AppDbContext db)
    {
        var connection = db.Database.GetDbConnection();
        var wasOpen = connection.State == ConnectionState.Open;
        if (!wasOpen)
            connection.Open();

        try
        {
            if (TableExists(connection, "PlanTemplateItems"))
                return;

            using var tx = connection.BeginTransaction();
            try
            {
                Execute(connection, tx, "DROP TABLE IF EXISTS \"DailyCheckCompletions\";");
                Execute(connection, tx, "DROP TABLE IF EXISTS \"DailyCheckRecords\";");
                Execute(connection, tx, "DROP TABLE IF EXISTS \"DailyCheckDefinitions\";");

                Execute(connection, tx, """
                    CREATE TABLE IF NOT EXISTS "PlanTemplateItems" (
                        "Id" INTEGER NOT NULL CONSTRAINT "PK_PlanTemplateItems" PRIMARY KEY AUTOINCREMENT,
                        "TemplateKind" INTEGER NOT NULL,
                        "StartTime" TEXT NOT NULL,
                        "EndTime" TEXT NOT NULL,
                        "Title" TEXT NOT NULL,
                        "SortOrder" INTEGER NOT NULL
                    );
                    """);

                Execute(connection, tx, """
                    CREATE TABLE IF NOT EXISTS "DailyPlanTrackHeaders" (
                        "Id" INTEGER NOT NULL CONSTRAINT "PK_DailyPlanTrackHeaders" PRIMARY KEY AUTOINCREMENT,
                        "TrackDate" TEXT NOT NULL,
                        "TemplateKind" INTEGER NOT NULL,
                        "Note" TEXT NOT NULL
                    );
                    """);

                Execute(connection, tx, """
                    CREATE UNIQUE INDEX IF NOT EXISTS "IX_DailyPlanTrackHeaders_TrackDate"
                    ON "DailyPlanTrackHeaders" ("TrackDate");
                    """);

                Execute(connection, tx, """
                    CREATE TABLE IF NOT EXISTS "DailyPlanTrackItems" (
                        "Id" INTEGER NOT NULL CONSTRAINT "PK_DailyPlanTrackItems" PRIMARY KEY AUTOINCREMENT,
                        "HeaderId" INTEGER NOT NULL,
                        "StartTime" TEXT NOT NULL,
                        "EndTime" TEXT NOT NULL,
                        "Title" TEXT NOT NULL,
                        "SortOrder" INTEGER NOT NULL,
                        "IsCompleted" INTEGER NOT NULL,
                        CONSTRAINT "FK_DailyPlanTrackItems_DailyPlanTrackHeaders_HeaderId"
                            FOREIGN KEY ("HeaderId") REFERENCES "DailyPlanTrackHeaders" ("Id") ON DELETE CASCADE
                    );
                    """);

                Execute(connection, tx, """
                    CREATE INDEX IF NOT EXISTS "IX_DailyPlanTrackItems_HeaderId_SortOrder"
                    ON "DailyPlanTrackItems" ("HeaderId", "SortOrder");
                    """);

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
        finally
        {
            if (!wasOpen)
                connection.Close();
        }
    }

    private static bool TableExists(System.Data.Common.DbConnection connection, string name)
    {
        var safe = name.Replace("'", "''", StringComparison.Ordinal);
        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{safe}'";
        var result = cmd.ExecuteScalar();
        return result is long l ? l > 0 : Convert.ToInt64(result) > 0;
    }

    private static void Execute(
        System.Data.Common.DbConnection connection,
        System.Data.Common.DbTransaction tx,
        string sql)
    {
        using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }
}
