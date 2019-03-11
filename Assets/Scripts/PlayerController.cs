﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

    public static IList<Building> buildingsList = new List<Building>() {

        new Building(){ buildingName = "Office Building 1", buildingBought = false, buildingPrice = 50, owner = "none", revenue = 10, employeeCap = 10, employeesOwned = 0},
        new Building(){ buildingName = "Office Building 2", buildingBought = false, buildingPrice = 50, owner = "none", revenue = 10, employeeCap = 10, employeesOwned = 0},
        new Building(){ buildingName = "Convenience Store 1", buildingBought = false, buildingPrice = 30, owner = "none", revenue = 15, employeeCap = 5, employeesOwned = 0},
        new Building(){ buildingName = "Convenience Store 2", buildingBought = false, buildingPrice = 30, owner = "none", revenue = 15, employeeCap = 5, employeesOwned = 0},
        new Building(){ buildingName = "Apartment Building 1", buildingBought = false, buildingPrice = 50, owner = "none", revenue = 25, employeeCap = 10, employeesOwned = 0},
        new Building(){ buildingName = "Apartment Building 2", buildingBought = false, buildingPrice = 50, owner = "none", revenue = 25, employeeCap = 10, employeesOwned = 0},
        new Building(){ buildingName = "Trade Center 1", buildingBought = false, buildingPrice = 70, owner = "none", revenue = 35, employeeCap = 15, employeesOwned = 0},
        new Building(){ buildingName = "Trade Center 2", buildingBought = false, buildingPrice = 70, owner = "none", revenue = 35, employeeCap = 15, employeesOwned = 0},
        new Building(){ buildingName = "Club", buildingBought = false, buildingPrice = 100, owner = "none", revenue = 50, employeeCap = 20, employeesOwned = 0},
        new Building(){ buildingName = "Super Market", buildingBought = false, buildingPrice = 50, owner = "none", revenue = 25, employeeCap = 10, employeesOwned = 0},
        new Building(){ buildingName = "Church", buildingBought = false, buildingPrice = 30, owner = "none", revenue = 15, employeeCap = 5, employeesOwned = 0},
        new Building(){ buildingName = "Movie Theater", buildingBought = false, buildingPrice = 50, owner = "none", revenue = 25, employeeCap = 10, employeesOwned = 0}
    };

    private GameObject dayText;
    private GameObject cashText;
    private GameObject employeePoolText;

    void Start () {

        dayText = GameObject.Find("Day");
        cashText = GameObject.Find("Cash");
        employeePoolText = GameObject.Find("Employee Pool");
    }
	
	void Update () {

        if (DataBase.cash >= 200 && isServer)
        {
            DataBase.serverWin = true;
            RpcUpdateWinOnClient();
        }
        else if (DataBase.cash >= 200 && isClient) {
            DataBase.clientWin = true;
            CmdUpdateWinOnServer();
        }

        if (DataBase.serverTurnEnded == true && hasAuthority) //if the end turn button has been pressed...
        {
            /*DataBase.turns++;

            if (DataBase.turns % 2 == 0) //every 2 turns, the day is ended
            {
                DataBase.day++;
                dayText.GetComponent<Text>().text = DataBase.day.ToString();
            }*/

            RpcUpdateTurnOnClient(DataBase.day, DataBase.employeePool);

            DataBase.serverTurnEnded = false; 
        }
        else if (DataBase.clientTurnEnded == true && hasAuthority)
        {
            /*DataBase.turns++;

            if (DataBase.turns % 2 == 0) //every 2 turns, the day is ended
            {
                DataBase.day++;
                dayText.GetComponent<Text>().text = DataBase.day.ToString();
            } */

            DataBase.day++;
            dayText.GetComponent<Text>().text = DataBase.day.ToString();

            CmdUpdateTurnOnServer(DataBase.day, DataBase.employeePool);
            DataBase.clientTurnEnded = false;
        }

        if (DataBase.serverBuildingPurchased == true)
        {
            for (int ii = 0; ii < buildingsList.Count; ii++) //if new purchase shows up in the database that isn't recognized locally, server sends info to the client
            {
                if (DataBase.buildingsList[ii].buildingBought == true && buildingsList[ii].buildingBought == false && DataBase.buildingsList[ii].owner == "Server" && hasAuthority)
                {
                    RpcUpdateBuildingPurchaseOnClient(ii);
                    DataBase.serverBuildingPurchased = false;
                }
            }
        }

        if (DataBase.clientBuildingPurchased == true)
        {
            for (int i = 0; i < buildingsList.Count; i++) //if new purchase shows up in the database that isn't recognized locally, client sends info to the sever
            {
                if (DataBase.buildingsList[i].buildingBought == true && buildingsList[i].buildingBought == false && DataBase.buildingsList[i].owner == "Client" && hasAuthority)
                {
                    CmdUpdateBuildingPurchaseOnServer(i);
                    DataBase.clientBuildingPurchased = false;
                }
            }
        }
	}

    [Command]
    void CmdUpdateBuildingPurchaseOnServer(int building) { //sends command to run on the server that says that a specific building has been baught
        DataBase.buildingsList[building].buildingBought = true;
        DataBase.buildingsList[building].owner = "Client";

        buildingsList[building].buildingBought = true;
        buildingsList[building].owner = "Client";
    }

    [ClientRpc]
    void RpcUpdateBuildingPurchaseOnClient(int building) { //sends command to run on the client
        DataBase.buildingsList[building].buildingBought = true;
        DataBase.buildingsList[building].owner = "Server";

        buildingsList[building].buildingBought = true;
        buildingsList[building].owner = "Server";
    }

    [Command]
    void CmdUpdateTurnOnServer(int day, int newEmployeePool) {
        DataBase.turns++;
        DataBase.day = day;
        dayText.GetComponent<Text>().text = DataBase.day.ToString();
        DataBase.turn = "server";
        //DataBase.cash += DataBase.totalRevenue;  //this is the old way of adding revenue per turn
        DataBase.cash = DataBase.AddRevenue();
        cashText.GetComponent<Text>().text = DataBase.cash.ToString();
        DataBase.employeePool = newEmployeePool;
        employeePoolText.GetComponent<Text>().text = newEmployeePool.ToString();
    }

    [ClientRpc]
    void RpcUpdateTurnOnClient(int day, int newEmployeePool) {
        DataBase.turns++;
        DataBase.day = day;
        dayText.GetComponent<Text>().text = DataBase.day.ToString();
        DataBase.turn = "client";
        if (!isServer)
        {
            //DataBase.cash += DataBase.totalRevenue;
            DataBase.cash = DataBase.AddRevenue();
            cashText.GetComponent<Text>().text = DataBase.cash.ToString();
            DataBase.employeePool = newEmployeePool;
            employeePoolText.GetComponent<Text>().text = newEmployeePool.ToString();
        }
    }

    [Command]
    void CmdUpdateWinOnServer()
    {
        DataBase.clientWin = true;
    }

    [ClientRpc]
    void RpcUpdateWinOnClient()
    {
        DataBase.serverWin = true;
    }

    public class Building
    {
        public string buildingName { get; set; }

        public bool buildingBought { get; set; }

        public int buildingPrice { get; set; }

        public string owner { get; set; }

        public int revenue { get; set; }

        public int employeeCap { get; set; }

        public int employeesOwned { get; set; }
    }
}
