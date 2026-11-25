namespace LiveTrackerHovedopgave.Models;

public class User
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public User(int userID, string userName)
    {
        UserID = userID;
        UserName = userName;
    }
}