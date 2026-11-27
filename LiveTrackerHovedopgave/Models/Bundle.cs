namespace LiveTrackerHovedopgave.Models;

public class Bundle
{
    public int BundleID { get; set; }
    public int BundleIndex { get; set; }
    public string BundleInternalName { get; set; } = string.Empty;
    public string BundleName { get; set; } = string.Empty;

    // Komposition: slots hører til præcis ét bundle
    private List<BundleItemSlot> slots = new List<BundleItemSlot>();

    // Skrivebeskyttet adgang, så fx en Razor Page kan vise slots
    public List<BundleItemSlot> Slots
    {
        get { return slots; }
    }

    public bool IsComplete
    {
        get { return GetIsComplete(); }
    }

    public int SlotsFilled
    {
        get { return GetSlotsFilled(); }
    }

    // Returnerer antal slots der er fyldt
    public int GetSlotsFilled()
    {
        int count = 0;

        foreach (BundleItemSlot slot in slots)
        {
            if (slot.IsFilled)
            {
                count++;
            }
        }
        return count;
    }

    // Returnerer true hvis alle slots er fyldt
    public bool GetIsComplete()
    {
        return slots.Count > 0 && GetSlotsFilled() == slots.Count;
    }

    // Tilføjer en slot til bundlet
    public void AddSlot(BundleItemSlot slot)
    {
        slots.Add(slot);
    }
}
