using LiveTrackerHovedopgave.Models;

namespace LiveTrackerHovedopgave.Services.Interfaces;

public interface IBundleRepository
{
    List<GoldenScroll> GetAll(string saveFolderName);

    // Gemmer fremskridtet for en save. Returnerer true hvis det blev gemt, false hvis saven ikke findes
    bool SaveProgress(string saveFolderName, List<GoldenScroll> rooms);
}
