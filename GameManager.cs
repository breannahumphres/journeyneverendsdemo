using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set; }
    public Character PlayerCharacter {get ; private set; }

    //UI Elements
    public TextMeshProUGUI storyText; // Link to your story text UI element
    public TextMeshProUGUI Health;
    public TMP_InputField nameInputField;
    public GameObject confirmNameButton;
    public GameObject choiceButton1;
    public GameObject choiceButton2;
    public GameObject Bread;
    public GameObject GoldenDagger;
    public GameObject HealthPotion;

    // Player and Progression
    public GameObject playerGameObject;
    private Character player;
    public int storyProgress = 0;

    //Control flags
    private bool aPressed, dPressed, spacePressed, attackPressed;
    private bool controlsConfirmed = false;
    private Vector3 initialPosition;
    private float walkDistanceThreshold = 20f;
    private int playerAttackCount = 0;
    private EnemyTest enemy;
    private float nextAttackTime = 0f;
    private float attackInterval = 1f;
    public float attackRange = 10f;
    public bool canPickupBread = false;
    public bool canPickupDagger = false;
    public bool canPickupHealthPotion = false;
    private bool isProgressing = false;
    // public GameObject choicePopup;
    // public GameObject inventoryPanel;
    // public GameObject choicePopupPrefab;
    // public GameObject inventoryPanelPrefab;
    // public List<Button> inventorySlots; //this is the list of the inventory slots
    // private Dictionary<string, GameObject> itemIcons = new Dictionary<string, GameObject>(); //track the collected items
    // private string previousStoryText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCharacter();
            // InitializeInventory();
        }
        else
        {
          Destroy(gameObject);  
        }
        
    }
    private void InitializeCharacter()
    {
        if (PlayerCharacter == null)
        {
            PlayerCharacter = new Character("Player", 100, 100, "Rusty Sword");
        }
        UpdateHealthDisplay();
    }

    // private void InitializeInventory()
    // {
    //     foreach (var slot in inventorySlots)
    //     {
    //         slot.gameObject.SetActive(false);
    //     }
    // }

    // public void AddtoInventory(string itemName, Sprite itemIcon)
    // {
    //     Debug.Log("AddtoInventory method called");
    //     if (inventorySlots == null)
    //     {
    //         Debug.LogError("inventorySlots list is null.");
    //     }
    //     else
    //     {
    //         Debug.Log($"inventorySlots contains {inventorySlots.Count} slots.");
    //     }
    //     for (int i = 0; i < inventorySlots.Count; i++)
    //     {
    //         Debug.Log($"Checking slot {i}, active: {inventorySlots[i].gameObject.activeSelf}");

    //         if (!inventorySlots[i].gameObject.activeSelf)
    //         {
    //             Debug.Log($"Activating slot {i} for {itemName}");
    //             inventorySlots[i].gameObject.SetActive(true);
    //             Debug.Log($"Activating slot {i} for {itemName}");

    //             inventorySlots[i].GetComponent<Image>().sprite = itemIcon;
    //             Debug.Log($"Sprite set for {itemName} in slot {i}");
    //             inventorySlots[i].onClick.RemoveAllListeners();
    //             inventorySlots[i].onClick.AddListener(() => ShowItemOptions(itemName, i));
    //             Debug.Log($"Listener added to slot {i} for {itemName}");
    //             itemIcons[itemName] = inventorySlots[i].gameObject;
    //             break;
    //         }
    //         else {
    //             Debug.Log($"Slot {i} is already active");
    //         }
    //     }
    // }

    // private void ShowItemOptions(string itemName, int slotIndex)
    // {
    //     previousStoryText = storyText.text;
    //     string action1 = itemName == "Bread" ? "Eat" : "Use";
    //     string action2 = "Drop";
    //     DisplayChoicePopup(action1, action2,
    //     () => PerformInventoryAction(action1, itemName, slotIndex), 
    //     () => PerformInventoryAction(action2, itemName, slotIndex));
    // }

    // private void PerformInventoryAction(string action, string itemName, int slotIndex)
    // {
    //     if (action == "Eat" && itemName == "Bread")
    //     {
    //         PlayerCharacter.AddHealth(10);
    //         UpdateHealthDisplay();
    //         RemoveFromInventory(slotIndex);
    //         storyText.text = "You ate the bread. It grants 10 health.";
    //     }
    //     else if (action == "Drop")
    //     {
    //         DropItem(itemName);
    //         RemoveFromInventory(slotIndex);
    //         storyText.text = "You dropped the item.";
    //     }
    //     else if (action == "Use" && itemName != "Bread")
    //     {
    //         PlayerCharacter.AddHealth(10);
    //         UpdateHealthDisplay();
    //         RemoveFromInventory(slotIndex);
    //         storyText.text = "You used the item.";
    //     }
    //     StartCoroutine(RestoreStoryTextAfterDelay(previousStoryText, 2f));
    // }

    // private IEnumerator RestoreStoryTextAfterDelay(string previousText, float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     storyText.text = previousText;
    // }
    // private void RemoveFromInventory(int slotIndex)
    // {
    //     inventorySlots[slotIndex].gameObject.SetActive(false);
    //     inventorySlots[slotIndex].onClick.RemoveAllListeners();
    // }
    // private void DropItem(string itemName)
    // {
    //     if (itemIcons.ContainsKey(itemName))
    //     {
    //         itemIcons[itemName].SetActive(true);
    //         itemIcons.Remove(itemName);
    //     }
    // }
    // private void DisplayChoicePopup(string action1, string action2, UnityEngine.Events.UnityAction action1Callback, UnityEngine.Events.UnityAction action2Callback)
    // {
    //     choicePopup = GameObject.Find("choicePopup");
    //     if (choicePopup != null)
    //     {
    //         choicePopup.SetActive(true);
    //     }

        // Find and set up the action buttons
    //     Button actionButton1 = choicePopup.transform.Find("ActionButton1").GetComponent<Button>();
    //     Button actionButton2 = choicePopup.transform.Find("ActionButton2").GetComponent<Button>();

    //     if (actionButton1 == null || actionButton2 == null)
    //     {
    //         Debug.LogError("One or both action buttons are missing as children of choicePopup.");
    //         return;
    //     }

    //     TextMeshProUGUI button1Text = actionButton1.GetComponentInChildren<TextMeshProUGUI>();
    //     TextMeshProUGUI button2Text = actionButton2.GetComponentInChildren<TextMeshProUGUI>();

    //     button1Text.text = action1;
    //     button2Text.text = action2;

    //     actionButton1.onClick.RemoveAllListeners();
    //     actionButton1.onClick.AddListener(action1Callback);
    //     actionButton1.onClick.AddListener(CloseChoicePopup); // Close popup after action

    //     actionButton2.onClick.RemoveAllListeners();
    //     actionButton2.onClick.AddListener(action2Callback);
    //     actionButton2.onClick.AddListener(CloseChoicePopup);

    //     storyText.text = $"Do you want to {action1} or {action2}.";
    // }

    // private void CloseChoicePopup()
    // {
    //     choicePopup.SetActive(false);
    // }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //ONSCENELOADED TO ENSURE ITEMS ARE BEING FOUND SCENE TO SCENE

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {    
        AssignUIReferences();
        MovePlayerToSpawnPoint();
        // UpdateInventoryUI();
        // ActivateCollectedItems();

        if (playerGameObject != null)
        {
            DontDestroyOnLoad(playerGameObject);
        }

        if (scene.name == "MonsterScene")
        {

            Bread = GameObject.FindWithTag("Bread");
            if (Bread !=null) Bread.SetActive(false);
            // choicePopup = GameObject.Find("choicePopup");
            // if (choicePopup != null) choicePopup.SetActive(false);
            GoldenDagger =  GameObject.FindWithTag("GoldenDagger");
            if (GoldenDagger != null) GoldenDagger.SetActive(false);
            if (enemy != null) enemy.gameObject.SetActive(false);
            SetupMonsterSceneUI();
        }
        if (scene.name == "InteriorCottage")
        {
            SetUpCottageSceneUI();
            Bread = GameObject.FindWithTag("Bread");
            if (Bread !=null) Bread.SetActive(true);
            HealthPotion = GameObject.FindWithTag("HealthPotion");
            if (HealthPotion != null) HealthPotion.SetActive(false);
            if (storyProgress == 8)
            {
                StartCoroutine(PrepareHearHerOut());
            }
        }
        else 
        {
            DeactivateScrollView();
        }

        if (scene.name == "EndOfGame")
        {
            if (playerGameObject != null)
            {
                playerGameObject.SetActive(false);  // Hide the player in the final scene
            }
            if (Health != null)
            {
                Health.gameObject.SetActive(false);
            }
            // if (inventoryPanelPrefab != null)
            // {
            //     inventoryPanelPrefab.SetActive(false);
            // }
        }
        else
        {
            if (playerGameObject != null)
            {
                DontDestroyOnLoad(playerGameObject);  // Keep player across other scenes
                playerGameObject.SetActive(true); // Ensure it's enabled in non-final scenes
            }
            if (Health != null)
            {
                Health.gameObject.SetActive(true);
                UpdateHealthDisplay();
            }
        }
        // if (choicePopup == null && choicePopupPrefab != null)
        // {
        //     choicePopup = Instantiate(choicePopupPrefab);
        //     choicePopup.SetActive(true);  // Ensure it starts inactive
        //     Debug.Log("choicePopup instantiated and assigned with ID: " + choicePopup.GetInstanceID());
        // }
        // else if (choicePopup == null)
        // {
        //     Debug.LogError("choicePopupPrefab is missing or not assigned in the inspector.");
        // }
        // else
        // {
        //     Debug.Log("choicePopup already assigned with ID: " + choicePopup.GetInstanceID());
        // }
        StartCoroutine(RefreshUIAfterSceneLoad());
    }
    public void UpdateHealthDisplay()
    {
        if (Health != null)
        {
            Health.text = $"Health: {PlayerCharacter.Health}/{PlayerCharacter.MaxHealth}";
        }
    }
    private void SetUpCottageSceneUI()
    {
        if (nameInputField != null) nameInputField.gameObject.SetActive(false);
        if (confirmNameButton != null) confirmNameButton.SetActive(false);
        if (choiceButton1 != null) choiceButton1.SetActive(false);
        if (choiceButton2 != null) choiceButton2.SetActive(false);
        if (storyText != null) storyText.gameObject.SetActive(true); // Ensure story text is visible
    }
    private void SetupMonsterSceneUI()
    {
        if (confirmNameButton != null) confirmNameButton.SetActive(true);
        if (choiceButton1 != null) choiceButton1.SetActive(true);
        if (choiceButton2 != null) choiceButton2.SetActive(true);
    }

    private void MovePlayerToSpawnPoint()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        GameObject spawnPoint = GameObject.Find("PlayerSpawnPoint");

        if (playerObject != null && spawnPoint != null)
        {
            playerObject.transform.position = spawnPoint.transform.position;
        }
        else if (playerObject == null)
        {
            Debug.LogWarning("Player object not found");
        }
        else if (spawnPoint == null)
        {
            Debug.LogWarning("spawnpoint not found");
        }
    }
    // public void UpdateInventoryUI()
    // {
    //     foreach(var item in PlayerCharacter.Inventory)
    //     {
    //         if (itemIcons.TryGetValue(item, out GameObject itemUI))
    //         {
    //             itemUI.SetActive(true);
    //         }
    //     }
    // }

    // private void ActivateCollectedItems()
    // {
    //     if (PlayerCharacter.Inventory.Contains("Bread") && Bread != null)
    //     {
    //         Bread.SetActive(true);
    //     }
    // }
    private void ActivateScrollView(bool useScrollView)
    {
        GameObject scrollView = GameObject.Find("Scroll View");
        if (useScrollView && scrollView != null)
        {
            scrollView.SetActive(true);
            storyText = scrollView.transform.Find("Viewport/Content/StoryText")?.GetComponent<TextMeshProUGUI>();
            if (storyText == null)
            {
                Debug.LogError("StoryText UI element is missing in Scroll View hierarchy.");
            }
        }
        else
        {
            if (scrollView != null) scrollView.SetActive(false);
            storyText = GameObject.Find("StoryText")?.GetComponent<TextMeshProUGUI>();
        }
        
    }
    private void DeactivateScrollView()
    {
        GameObject scrollView = GameObject.Find("ScrollView");
        if (scrollView != null)
        {
            scrollView.SetActive(false);
        }
    }
    private void Start()
    {   
        if (storyProgress == 0)
        {
            SetupInitialUI();
        }
        
        
    }
    private void SetupInitialUI()
    {
        confirmNameButton.SetActive(true);
        HideChoiceButtons();
    }

    private IEnumerator RefreshUIAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.2f);
        UpdateStoryProgression();
    }

    private void AssignUIReferences()
    {
        if (SceneManager.GetActiveScene().name == "InteriorCottage")
        {
            ActivateScrollView(true); // Use Scroll View and assign storyText for InteriorCottage
        }
        else
        {
            ActivateScrollView(false); // Disable Scroll View in other scenes and assign regular StoryText
            storyText = GameObject.Find("StoryText")?.GetComponent<TextMeshProUGUI>();

        }
        Bread = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("Bread"));
        if (Bread != null)
        {
            Debug.Log("Bread found in AssignUIReferences");
            Bread.SetActive(true);
            Sprite breadSprite = Bread.GetComponent<SpriteRenderer>()?.sprite;
            // if (breadSprite != null)
            // {
            //     AddtoInventory("Bread", breadSprite); // Add bread to the inventory
            // }
            // else
            // {
            //     Debug.LogError("Bread sprite is missing, cannot add to inventory.");
            // }
        }
        else 
        {
            Debug.LogError("Bread not found in AssignUIReferences");
        }

        // inventoryPanel = GameObject.Find("inventoryPanel");
        // if (inventoryPanel != null)
        // {
        //     inventorySlots = inventoryPanel.GetComponentsInChildren<Button>(true).ToList();
        //     Debug.Log($"inventorySlots contains {inventorySlots.Count} slots after assignment.");
        // }
        // else 
        // {
        //     Debug.LogError("inventoryPanel not found in scene");
        // }
        // choicePopup = GameObject.Find("choicePopup");
        // if (choicePopup != null)
        // {
        //     choicePopup.SetActive(true);
        // }
        // else if (choicePopupPrefab == null)
        // {
        //     Debug.LogError("choicePopupPrefab is not assigned");
        // }
        // Debug.Log("Number of inventory slots found: " + inventorySlots.Count);

        nameInputField = GameObject.Find("NameInput")?.GetComponent<TMP_InputField>();
        confirmNameButton = GameObject.Find("ConfirmNameButton");
        choiceButton1 = GameObject.Find("ChoiceButton1");
        choiceButton2 = GameObject.Find("ChoiceButton2");
        enemy = Object.FindObjectsByType<EnemyTest>(FindObjectsSortMode.None).FirstOrDefault();
        Health = GameObject.Find("Health")?.GetComponent<TextMeshProUGUI>();
        UpdateHealthDisplay();
        Debug.Log("Enemy assigned: " + (enemy != null));
        if (confirmNameButton != null)
        {
            confirmNameButton.SetActive(false);
        }
        if (storyText == null) Debug.LogError("StoryText UI element is missing. Please ensure it's available in this scene.");
        if (storyProgress > 0 && (choiceButton1 == null || choiceButton2  == null))
        {
            Debug.LogError("Choice buttons are missing. Ensure ChoiceButton1 and ChoiceButton2 are present in this scene.");
        }
        if (confirmNameButton == null) Debug.LogError("ConfirmNameButton UI element is missing. Please ensure it's available in this scene.");
    }
    public void LoadAwakeningScene()
    {
        Debug.Log("LoadAwakeningScene called, resetting storyProgress");
        storyProgress = 0;
        SceneManager.LoadScene("MonsterScene");
        StartCoroutine(RefreshUIAfterSceneLoad());
    }

    private void ActivateAwakeningSceneUI()
    {
        storyText.gameObject.SetActive(true);
        nameInputField.gameObject.SetActive(true);
        confirmNameButton.SetActive(true);
    }
    public void UpdateStoryProgression()
    {
        if (isProgressing) return;
        isProgressing = true;

        Debug.Log("Current storyProgress: " + storyProgress);
        switch (storyProgress)
        {
            case 0:
                DisplayAwakeningMessage();
                ActivateAwakeningSceneUI();
                break;
            case 1: 
                TriggerMonsterEncounter();
                break;
            case 2: 
                DisplayMonsterChoices();
                ActivateMonsterSceneUI();
                break;
            case 3: 
                DisplayFightOutcome();
                break;
            case 4:
                DisplayFleeOutcome();
                ActivateFleeOutcomeUI();
                break;
            case 5:
                DisplayRewardChoice();
                ActivateRewardChoiceUI();
                break;
            case 6:
                ContinueJourney();
                break;
            case 7:
                EnterCottage();
                break;
            case 8: 
                StartCoroutine(PrepareHearHerOut());
                break;
            case 9:
                PickupPotion();
                break;
            case 10:
                MemoryRestored();
                break;
            case 11:
                DisplayEvalineGoodbyeScene();
                break;
            case 12:
                EndOfGame();
                break;
            default:
                Debug.Log("End of Story or Unknown Progress");
                break;
        }
        isProgressing = false;
    }
    private void DisplayAwakeningMessage()
    {
        storyText.text = "You have just awoken in a mystical forest. You see that your health is at 100, you have 100 gold coins in your pocket, and a sturdy-looking rusty sword. You must have hit your head because you cannot remember much. But you do remember your name.";
        nameInputField.gameObject.SetActive(true);
        confirmNameButton.SetActive(true);
    }

    public void ConfirmNameAndStartGame()
    {
        if (!string.IsNullOrWhiteSpace(nameInputField.text))
        {
            string playerName = nameInputField.text;
            GameManager.Instance.PlayerCharacter.Name = playerName;
            GameManager.Instance.PlayerCharacter.Equip("Rusty Sword");
            if (storyProgress == 0) 
            {
                DisplayControlInstructions();
            }
            
        }
        else
        {
            Debug.Log("Please enter a valid name.");
        }
    }
    private void DisplayControlInstructions()
    {
        storyText.text = "To begin your journey, let's get familiar with your controls. Use the A and D keys to move back and forth, Space to jump, and the Left Mouse Button to attack.";
        nameInputField.gameObject.SetActive(false);
        confirmNameButton.SetActive(false);
        controlsConfirmed = false;
    }

    private void Update()
    {   
        if (storyProgress == 0)
        {
            if (!controlsConfirmed)
            {
                CheckKeyPresses();
            }
            else if (playerGameObject != null && initialPosition != null)
            {
                CheckDistanceTraveled();
            }
        }
        if (storyProgress == 3 && Input.GetMouseButtonDown(0))
        {
            if (playerGameObject != null && enemy != null)
            {
                float distance = Vector3.Distance(playerGameObject.transform.position, enemy.transform.position);
                // Debug.Log("Player to enemy distance: " + distance + ", Attack Range: " + attackRange);
                if (distance <= attackRange)
                {
                    if (Time.time >= nextAttackTime)
                    {
                        nextAttackTime = Time.time + attackInterval;
                        DisplayFightSequence();
                    }
                }
            }
        }
    }

    private void CheckKeyPresses()
    {
        if (Input.GetKeyDown(KeyCode.A)) aPressed = true;
        if (Input.GetKeyDown(KeyCode.D)) dPressed = true;
        if (Input.GetKeyDown(KeyCode.Space)) spacePressed = true;
        if (Input.GetMouseButtonDown(0)) attackPressed = true;
        if (aPressed && dPressed && spacePressed && attackPressed)
        {
            controlsConfirmed = true;
            initialPosition = playerGameObject.transform.position;
            storyText.text = "Great! Now walk forward a bit to get used to the movement.";
        }
    }
    private void CheckDistanceTraveled()
    {
        float distanceTraveled = Vector3.Distance(initialPosition, playerGameObject.transform.position);
        if (distanceTraveled >= walkDistanceThreshold)
        {
            TriggerMonsterEncounter();
        }
    }


    private void TriggerMonsterEncounter()
    {
        storyProgress = 2;
        GameObject monster = GameObject.FindWithTag("Enemy");
        if (monster != null)
        {
            monster.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Monster not found in the scene.");
        }
        DisplayMonsterChoices();
    }

    private void ActivateMonsterSceneUI()
    {
        if (enemy != null) enemy.gameObject.SetActive(false);
        storyText.gameObject.SetActive(true);
        choiceButton1.SetActive(true);
        choiceButton2.SetActive(true);
    }

    private void DisplayMonsterChoices()
    {
        if (enemy != null) enemy.gameObject.SetActive(true);
        storyText.text = "A monster has appeared! What do you wanna do?";
        SetChoiceButtons("Fight", "Flee", FightMonster, FleeMonster);
    }
    private void FightMonster()
    {   
        storyText.text = "Try attacking the monster!";
        HideChoiceButtons();
        playerAttackCount = 0; 
        storyProgress = 3; //advances to fight outcome case
    }

    private void DisplayFightSequence()
    {
        // Debug.Log("Entering DisplayFightSequence with playerAttackCount: " + playerAttackCount);
        if (playerAttackCount < 3 )
        {
            playerAttackCount++;
            storyText.text = $"You attack the monster!";
            if (enemy != null)
            {
                enemy.Animation_3_Hit();
            }
            //Debug.Log($"Attack Count: {playerAttackCount}, storyProgress: {storyProgress}");
            if (playerAttackCount == 3)
            {
                Debug.Log("Monster defeated");
                if (enemy != null)
                {
                    enemy.Animation_4_Death();
                    StartCoroutine(RemoveMonsterAfterDelay(2f));
                }
                storyProgress = 5;
                DisplayFightOutcome();
                Debug.Log($"storyProgress set to {storyProgress}");
            }
            Debug.Log("Exiting DisplayFightSequence normally");
        }
    }
    private IEnumerator RemoveMonsterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (enemy != null)
        {
            enemy.gameObject.SetActive(false);
        }
    }

    private IEnumerator WaitAndAttack()
    {
        yield return new WaitForSeconds(1f);
        if (playerAttackCount < 3)
        {
           DisplayFightSequence(); 
        }    
    }
    private void FleeMonster()
    {
        storyText.text = "You try to flee but the monster attacks, causing you to lose 10 health.";
        if (enemy != null)
        {
            enemy.Animation_6_Attack();
        }
        else 
        {
            Debug.LogError("Enemy reference is missing!");
        }
        PlayerCharacter.TakeDamage(10);
        UpdateHealthDisplay();
        storyProgress = 4; 
        DisplayFleeOutcome();
        UpdateStoryProgression();
    }
    private void DisplayFightOutcome()
    {
        storyText.text = "You stand your ground and realize you can defeat the monster with your rusty sword!";
        if (Bread!=null)
        {
            Bread.SetActive(true); 
        }
        else
        {
            Debug.LogWarning("Bread object is null. Make sure it's assigned in the scene.");
        }
        UpdateStoryProgression();
    }
    private void DisplayFleeOutcome()
    {
        storyText.text = "You attempt to flee but the monster is not going anywhere and just attacked! You lost 10 damage! Try fighting with your sword!";
        SetChoiceButtons("Fight", "Keep Running", FightMonster, KeepRunning);
    }

    private void ActivateFleeOutcomeUI()
    {
        storyText.gameObject.SetActive(true);
        choiceButton1.SetActive(true);
        choiceButton2.SetActive(true);
    }
    private void KeepRunning()
    {   
        PlayerCharacter.TakeDamage(10);
        UpdateHealthDisplay();
        storyText.text = $"You tried to keep running, but it attacked again. You've lost 10 more damage. You now have {PlayerCharacter.Health} health.";
        
        StartCoroutine(DelayedFightOutcome(2f));
    }

    private IEnumerator DelayedFightOutcome(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisplayFightOutcome();
    }

    private void DisplayRewardChoice()
    {
        storyText.text = "After defeating the monster, you see he dropped a loaf of bread. The idea of leaving a gold coin as tribute to the beast also entered your head. Do you take the bread or leave a tribute?";
        SetChoiceButtons("Take Bread", "Leave Tribute", TakeBread, LeaveTribute);
    }

    private void ActivateRewardChoiceUI()
    {
        storyText.gameObject.SetActive(true);
        choiceButton1.SetActive(true);
        choiceButton2.SetActive(true);
    }

    private void TakeBread()
    {
        storyText.text = "You take the bread. It can restore health by 10.";
        HideChoiceButtons();
        canPickupBread = true;
    }
  
    private void LeaveTribute()
    {
        PlayerCharacter.LoseGold(10);
        canPickupDagger = true;
        storyText.text = "You leave a tribute and a golden dagger appears! You equip it!";

        if (GoldenDagger != null)
        {
            GoldenDagger.SetActive(true);
        }
        else 
        {
            Debug.LogWarning("golden dagger reference is missing");
        }
        HideChoiceButtons();
    }

    public void ProgressStoryWithDelay(int newProgress, float delay)
    {
        StartCoroutine(DelayedProgressStory(newProgress, delay));
    }
    private IEnumerator DelayedProgressStory(int newProgress, float delay)
    {
        yield return new WaitForSeconds(delay);
        ProgressStory(newProgress);
    }
    public void ProgressStory(int newProgress)
    {
        storyProgress = newProgress;
        UpdateStoryProgression();
    }

    private void ContinueJourney()
    {
        storyText.text = "Continue on your journey.";
    }
    public void OnEnterCottage()
    {
        storyProgress = 7;
        UpdateStoryProgression();
    }
    public void EnterCottage()
    {
        if (SceneManager.GetActiveScene().name != "InteriorCottage")
        {
            HideMonsterSceneUI();
            SceneManager.LoadScene("InteriorCottage");
            storyProgress = 8;
        }
    }

    private void HideMonsterSceneUI()
    {
        if (nameInputField != null) nameInputField.gameObject.SetActive(false);
        if (confirmNameButton != null) confirmNameButton.SetActive(false);
        if (choiceButton1 != null) choiceButton1.SetActive(false);
        if (choiceButton2 != null) choiceButton2.SetActive(false);
    }

    private IEnumerator PrepareHearHerOut()
    {
        yield return null;
        if(storyText != null)
        {
            storyText.text = $"{PlayerCharacter.Name}, I have been waiting for you. Please, hear me out. I have a potion that will restore your memory.";
            if (confirmNameButton != null)
            {
                confirmNameButton.SetActive(true);
                confirmNameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Hear her out";
                confirmNameButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
                confirmNameButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    if (!isProgressing)
                    {
                        storyProgress = 9;
                    UpdateStoryProgression();
                    }
                });
            }
        }
        else
        {
            Debug.LogWarning("StoryText UI element is still missing in Interior cottage scene.");
        }
    }
    public void PickupPotion()
    {
        storyText.text = "Evaline dropped the health potion for you.";

        if (HealthPotion !=null)
        {
            HealthPotion.SetActive(true);
        }
        canPickupHealthPotion = true;
        storyProgress = 10;
        UpdateStoryProgression();
    }
    public void MemoryRestored()
    {
        storyText.text = $"After taking the Health Potion, Evaline is staring. Studying as always. You detest when she attempts to read you like a book. Though you suppose you should be thanking her. The High Wizard wiped your mind and you never would have regained your memory if not for her.'So you do still care for me after all this time, Evaline. Incredibly touching. But I really should be going. High Wizard to defeat and all, ya know? Thanks again, though!' You head towards the door. 'Do you think you could defeat the High Wizard with just that, {PlayerCharacter.CurrentWeapon}?' You pause. A {PlayerCharacter.CurrentWeapon}? Could I?.. No... Maybe?.. Hmm...";
        if (confirmNameButton != null)
        {
            confirmNameButton.SetActive(true);
            confirmNameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            confirmNameButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            confirmNameButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                storyProgress = 11;
                UpdateStoryProgression();
            });
        }
    }

    private void DisplayEvalineGoodbyeScene()
    {
        storyText.text = "'What did you have in mind, my dear friend', you ask. She smiles slightly and chuckles. 'Head out that door and continue through the forest path. It will take you about 2 miles outside the walls of the city of Constania. There you will find a blacksmith merchant stand. The blacksmith is an Elf named Dylan. Tell him I sent you and he will provide you with a weapon that will destroy the High Wizard. Once and for all... Then, you must head to his Palace in the Cerulean City. You must end him.' You see tears falling down her face. You step forward and hug her. 'Thank you, Evaline. For everything. I will never forget what you have done for me. I will find the High Wizard and I will put a stop to his reign of terror. I will make sure my brother can no longer harm the citizens of this world.' She does not say anything.";
        if (confirmNameButton != null)
        {
            confirmNameButton.SetActive(true);
            confirmNameButton.GetComponentInChildren<TextMeshProUGUI>().text = "End Game";
            confirmNameButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            confirmNameButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SceneManager.LoadScene("EndOfGame"));
        }
        storyProgress = 12;
        UpdateStoryProgression();
    }
    private void EndOfGame()
    {
        storyText.text = "You've reached the end of your adventure, for now! Thank you for playing The Journey Never Ends Demo. This demo was created in two weeks as a final project by Bre Humphres. Thank you to everyone who supported me. I appreciate you all. May the journey never end.";
        if (confirmNameButton != null)
        {   
            HideChoiceButtons();
            confirmNameButton.SetActive(true);
            confirmNameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Exit";
            confirmNameButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            confirmNameButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(RestartGame);
        }
    }
    private void SetChoiceButtons(string choice1Text, string choice2Text, UnityEngine.Events.UnityAction choice1Action, UnityEngine.Events.UnityAction choice2Action)
    {
        choiceButton1.GetComponentInChildren<TextMeshProUGUI>().text = choice1Text;
        choiceButton2.GetComponentInChildren<TextMeshProUGUI>().text = choice2Text;
        choiceButton1.SetActive(true);
        choiceButton2.SetActive(true);
        choiceButton1.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        choiceButton2.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        choiceButton1.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(choice1Action);
        choiceButton2.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(choice2Action);
    }

    private void HideChoiceButtons()
    {
        if (choiceButton1 != null && choiceButton2 != null) 
        {
            choiceButton1.SetActive(false);
            choiceButton2.SetActive(false);
        }
    }
    private void RestartGame()
    {
        #if UNITY_EDITOR
        Debug.Log("Exit button click. Application would close in a build");
        #else
        Application.Quit();
        #endif 
    }

}