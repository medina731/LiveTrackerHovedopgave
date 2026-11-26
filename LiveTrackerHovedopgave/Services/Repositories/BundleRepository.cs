using System.Data;
using LiveTrackerHovedopgave.Models;
using LiveTrackerHovedopgave.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace LiveTrackerHovedopgave.Services.Repositories;

public class BundleRepository : IBundleRepository
{
    private readonly string connectionString;
    public BundleRepository(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // Henter rum, bundles og slots for en save i ét SELECT med JOIN
    public List<GoldenScroll> GetAll(string saveFolderName)
    {
        string sql = @"
            SELECT g.RoomName,
                   b.BundleID, b.BundleIndex,
                   b.InternalName AS BundleInternalName, b.Name AS BundleName,
                   s.BundleItemSlotID, s.BundleItemSlotIndex,
                   CAST(COALESCE(sp.IsFilled, 0) AS BIT) AS IsFilled
            FROM GoldenScroll g
            JOIN Bundle b ON b.BundleID = g.Bundle_BundleID
            JOIN BundleItemSlot_has_Bundle bs ON bs.Bundle_BundleID = b.BundleID
            JOIN BundleItemSlot s ON s.BundleItemSlotID = bs.BundleItemSlot_BundleItemSlotID
            LEFT JOIN Save sv ON sv.SaveFolderName = @saveFolderName
            LEFT JOIN SaveProgress sp ON sp.SaveID = sv.SaveID
                                     AND sp.BundleItemSlotID = s.BundleItemSlotID
            ORDER BY g.RoomName, b.BundleIndex, s.BundleItemSlotIndex";

        //Hjælpe Variabler.
        List<GoldenScroll> rooms = new List<GoldenScroll>();
        GoldenScroll? currentRoom = null;
        Bundle? currentBundle = null;
        string? currentRoomName = null;
        int? currentBundleID = -1;

        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@saveFolderName", saveFolderName);
            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string roomName = (string)reader["RoomName"];
                    int bundleID = (int)reader["BundleID"];

                    // Nyt rum, når RoomName skifter
                    bool sameRoom = roomName == currentRoomName;
                    if (sameRoom == false)
                    {
                        currentRoom = new GoldenScroll(roomName);
                        rooms.Add(currentRoom);
                        currentRoomName = roomName;
                    }

                    // Nyt bundle, når BundleID skifter ( object initializer new bundle)
                    bool sameBundle = bundleID == currentBundleID;
                    if (sameBundle == false)
                    {
                        currentBundle = new Bundle
                        {
                            BundleID = bundleID,
                            BundleIndex = (int)reader["BundleIndex"],
                            BundleInternalName = (string)reader["BundleInternalName"],
                            BundleName = (string)reader["BundleName"]
                        };
                        currentRoom.AddBundle(currentBundle);
                        currentBundleID = bundleID;
                    }

                    currentBundle.AddSlot(new BundleItemSlot
                    {
                        BundleItemSlotID = (int)reader["BundleItemSlotID"],
                        BundleItemSlotIndex = (int)reader["BundleItemSlotIndex"],
                        IsFilled = Convert.ToBoolean(reader["IsFilled"])
                    });
                }
            }
        }

        return rooms;
    }

    // Gemmer fremskridtet for en save i én transaktion
    public bool SaveProgress(string saveFolderName, List<GoldenScroll> rooms)
    {
        string findSaveSql = "SELECT SaveID FROM Save WHERE SaveFolderName = @saveFolderName";

        // Opdaterer rækken, og opretter den hvis den ikke findes endnu
        string upsertSql = @"
            UPDATE SaveProgress
            SET IsFilled = @isFilled
            WHERE SaveID = @saveID AND BundleItemSlotID = @slotID;

            IF @@ROWCOUNT = 0
                INSERT INTO SaveProgress (SaveID, BundleItemSlotID, IsFilled)
                VALUES (@saveID, @slotID, @isFilled);";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    object result;
                    using (SqlCommand findCommand = new SqlCommand(findSaveSql, connection, transaction))
                    {
                        findCommand.Parameters.AddWithValue("@saveFolderName", saveFolderName);
                        result = findCommand.ExecuteScalar();
                    }

                    // Ukendt save: der gemmes ingenting
                    if (result == null)
                    {
                        return false;
                    }

                    int saveID = (int)result;

                    using (SqlCommand command = new SqlCommand(upsertSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@saveID", saveID);
                        SqlParameter slotParameter = command.Parameters.Add("@slotID", SqlDbType.Int);
                        SqlParameter filledParameter = command.Parameters.Add("@isFilled", SqlDbType.Bit);

                        foreach (GoldenScroll room in rooms)
                        {
                            foreach (Bundle bundle in room.Bundles)
                            {
                                foreach (BundleItemSlot slot in bundle.Slots)
                                {
                                    slotParameter.Value = slot.BundleItemSlotID;
                                    filledParameter.Value = slot.IsFilled;
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}