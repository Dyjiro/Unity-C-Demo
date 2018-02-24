/**************************

GameControl.cs

Kory Staab


***************************/

using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;

public class GameControl : MonoBehaviour 
{

	public static GameControl control;	//this control object

	GameObject player;	
	public GameObject cam;

	public List<int> inv;	//player inventory

	public int health;
	public int maxHealth;
	public int attack;
	public int defense;
	public float speed;

	public int id;
	public bool inside;

/*

Initialization for when this object springs to exsistance

*/
	void Awake () 
	{
		if(control == null)		//if this object already exists, destroy it to prevent duplicates
		{			
			DontDestroyOnLoad(gameObject);
			control = this;
		}
		else
		{
			DestroyImmediate(gameObject);
		}

		Screen.orientation = ScreenOrientation.LandscapeLeft;	//locks screen to this orientation
		//cam.GetComponent<Camera>().orthographicSize = (Screen.height/(2f * 100f));
	}

/*

Saves objects to a binary file

*/
	public void Save()
	{
		BinaryFormatter binaryFile = new BinaryFormatter();
		PlayerData playerData = new PlayerData();

		try 	//check if file can be opened
		{
			using (FileStream file = File.Create (Application.persistentDataPath + "/playerinfo.dat")) 
			{
				if(playerData != null)
				{
					player = GameObject.FindGameObjectWithTag("Player");

					if(player)
					{
						//player variables to be saved
						//the location as 3D vector
						playerData.playerposX = player.transform.position.x;
						playerData.playerposY = player.transform.position.y;
						playerData.playerposZ = player.transform.position.z;

						//player inventory
						playerData.inven = inv;

						binaryFile.Serialize(file, playerData);		//format as binary file
					}
					else
					{
						Debug.Log("ERROR: GameControl.Save, finding player object");
					}
				}
				else
				{
					Debug.Log("ERROR: GameControl.Save, opening playerData class");
				}

				file.Close ();
			}//end using filestream
		}
		catch (IOException) 
		{
			Debug.Log("ERROR: GameControl.Load, opening file");
		}
	}//end Save()


/*

Loads objects from PlayerData Class and initializes them to player

*/
	public void Load()
	{
		if(File.Exists(Application.persistentDataPath + "/playerinfo.dat"))		//check if file exists
		{				
			try 	//check if file can be opened
			{
				using (FileStream file = File.Open (Application.persistentDataPath + "/playerinfo.dat", FileMode.Open)) 
				{
					BinaryFormatter binaryFile = new BinaryFormatter();

					try 	//check if file can be opened
					{
							PlayerData playerData = (PlayerData)binaryFile.Deserialize(file);		//reads binary data from saved file					
							player = GameObject.FindGameObjectWithTag("Player");	//finds player object to populate

							if(player != null)	//Deserialize was successful and player object was found
							{								
								if(playerData != null)	//Deserialize was successful and player object was found
								{
									player.transform.position = new Vector3(playerData.playerposX, playerData.playerposY, playerData.playerposZ);		//moves player to saved postion
								}
								else
								{
									Debug.Log("ERROR: GameControl.Load, Deserialize binary file");
								}
							}
							else
							{
								Debug.Log("ERROR: GameControl.Load, finding player");
							}
						}//end using binary formatter

					catch (SerializationException) 
					{
						Debug.Log("ERROR: GameControl.Load, Deserialize binary file");
					}						

					file.Close ();
				}//end using filestream

			}
			catch (IOException) 
			{
				Debug.Log("ERROR: GameControl.Load, opening file");
			}
		}	//end check if file exists
	}	//end Load()

}

[Serializable]
class PlayerData
{
	public float playerposX,  playerposY, playerposZ;	//player location

	public List<int> inven;	//player inventory
}
