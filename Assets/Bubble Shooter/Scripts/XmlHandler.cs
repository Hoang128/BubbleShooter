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

    public List<BubbleMapMgr.BubbleData> LoadBubbleLevel(int level, int levelSub)
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

            BubbleMapMgr.BubbleData bubbleData = new BubbleMapMgr.BubbleData();
            bubbleData.GridPos = new Vector2Int(resultAttX, resultAttY);
            bubbleData.TypeNum = resultAttType;
            bubbleData.TypeKey = resultAttKeyType;

            bubbleList.Add(bubbleData);
        }

        return bubbleList;
    }
}

