﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
namespace WindomXpAniTool
{
    struct hod2v0_Part
    {
        public int treeDepth;
        public int childCount;
        public string name;
        public Quaternion rotation;
        public Vector3 scale;
        public Vector3 position;
        public byte flag;
        public Vector3 unk;
    }
    class hod2v0
    {
        public string filename;
        public List<hod2v0_Part> parts;
        public hod2v0(string name)
        {
            filename = name;
        }

        public bool loadFromBinary(ref BinaryReader br)
        {

            //data = br.ReadBytes(11 + (partCount * 399));
            string signature = new string(br.ReadChars(3));
            int version = br.ReadInt32();
            if (signature == "HD2" && version == 0)
            {
                parts = new List<hod2v0_Part>();
                int partCount = br.ReadInt32();
                for (int i = 0; i < partCount; i++)
                {
                    hod2v0_Part nPart = new hod2v0_Part();
                    nPart.treeDepth = br.ReadInt32();
                    nPart.childCount = br.ReadInt32();
                    nPart.name = ASCIIEncoding.ASCII.GetString(br.ReadBytes(256)).TrimEnd('\0');
                    nPart.rotation = new Quaternion();
                    nPart.rotation.x = br.ReadSingle();
                    nPart.rotation.y = br.ReadSingle();
                    nPart.rotation.z = br.ReadSingle();
                    nPart.rotation.w = br.ReadSingle();
                    nPart.scale = new Vector3();
                    nPart.scale.x = br.ReadSingle();
                    nPart.scale.y = br.ReadSingle();
                    nPart.scale.z = br.ReadSingle();
                    nPart.position = new Vector3();
                    nPart.position.x = br.ReadSingle();
                    nPart.position.y = br.ReadSingle();
                    nPart.position.z = br.ReadSingle();
                    nPart.flag = br.ReadByte();
                    nPart.unk = new Vector3();
                    nPart.unk.x = br.ReadSingle();
                    nPart.unk.y = br.ReadSingle();
                    nPart.unk.z = br.ReadSingle();
                    br.BaseStream.Seek(82, SeekOrigin.Current);
                    parts.Add(nPart);
                }
            }
            else
                return false;

            return true;
        }

        public void saveToBinary(ref BinaryWriter bw)
        {
            bw.Write(ASCIIEncoding.ASCII.GetBytes("HD2"));
            bw.Write(0);
            bw.Write(parts.Count);
            for (int i = 0; i < parts.Count; i++)
            {
                bw.Write(parts[i].treeDepth);
                bw.Write(parts[i].childCount);
                byte[] text = ASCIIEncoding.ASCII.GetBytes(parts[i].name);
                bw.Write(text);
                bw.BaseStream.Seek(256 - text.Length, SeekOrigin.Current);
                bw.Write(parts[i].rotation.x);
                bw.Write(parts[i].rotation.y);
                bw.Write(parts[i].rotation.z);
                bw.Write(parts[i].rotation.w);
                bw.Write(parts[i].scale.x);
                bw.Write(parts[i].scale.y);
                bw.Write(parts[i].scale.z);
                bw.Write(parts[i].position.x);
                bw.Write(parts[i].position.y);
                bw.Write(parts[i].position.z);
                bw.Write(parts[i].flag);
                bw.Write(parts[i].unk.x);
                bw.Write(parts[i].unk.y);
                bw.Write(parts[i].unk.z);
                bw.BaseStream.Seek(82, SeekOrigin.Current);
            }
        }

