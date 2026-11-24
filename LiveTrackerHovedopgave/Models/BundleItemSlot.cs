namespace LiveTrackerHovedopgave.Models;

public class BundleItemSlot
{
    public int BundleItemID { get; set; }
    public int BundleItemSlotID { get; set; }
    public int BundleItemSlotIndex { get; set; }
    public bool IsFilled { get; set; }

    // Skifter slotten mellem fyldt og ikke fyldt
    public void IsFilledToggle()
    {
        if (IsFilled)
        {
            IsFilled = false;
        }
        else
        {
            IsFilled = true;
        }
    }
}
