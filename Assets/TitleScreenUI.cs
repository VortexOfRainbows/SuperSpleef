using UnityEngine;
using UnityEngine.UI;

public class TitleScreenUI : MonoBehaviour
{
    public const int MaxChars = 32;
    public static readonly string[] viableFirstNames = { 
        "Defeated", "Dart", "Big", "Suspicious", "Epic", "Smelly", "Nervous", "Verbose", "Humongous", "Abashed", "Tiny", "Star", "Quick", "Slow", "Annoying", "Cheerful", "Goofy", "Lazy", "Hungry", "Sly", "Strong", "Magical", "Blue", "Red", "Scared", 
        "Spooky", "Colorful", "Robo", "Irradiated", "Random", "Sinful", "Holy", "Godly", "Broken", "Damaged", "Demonic", "Lucky", "President", "King", "Plasma", "Energetic", "Dreary", "Hopeful", "Sus", "Truthful", "Gaming", "Shadow", "Running", 
        "Jumping", "Sneaking", "Anime", "Cold", "Hot", "Ugly", "Angry", "Fishy", "Impossible", "Curious", "Smart", "Athletic", "Stunned", "Clear", "Surprised", "Starved", "Super", "Awesome", "Basic", "Extreme", "Murder", "Doctor", "Cooking", 
        "Bagged", "Humid", "Humble", "Double", "Zombie", "Lord", "Lost", "Many", "Beached", "Master", "Professional", "Hospital", "Freedom", "Patriotic", "Legendary", "European", "Mystic", "Messy", "Powerful", "Insightful", "Happy", "Sad", 
        "Dazzled", "Dead", "Beautiful", "Excited", "Thinking", "Pondering", "Bat"
    };
    public static readonly string[] viableSecondNames = { 
        "Goose", "Mongoose", "Duck", "Swan", "Chicken", "Hen", "Rooster", "Manx", "Cat", "Dog", "Goob", "Night", "Mouse", "Taxi", "Man", "House", "Car", "Door", "Wheel", "Castle", "Floor", "Cup", "Building", "BurjKhalifa", "Knob", "President", 
        "King", "Monkey", "Fort", "Dumpster", "Imposter", "Crewmate", "Innocence", "Shadow", "Hedgehog", "Broccoli", "NewYorker", "Fish", "Hippo", "Queen", "Placemat", "Fork", "Spoon", "Knife", "Prince", "Wallet", "Table", "Comic", "Phone", 
        "Money", "Fur", "Hornet", "Plumber", "Cop", "Fireman", "Clerk", "Doctor", "Villain", "Bear", "Moose", "Beaver", "Block", "Dirt", "Rock", "Medic", "Hero", "Mafia", "Theorist", "Window", "Leg", "Hand", "Face", "Park", "PowerPlant", 
        "Doughnut", "Aluminium", "Pan", "Egg", "Bacon", "Cheese", "Bank", "Robber", "Janitor", "Paper", "Uranium", "Gentleman", "Zombie", "Lord", "Ogre", "Goblin", "Dwarf", "Elf", "Steambath", "Squirrel", "Dragon", "Almond", "School", "Program", 
        "Tree", "Tank", "Court", "Judge", "Lawyer", "Rainbow", "Vortex"
    };

    [SerializeField] private InputField UsernameField;
    [SerializeField] private InputField IPField;
    private bool HasUpdatedIPField = false;
    public void Start()
    {
        if(GameStateManager.LocalUsername.Equals(string.Empty))
        {
            RandomizeUsername();
        }
        IPField.text = NetHandler.IP;
        UsernameField.text = GameStateManager.LocalUsername;
    }
    public void RandomizeUsername()
    {
        GameStateManager.LocalUsername = GenerateRandomUsername();
        UsernameField.text = GameStateManager.LocalUsername;
    }
    public static string GenerateRandomUsername()
    {
        while(true)
        {
            string final = viableFirstNames[Random.Range(0, viableFirstNames.Length)] + viableSecondNames[Random.Range(0, viableSecondNames.Length)];
            for(int i = 0; i < 4; i++)
                final += Random.Range(0, 10).ToString();
            if (final.Length <= MaxChars)
                return final;
        }
    }
}
