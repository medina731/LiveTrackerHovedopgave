namespace LiveTrackerHovedopgave.Models;

public class Save
{
    public int SaveID { get; set; }
    public string SaveFolderName { get; set; }
    public User User { get; set; }

    // Hardcodede testdata: viser en mulig fremtidig iteration med flere brugere og saves
    private static List<Save> saves = CreateTestData();

    public Save(int saveID, string saveFolderName, User user)
    {
        SaveID = saveID;
        SaveFolderName = saveFolderName;
        User = user;
    }

    private static List<Save> CreateTestData()
    {
        User user1 = new User(1, "Anders");
        User user2 = new User(2, "Mette");

        List<Save> testSaves = new List<Save>();
        testSaves.Add(new Save(1, "Farm_123456789", user1));
        testSaves.Add(new Save(2, "Hovedgaard_987654321", user1));
        testSaves.Add(new Save(3, "Solgaarden_555555555", user2));

        return testSaves;
    }

    // Returnerer alle saves
    public static List<Save> GetAll()
    {
        return new List<Save>(saves);
    }

    // Returnerer det save der har det givne ID, eller null hvis det ikke findes
    public static Save? GetByID(int saveID)
    {
        foreach (Save save in saves)
        {
            if (save.SaveID == saveID)
            {
                return save;
            }
        }

        return null;
    }

    // Opdaterer et eksisterende save. Returnerer false hvis det ikke findes
    public static bool Update(Save updatedSave)
    {
        Save? existing = GetByID(updatedSave.SaveID);

        if (existing == null)
        {
            return false;
        }

        existing.SaveFolderName = updatedSave.SaveFolderName;
        existing.User = updatedSave.User;
        return true;
    }
}