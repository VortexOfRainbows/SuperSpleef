using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenUI : MonoBehaviour
{
    public const int MaxChars = 32;
    public static readonly string[] viableFirstNames = { 
        "Nature", "Earthen", "Frigid", "Otherworldly", "Tidal", "Evil", "Infernal", "Chaotic", 
        "Defeated", "Dart", "Big", "Suspicious", "Epic", "Smelly", "Nervous", "Verbose", "Humongous", "Abashed", "Tiny", "Star", "Quick", "Slow", "Annoying", "Cheerful", "Goofy", "Lazy", "Hungry", "Sly", "Strong", "Magical", "Blue", "Red", "Scared", 
        "Spooky", "Colorful", "Robo", "Irradiated", "Random", "Sinful", "Holy", "Godly", "Broken", "Damaged", "Demonic", "Lucky", "President", "King", "Plasma", "Energetic", "Dreary", "Hopeful", "Sus", "Truthful", "Gaming", "Shadow", "Running", 
        "Jumping", "Sneaking", "Anime", "Cold", "Hot", "Ugly", "Angry", "Fishy", "Impossible", "Curious", "Smart", "Athletic", "Stunned", "Clear", "Surprised", "Starved", "Super", "Awesome", "Basic", "Extreme", "Murder", "Doctor", "Cooking", 
        "Bagged", "Humid", "Humble", "Double", "Zombie", "Lord", "Lost", "Many", "Beached", "Master", "Professional", "Hospital", "Freedom", "Patriotic", "Legendary", "European", "Mystic", "Messy", "Powerful", "Insightful", "Happy", "Sad", 
        "Dazzled", "Dead", "Beautiful", "Excited", "Thinking", "Pondering", "Bat", "VortexOf"
    };
    public static readonly string[] viableSecondNames = { 
        "Goose", "Mongoose", "Duck", "Swan", "Chicken", "Hen", "Rooster", "Manx", "Cat", "Dog", "Goob", "Night", "Mouse", "Taxi", "Man", "House", "Car", "Door", "Wheel", "Castle", "Floor", "Cup", "Building", "BurjKhalifa", "Knob", "President", 
        "King", "Monkey", "Fort", "Dumpster", "Imposter", "Crewmate", "Innocence", "Shadow", "Hedgehog", "Broccoli", "NewYorker", "Fish", "Hippo", "Queen", "Placemat", "Fork", "Spoon", "Knife", "Prince", "Wallet", "Table", "Comic", "Phone", 
        "Money", "Fur", "Hornet", "Plumber", "Cop", "Fireman", "Clerk", "Doctor", "Villain", "Bear", "Moose", "Beaver", "Block", "Dirt", "Rock", "Medic", "Hero", "Mafia", "Theorist", "Window", "Leg", "Hand", "Face", "Park", "PowerPlant", 
        "Doughnut", "Aluminium", "Pan", "Egg", "Bacon", "Cheese", "Bank", "Robber", "Janitor", "Paper", "Uranium", "Gentleman", "Zombie", "Lord", "Ogre", "Goblin", "Dwarf", "Elf", "Steambath", "Squirrel", "Dragon", "Almond", "School", "Program", 
        "Tree", "Tank", "Court", "Judge", "Lawyer", "Rainbow", "Vortex", "Spirit"
    };
    public static readonly string[] SplashTextVariants = 
    {
        "like a record baby", "yellow text", "now with splash text", "how many blocks can you click", "names courtesy of zombified bears", "also try Minecraft", "also try Terraria", "also try SOTS tMod", "life level up", "avoid the void", 
        "now with more chunks", "now with geometry", "dancing with slimes and flies", "cloudy with a chance of spheres", "grass to touch here", "sponsored by gamers", "unlimited bacon but no video games\nor unlimited games but no games", 
        "ants in pants", "let bro cook", "we cooked too much", "a shrimp fried this rice", "multiplier", "plays boom sound effect", "among sus", "gremlin activity", "cooked al dente", "now with 20% more",
        "buy one for the price of two and get one free", "hi hungry im dad", "s tier", "now with lasers", "bear hill roll", "ducks vs chickens", "v\ne\nr\nt\ni\nc\na\nl"
    };

    [SerializeField] private InputField UsernameField;
    [SerializeField] private InputField IPField;
    [SerializeField] private Text SplashText;
    [SerializeField] private GameObject MultiplayerButton;
    [SerializeField] private GameObject NoMultButton;
    private const float spinSpeed = 90f;
    private const float wobbleSizeSpeed = 6f;
    private const float growShrinkAmt = 0.015f;
    private float timer;
    public void Start()
    {
        if(GameStateManager.LocalUsername.Equals(string.Empty))
        {
            RandomizeUsername();
        }
        IPField.text = NetHandler.IP;
        UsernameField.text = GameStateManager.LocalUsername;
        SplashText.text = GenerateSplashText();
    }
    public void Update()
    {
        if(SceneManager.GetActiveScene().name == GameStateManager.TitleScreen)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                NoMultButton.SetActive(true);
                MultiplayerButton.SetActive(false);
            }
            else
            {
                MultiplayerButton.SetActive(true);
                NoMultButton.SetActive(false);
            }
        }
        else
        {
            NoMultButton.SetActive(false);
            MultiplayerButton.SetActive(false);
        }
        if(SplashText.text == SplashTextVariants[0])
        {
            SplashText.gameObject.transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        }
        timer += Time.deltaTime;
        if (SplashText.text != SplashTextVariants[1])
        {
            float r = Mathf.Sin(timer) * 0.5f + 0.5f;
            float g = Mathf.Sin(timer + 2.0f) * 0.5f + 0.5f; //The 2.0 and 4.0 are estimates for 2/3 of Pi and 4/3 of Pi respectively.
            float b = Mathf.Sin(timer + 4.0f) * 0.5f + 0.5f;
            SplashText.color = Color.Lerp(new Color(r, g, b), Color.white, 0.5f);
        }
        SplashText.transform.localScale = Vector3.one * (1 - growShrinkAmt + growShrinkAmt * Mathf.Cos(timer * wobbleSizeSpeed));
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
    public static string GenerateSplashText()
    {
        return SplashTextVariants[Random.Range(0, SplashTextVariants.Length)];
    }
}
