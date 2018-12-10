﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataBase : MonoBehaviour {

    public static int cash = 100;
    public static int currentBuildingPrice;
    public static string currentBuilding;
    public static bool highlightBuildingTest = false;
    public static GameObject previousBuilding;

    public static bool officeBought = false;
    public static bool convienienceStoreBought = false;
    public static bool apartmentBuilding1Bought = false;
    public static bool apartmentBuilding2Bought = false;
    public static bool tradeCenter1Bought = false;
    public static bool tradeCenter2Bought = false;

    public static IList<Building> buildingsList = new List<Building>() {

        new Building(){ buildingName = "Office Building", buildingBought = false, buildingPrice = 20},
        new Building(){ buildingName = "Convienience Store", buildingBought = false, buildingPrice = 30},
        new Building(){ buildingName = "Apartment Building 1", buildingBought = false, buildingPrice = 50},
        new Building(){ buildingName = "Apartment Building 2", buildingBought = false, buildingPrice = 50},
        new Building(){ buildingName = "Trade Center 1", buildingBought = false, buildingPrice = 75},
        new Building(){ buildingName = "Trade Center 2", buildingBought = false, buildingPrice = 75}
    };

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public class Building
    {
        public string buildingName { get; set; }

        public bool buildingBought { get; set; }

        public int buildingPrice { get; set; }
    }
}
