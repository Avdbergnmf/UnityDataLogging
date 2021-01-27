[System.Serializable]
public class PlayerInfo
{
    // Save any sort of player info in an instance of thiss class to easily pass it around other scripts
    public string name;
    public int age;
    public string gender;

    public PlayerInfo() // so that it starts out with some values
    {
        name = "Player";
        age = 20;
        gender = "male";
    }
}