using UnityEngine;
using System.IO;
using System.Xml;
using System.Collections.Generic;

public class XmlHandler
{
    private string fileName;
    private string path;
    private int numberNode;

    public string FileName { get => fileName; set => fileName = value; }
    public string Path { get => path; set => path = value; }
    public int NumberNode { get => numberNode; set => numberNode = value; }

    public XmlHandler()
    {
        fileName = "level";
        numberNode = 2;
    }

    public List<List<string>> ReadLevel(int level)
    {
        List<List<string>> bubbleList = new List<List<string>>();

        fileName = "level";

        path = getPath() + ".xml";

        XmlReader reader = XmlReader.Create(path);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(reader);

        XmlNodeList ObjectData = xmlDoc.GetElementsByTagName("Level");
        for (int i = 0; i < ObjectData.Count; i++)
        {
            XmlNode levelNode = ObjectData.Item(i);
            var attribute = levelNode.Attributes["number"].Value;
            if (attribute != null)
            {
                int readLevel;
                int.TryParse(attribute, out readLevel);
                if (readLevel == level)
                {
                    Debug.Log("correct level = " + readLevel);
                    var rangeX = levelNode.Attributes["rangeX"].Value;
                    var rangeY = levelNode.Attributes["rangeY"].Value;
                    Vector2Int range = new Vector2Int(int.Parse(rangeX), int.Parse(rangeY));

                    XmlNodeList levelRowNodes = levelNode.ChildNodes;
                    for (int j = 0; j < range.y; j++)
                    {
                        List<string> rowList = new List<string>();
                        XmlNodeList levelBubbleNodes = levelRowNodes.Item(j).ChildNodes;
                        for (int k = 0; k < range.x; k++)
                        {
                            rowList.Add(levelBubbleNodes.Item(k).InnerText);
                        }
                        bubbleList.Add(rowList);
                    }

                    break;
                }
            }
            continue;
        }

        return bubbleList;
    }

    public void Save()
    {
        //Đường dẫn lưu file xml, tí nữa mình sẽ nói rõ về phần đường dẫn file cho các nền tảng
        path = getPath() + ".xml";
        //Tạo file XmlDocument
        XmlDocument xmlDoc = new XmlDocument();
        //Tạo 1 element
        XmlElement elmRoot = xmlDoc.CreateElement("Object");
        //Thêm element vào document
        xmlDoc.AppendChild(elmRoot);
        for (int i = 0; i < numberNode; i++)
        {
            XmlElement elmChild = xmlDoc.CreateElement("ID");
            elmChild.InnerText = "" +i;
            //Thêm element vào document
            elmRoot.AppendChild(elmChild);
        }
        //Sau khi thiết lập thông tin thì chúng ta lưu file và đóng file
        StreamWriter outStream = File.CreateText(path);
        xmlDoc.Save(outStream);
        outStream.Close();
        Debug.Log("Save game information successful");
    }
    public void Load()
    {
        //Đường dẫn file
        path = getPath() + ".xml";
        //Tải file lên
        XmlReader reader = XmlReader.Create(path);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(reader);
        //Lấy NodeList của node cha
        XmlNodeList ObjectData = xmlDoc.GetElementsByTagName("Level");
        //duyệt danh sách của node cha -> “Object”
        for (int i = 0; i < ObjectData.Count; i++)
        {
            //Lấy từng item của node cha ->”Object 1,2,3…”
            XmlNode dataChild = ObjectData.Item(i);
            //lay danh sach cua node con trong node cha
            XmlNodeList allChildNode = dataChild.ChildNodes;
            //Duyệt danh sách của node con -> “ID”
            for (int j = 0; j < allChildNode.Count; j++)
            {
                XmlNode gameObject = allChildNode.Item(j);
                //Lấy dữ liệu trong node
                Debug.Log("data in child" +i + gameObject.InnerText);
            }
        }
        //Đóng file
        reader.Close();
    }
    private string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Bubble Shooter/Resources/" + fileName;
#elif UNITY_ANDROID
        return Application.persistentDataPath + "Bubble Shooter/" + fileName;
#elif UNITY_IPHONE
        return Application.persistentDataPath + ”/Bubble Shooter/” + fileName;
#else
        return Application.dataPath + ”/Bubble Shooter/” + fileName;
#endif
    }
}

