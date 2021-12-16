using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;


public class EnableOnSpecificUser : MonoBehaviour
{

	public System.Collections.Generic.List<CSteamID> listofUsers;

	public GameObject objectToEnable;
	public bool disablesOnSucess = false;

	// Use this for initialization
	void Start()
	{
		CSteamID userID = SteamUser.GetSteamID();
		//Debug.Log("My Steam ID is: " + userID);
		if (listofUsers.Contains(userID))
		{
			if (!disablesOnSucess)
			{
				objectToEnable.SetActive(true);
			}
			else
			{
				objectToEnable.SetActive(false);
			}

		}
	}
}
