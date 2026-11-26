using LiveTrackerHovedopgave.Models;
using LiveTrackerHovedopgave.Services.Interfaces;

namespace LiveTrackerHovedopgave.Services.Repositories;

public class BundleRepository : IBundleRepository
{
    // Bruges først, når databasen på simply.com kobles på
    private string connectionString = "";

    // Midlertidig lagring i hukommelsen: ét sæt rum pr. save
    private Dictionary<string, List<GoldenScroll>> progress = new Dictionary<string, List<GoldenScroll>>();

    // Sikrer at modden (POST) og web-siden (GET) ikke rører dictionary'en samtidigt
    private object padlock = new object();

    // Returnerer rummene for en save (tom liste hvis modden ikke har sendt noget endnu)
    public List<GoldenScroll> GetAll(string saveFolderName)
    {
        lock (padlock)
        {
            if (progress.TryGetValue(saveFolderName, out List<GoldenScroll> rooms))
            {
                // Kopi, så kalderen ikke kan ændre den gemte liste uden om SaveProgress
                return new List<GoldenScroll>(rooms);
            }

            return new List<GoldenScroll>();
        }
    }

    // Gemmer fremskridtet for en save
    public void SaveProgress(string saveFolderName, List<GoldenScroll> rooms)
    {
        lock (padlock)
        {
            // Kopi, så senere ændringer i kalderens liste ikke ændrer det gemte
            progress[saveFolderName] = new List<GoldenScroll>(rooms);
        }
    }
}