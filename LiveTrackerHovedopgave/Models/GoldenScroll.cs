namespace LiveTrackerHovedopgave.Models;

public class GoldenScroll
{
    public string RoomName { get; set; }

    public int GoldenScrollID { get; set; }

    // Aggregering: bundles hører til et rum (GoldenScroll)
    private List<Bundle> bundles = new List<Bundle>();

    // Adgang til bundles, så fx en Razor Page kan vise dem
    public List<Bundle> Bundles
    {
        get { return bundles; }
    }

    public GoldenScroll(string roomName, int goldenScrollID)
    {
        RoomName = roomName;
        GoldenScrollID = goldenScrollID;
    }

    // Tilføjer et bundle til rummet
    public void AddBundle(Bundle bundle)
    {
        bundles.Add(bundle);
    }

    // Returnerer true hvis alle bundles i rummet er færdige
    public bool IsRoomComplete()
    {
        if (bundles.Count == 0)
        {
            return false;
        }

        foreach (Bundle bundle in bundles)
        {
            if (bundle.IsComplete == false)
            {
                return false;
            }
        }

        return true;
    }
}
