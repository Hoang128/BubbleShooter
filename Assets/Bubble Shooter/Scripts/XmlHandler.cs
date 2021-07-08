using UnityEngine;
using System.IO;
using System.Xml;
using System.Collections.Generic;

public class XmlHandler
{
    private string fileName;

    public string FileName { get => fileName; set => fileName = value; }

    public XmlHandler()
    {
        fileName = "level";
    }

    public int[] LoadLevelRandomColor(int level, int levelSub)
    {
        fileName = "Level_" + level + "_" + levelSub;

        TextAsset reader = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(reader.text);
        XmlNodeList levelDataList = xmlDoc.GetElementsByTagName("Level");
        XmlNode levelData = levelDataList[0];

        int[] rColorEnable = new int[6];

        string attRRC = levelData.Attributes["rRC"].Value;
        string[] attRRCs = attRRC.Split(',');
        int resultAttRRC;
        int.TryParse(attRRCs[0], out resultAttRRC);

        string attRBC = levelData.Attributes["rBC"].Value;
        string[] attRBCs = attRBC.Split(',');
        int resultAttRBC;
        int.TryParse(attRBCs[0], out resultAttRBC);

        string attRGC = levelData.Attributes["rGC"].Value;
        string[] attRGCs = attRGC.Split(',');
        int resultAttRGC;
        int.TryParse(attRGCs[0], out resultAttRGC);

        string attRYC = levelData.Attributes["rYC"].Value;
        string[] attRYCs = attRYC.Split(',');
        int resultAttRYC;
        int.TryParse(attRYCs[0], out resultAttRYC);

        string attRPuC = levelData.Attributes["rPuC"].Value;
        string[] attRPuCs = attRPuC.Split(',');
        int resultAttRPuC;
        int.TryParse(attRPuCs[0], out resultAttRPuC);

        string attRPiC = levelData.Attributes["rPiC"].Value;
        string[] attRPiCs = attRPiC.Split(',');
        int resultAttRPiC;
        int.TryParse(attRPiCs[0], out resultAttRPiC);

        rColorEnable[0] = resultAttRBC;
        rColorEnable[1] = resultAttRRC;
        rColorEnable[2] = resultAttRYC;
        rColorEnable[3] = resultAttRGC;
        rColorEnable[4] = resultAttRPuC;
        rColorEnable[5] = resultAttRPiC;

        return rColorEnable;
    }

    public List<BubbleMapMgr.BubbleData> LoadBubbleLevelMap(int level, int levelSub)
    {
        List<BubbleMapMgr.BubbleData> bubbleList = new List<BubbleMapMgr.BubbleData>();

        fileName = "Level_" + level + "_" + levelSub;

        TextAsset reader = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(reader.text);

        XmlNodeList objectData = xmlDoc.GetElementsByTagName("Bubble");
        for (int i = 0; i < objectData.Count; i++)
        {
            XmlNode bubbleNode = objectData.Item(i);
            var attX = bubbleNode.Attributes["x"].Value;
            int resultAttX;
            int.TryParse(attX, out resultAttX);
            var attY = bubbleNode.Attributes["y"].Value;
            int resultAttY;
            int.TryParse(attY, out resultAttY);
            var attType = bubbleNode.Attributes["tId"].Value;
            int resultAttType;
            int.TryParse(attType, out resultAttType);
            var attKeyType = bubbleNode.Attributes["nBST"].Value;
            int resultAttKeyType;
            int.TryParse(attKeyType, out resultAttKeyType);
            var attRandomType = bubbleNode.Attributes["rBId"].Value;
            int resultAttRandomType;
            int.TryParse(attRandomType, out resultAttRandomType);

            BubbleMapMgr.BubbleData bubbleData = new BubbleMapMgr.BubbleData();
            bubbleData.GridPos = new Vector2Int(resultAttX, resultAttY);
            bubbleData.TypeNum = resultAttType;
            bubbleData.TypeKey = resultAttKeyType;
            bubbleData.RandomType = resultAttRandomType;

            bubbleList.Add(bubbleData);
        }

        return bubbleList;
    }
}

