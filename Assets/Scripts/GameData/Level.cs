using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

public class Level
{
    static public Level current;

    public class Item
    {
        public string name;
    }
    public List<Item> items;

    static public void Load()
    {
        if (current == null)
        {
            current = (Level)XmlManager.LoadInstanceAsXml("Xml/Board", typeof(Level));
        }
    }
}
