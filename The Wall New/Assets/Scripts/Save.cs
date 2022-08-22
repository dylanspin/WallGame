using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Save 
{
    [Header("Save Paths")]
    public static string wallDataLoc = "/wallGenData.save";//where the scrapBook settings get saved
    public static string invSaveLoc = "/inventoryData.save";//where the settings get saved
    public static string settingsLoc = "/settings.save";//where the settings get saved


    ///Wall data
    public static void saveWallData(WallGen oData)///called after something is unlocked for example
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + wallDataLoc;//persistant path depends on the platform but for windows its here : %userprofile%\AppData\LocalLow\
        FileStream stream =  new FileStream(path,FileMode.Create);
        WallData data = new WallData(oData);
        formatter.Serialize(stream,data);//converts the data to be encrypted
        stream.Close();
    }

    public static WallData loadBook()//call on start so to load in the saved data
    {
        string path = Application.persistentDataPath + wallDataLoc;
        if(File.Exists(path))//if the file exist
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);
            WallData data = formatter.Deserialize(stream) as WallData;//decrypts the saved data
            stream.Close();
            return data;//returns the data from the saved BookData class
        }else{
            return null;          
        }
    }

    ///Wall data
    public static void saveInv(Inventory oData)///called after something is unlocked for example
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + wallDataLoc;//persistant path depends on the platform but for windows its here : %userprofile%\AppData\LocalLow\
        FileStream stream =  new FileStream(path,FileMode.Create);
        inventoryData data = new inventoryData(oData);
        formatter.Serialize(stream,data);//converts the data to be encrypted
        stream.Close();
    }

    public static inventoryData loadInv()//call on start so to load in the saved data
    {
        string path = Application.persistentDataPath + wallDataLoc;
        if(File.Exists(path))//if the file exist
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);
            inventoryData data = formatter.Deserialize(stream) as inventoryData;//decrypts the saved data
            stream.Close();
            return data;//returns the data from the saved BookData class
        }else{
            return null;          
        }
    }
}