        public void SaveToFile(string folder, int type)
        {
            string subFolder = "";
            string fileName = "";
            if (folder != null)
            {
                // ディレクトリーの作成
                DirectoryInfo di = Directory.CreateDirectory(folder);

                // ファイル名の作成
                string invalidChars = new string(Path.GetInvalidPathChars());
                fileName = Path.GetInvalidFileNameChars().Aggregate(filename, (current, c) => current.Replace(c.ToString(), "_"));
                if (string.Equals(fileName, "robo.hod", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = "robo";
                    subFolder = Path.Combine(folder, subFolder);
                }
                else
                {
                    // フォルダ名の作成
                    for (int i = 0; i < 100; i++)
                    {
                        subFolder = Path.Combine(folder, i.ToString("D2"));
                        if (!Directory.Exists(subFolder))
                        {
                            Directory.CreateDirectory(subFolder);
                            break;
                        }
                    }
                }
            }

            // ファイルパスの作成
            string filePath = Path.Combine(subFolder, fileName);

            if (fileName != "robo")
            {
                if (type == 0)
                {
                    // バイナリーファイルの保存
                    filePath += ".hod";
                    BinaryWriter bw = new BinaryWriter(File.Open(filePath, FileMode.CreateNew));
                    saveToBinary(ref bw);
                    bw.Close();
                }
                if (type == 1)
                {
                    // XMLファイルの保存
                    filePath += ".xml";
                    saveToXML(subFolder);
                }
            }
            if (fileName == "robo")
            {
                // ファイルが存在するかどうかチェック
                if (File.Exists(filePath + ".hod") || File.Exists(filePath + ".xml"))
                {
                    // ファイルが存在する場合、ダイアログを表示
                    System.Windows.Forms.MessageBox.Show("既にRobo.hodが展開されています。 " , "既存のファイルを削除してから実行してください。", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    return;

                }
                else
                {
                    if (type == 0)
                    {
                        // バイナリーファイルの保存
                        filePath += ".hod";
                        BinaryWriter bw = new BinaryWriter(File.Open(filePath, FileMode.CreateNew));
                        saveToBinary(ref bw);
                        bw.Close();
                        //System.Windows.Forms.MessageBox.Show("Robo.hodの展開が完了しました。" + filePath, "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    }
                    if (type == 1)
                    {
                        // XMLファイルの保存
                        filePath += ".xml";
                        saveToXML(subFolder);
                        //System.Windows.Forms.MessageBox.Show("Robo.hodの展開が完了しました。" + filePath, "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    }
                }
                
            }
        }

        public void saveToXML(string folder)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.ConformanceLevel = ConformanceLevel.Auto;
            xws.CheckCharacters = true;

            // ファイル名の作成
            string invalidChars = new string(Path.GetInvalidPathChars());
            string fileName = Path.GetInvalidFileNameChars()
                .Aggregate(filename, (current, c) => current.Replace(c.ToString(), "_")) + ".xml";
            string filePath = Path.Combine(folder, fileName);

            using (XmlWriter xw = XmlWriter.Create(filePath, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("HOD");
                for (int i = 0; i < parts.Count; i++)
                {
                    xw.WriteStartElement("Part");
                    xw.WriteAttributeString("name", XmlConvert.EncodeName(parts[i].name));
                    xw.WriteAttributeString("treeDepth", XmlConvert.ToString(parts[i].treeDepth));
                    xw.WriteAttributeString("childCount", XmlConvert.ToString(parts[i].childCount));

                    xw.WriteStartElement("Rotation");
                    xw.WriteAttributeString("x", XmlConvert.ToString(parts[i].rotation.x));
                    xw.WriteAttributeString("y", XmlConvert.ToString(parts[i].rotation.y));
                    xw.WriteAttributeString("z", XmlConvert.ToString(parts[i].rotation.z));
                    xw.WriteAttributeString("w", XmlConvert.ToString(parts[i].rotation.w));
                    xw.WriteEndElement();

                    xw.WriteStartElement("Scale");
                    xw.WriteAttributeString("x", XmlConvert.ToString(parts[i].scale.x));
                    xw.WriteAttributeString("y", XmlConvert.ToString(parts[i].scale.y));
                    xw.WriteAttributeString("z", XmlConvert.ToString(parts[i].scale.z));
                    xw.WriteEndElement();

                    xw.WriteStartElement("Position");
                    xw.WriteAttributeString("x", XmlConvert.ToString(parts[i].position.x));
                    xw.WriteAttributeString("y", XmlConvert.ToString(parts[i].position.y));
                    xw.WriteAttributeString("z", XmlConvert.ToString(parts[i].position.z));
                    xw.WriteEndElement();

                    xw.WriteStartElement("Flag");
                    xw.WriteAttributeString("value", XmlConvert.ToString(parts[i].flag));
                    xw.WriteEndElement();

                    xw.WriteStartElement("Unk");
                    xw.WriteAttributeString("x", XmlConvert.ToString(parts[i].unk.x));
                    xw.WriteAttributeString("y", XmlConvert.ToString(parts[i].unk.y));
                    xw.WriteAttributeString("z", XmlConvert.ToString(parts[i].unk.z));
                    xw.WriteEndElement();

                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
        }

        public void loadFromFile(string folderPath)
        {
            // 親ディレクトリの取得
            Console.WriteLine("親ディレクトリ取得開始");
            string parentFolderPath = Directory.GetParent(folderPath).FullName;
            // 下位ディレクトリを探索
            Console.WriteLine("下位ディレクトリ探索開始");
            for (int i = 0; i < 100; i++)
            {
                string subFolder = Path.Combine(parentFolderPath, i.ToString("D2"));
                
                if (Directory.Exists(subFolder))
                {
                    // バイナリファイルを読み込む
                    foreach (string file in Directory.GetFiles(subFolder, "*.hod"))
                    {
                        BinaryReader br = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read));
                        loadFromBinary(ref br);
                        br.Close();
                    }

                    // XMLファイルを読み込む
                    foreach (string file in Directory.GetFiles(subFolder, "*.xml"))
                    {
                        loadFromXML(file);
                    }
                }
            }
        }

        public void loadFromXML(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filepath);
            XmlNode mainNode = doc.SelectSingleNode("HOD");
            XmlNodeList partsList = mainNode.ChildNodes;
            parts.Clear();
            foreach (XmlNode part in partsList)
            {
                hod2v0_Part nPart = new hod2v0_Part();
                float pf;
                int pi;
                nPart.name = part.Attributes["name"].Value;
                if (int.TryParse(part.Attributes["treeDepth"].Value, out pi))
                    nPart.treeDepth = pi;

                if (int.TryParse(part.Attributes["childCount"].Value, out pi))
                    nPart.childCount = pi;

                XmlNode rotation = part.SelectSingleNode("Rotation");
                nPart.rotation = new Quaternion();
                if (float.TryParse(rotation.Attributes["x"].Value, out pf))
                    nPart.rotation.x = pf;
                if (float.TryParse(rotation.Attributes["y"].Value, out pf))
                    nPart.rotation.y = pf;
                if (float.TryParse(rotation.Attributes["z"].Value, out pf))
                    nPart.rotation.z = pf;
                if (float.TryParse(rotation.Attributes["w"].Value, out pf))
                    nPart.rotation.w = pf;

                XmlNode scale = part.SelectSingleNode("Scale");
                nPart.scale = new Vector3();
                if (float.TryParse(scale.Attributes["x"].Value, out pf))
                    nPart.scale.x = pf;
                if (float.TryParse(scale.Attributes["y"].Value, out pf))
                    nPart.scale.y = pf;
                if (float.TryParse(scale.Attributes["z"].Value, out pf))
                    nPart.scale.z = pf;

                XmlNode position = part.SelectSingleNode("Position");
                nPart.position = new Vector3();
                if (float.TryParse(position.Attributes["x"].Value, out pf))
                    nPart.position.x = pf;
                if (float.TryParse(position.Attributes["y"].Value, out pf))
                    nPart.position.y = pf;
                if (float.TryParse(position.Attributes["z"].Value, out pf))
                    nPart.position.z = pf;

                XmlNode flag = part.SelectSingleNode("Flag");
                byte f;
                if (byte.TryParse(flag.Attributes["value"].Value, out f))
                    nPart.flag = f;

                XmlNode unk = part.SelectSingleNode("Unk");
                nPart.unk = new Vector3();
                if (float.TryParse(unk.Attributes["x"].Value, out pf))
                    nPart.unk.x = pf;
                if (float.TryParse(unk.Attributes["y"].Value, out pf))
                    nPart.unk.y = pf;
                if (float.TryParse(unk.Attributes["z"].Value, out pf))
                    nPart.unk.z = pf;

                parts.Add(nPart);
            }
        }
    }
}
