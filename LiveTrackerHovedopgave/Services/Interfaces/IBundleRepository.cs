using LiveTrackerHovedopgave.Models;

namespace LiveTrackerHovedopgave.Services.Interfaces;

public interface IBundleRepository
{
    List<GoldenScroll> GetAll(string saveFolderName);
    void SaveProgress(string saveFolderName, List<GoldenScroll> rooms);
}
