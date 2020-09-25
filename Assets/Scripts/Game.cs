using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    // Variables
    public float currency = 0f; // Currency we have.
    public float currencyPerSecond = 0f; // Currency per second from buildings.
    public Text currencyText; // Text to display currency we have.
    public Text currencyPerSecondText; // Text to display currency per second from buildings.

    // Building struct
    [System.Serializable] // So we can see this thing in the inspector.
    public struct Building
    {
        [Header("Stats")]
        public string name; // Name of the building.
        public float baseCps; // Base currency per second.

        public float baseCost; // Base cost.
        public float count; // Number of building owned.

        public float upgradeCost; // Base upgrade cost.
        public int upgradeLevel; // Building level.

        [Header("References")]
        public GameObject uiGroup; // Used to enable/disable groups of UI elements.

        public Text countText; // Shows number of building owned.
        public Text nameText; // Shows name and level of building.
        public Text costText; // Shows cost of building.
        public Text upgradeText; // Shows upgrade cost of building.
    }

    public Building[] buildings = new Building[4]; // Array of buildings.

    #region Start
    // Start is called at the start. Go figure :^)
    private void Start()
    {
        // Initalise currency/currency per second texts.
        currencyText.text = currency.ToString();
        currencyPerSecondText.text = "CpS: " + currencyPerSecond.ToString();

        // Loop through all the buildings, initialise their text.
        foreach (Building building in buildings)
        {
            building.costText.text = "Cost: " + building.baseCost.ToString();
            building.countText.text = building.count.ToString();
            building.upgradeText.text = "Upgrade: " + building.upgradeCost.ToString();
            building.nameText.text = string.Format("{0} (Lv. {1})", building.name, building.upgradeLevel + 1);
        }
    }
    #endregion
    #region Update
    // Update is called every frame
    void Update()
    {
        // Add currency from buildings
        AddCurrency(currencyPerSecond * Time.deltaTime);

        // DEBUG: Press E to add currency
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddCurrency(100000);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    #endregion

    #region AddCurrency()
    // Function to add currency, and update the currency text.
    // Could do some property shenanigans instead.
    public void AddCurrency(float value)
    {
        currency += value;
        currencyText.text = currency.ToString();

        // Check to see if we can unlock a building.
        foreach (Building building in buildings)
        {
            if (!building.uiGroup.activeSelf)
            {
                if (currency >= building.baseCost)
                {
                    building.uiGroup.SetActive(true);
                }
            }
        }
    }
    #endregion

    #region BuyBuilding(int)
    // Function to buy buildings.
    public void BuyBuilding(int target)
    {
        // Check if we have enough money to buy a building.
        if (currency >= buildings[target].baseCost * Mathf.Pow(1.15f, buildings[target].count))
        {
            // Add building, take money = baseCost * 1.15 ^ count.
            currency -= buildings[target].baseCost * Mathf.Pow(1.15f, buildings[target].count);
            buildings[target].count++;

            // Update UI elements
            buildings[target].countText.text = buildings[target].count.ToString(); // Building count
            buildings[target].costText.text = string.Format("Cost: {0}", buildings[target].baseCost * Mathf.Pow(1.15f, buildings[target].count)); // Building cost

            // CpS changed. Calculate it again.
            CalculateCps();
        }
    }
    #endregion

    #region UpgradeBuilding(int)
    // Function to upgrade buildings.
    public void UpgradeBuilding(int target)
    {
        // Check if we have enough money to upgrade the building.
        if (currency >= buildings[target].upgradeCost * Mathf.Pow(10, buildings[target].upgradeLevel))
        {
            // Upgrade building, take money = upgradeCost * 10 ^ upgradelevel
            currency -= buildings[target].upgradeCost * Mathf.Pow(10, buildings[target].upgradeLevel);
            buildings[target].upgradeLevel++;

            // Update UI for building level.
            // String should be "BuildingName (Lv. #)"
            buildings[target].nameText.text = string.Format("{0} (Lv. {1})", buildings[target].name, buildings[target].upgradeLevel + 1); // Building name/level.
            buildings[target].upgradeText.text = "Upgrade: " + buildings[target].upgradeCost * Mathf.Pow(10, buildings[target].upgradeLevel); // Upgrade cost

            // CpS changed. Calculate it again.
            CalculateCps();
        }
    }
    #endregion

    #region CalculateCps()
    // Function to calculate currency/second
    public void CalculateCps()
    {
        currencyPerSecond = 0f;

        // Go through every building in the building array, add them together to get the total CpS.
        // Calculate the total CpS from buildings whenever the amount of buildings/building upgrades change.
        foreach (Building building in buildings)
        {
            currencyPerSecond += building.count * building.baseCps * (building.upgradeLevel + 1);
        }

        // Update CpS text.
        currencyPerSecondText.text = "CpS: " + currencyPerSecond.ToString();
    }
    #endregion

    #region Update UI Functions
    // Currency
    // CpS
    // Building name/level
    // Building count
    // Building cost
    // Building upgrade cost
    #endregion
}
