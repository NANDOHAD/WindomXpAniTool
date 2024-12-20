using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace WindomXpAniTool
{
    struct hod2v1_Part
    {
        public string name;
        public int treeDepth;
        public int childCount;
        public Quaternion rotation;
        public Vector3 scale;
        public Vector3 position;
        public Quaternion unk1;
        public Quaternion unk2;
        public Quaternion unk3;
    }
    class hod2v1
    {
        public string filename;
        public byte[] data;
        public List<hod2v1_Part> parts;
        public hod2v1(string name)
        {
            filename = name;
        }

        public bool loadFromBinary(ref BinaryReader br, ref hod2v0 structure)
        {
            //Console.WriteLine("バイナリ読み込み開始");
            //data = br.ReadBytes(11 + (partCount * 179));
            string signature = new string(br.ReadChars(3));
            int version = br.ReadInt32();
            if (signature == "HD2" && version == 1)
            {
                //Console.WriteLine("HODファイルがV2です");
                parts = new List<hod2v1_Part>();
                int partCount = br.ReadInt32();
                for (int i = 0; i < partCount; i++)
                {
                    hod2v1_Part nPart = new hod2v1_Part();
                    nPart.name = structure.parts[i].name;
                    nPart.treeDepth = br.ReadInt32();
                    nPart.childCount = br.ReadInt32();
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
                    nPart.unk1 = new Quaternion();
                    nPart.unk1.x = br.ReadSingle();
                    nPart.unk1.y = br.ReadSingle();
                    nPart.unk1.z = br.ReadSingle();
                    nPart.unk1.w = br.ReadSingle();
                    nPart.unk2 = new Quaternion();
                    nPart.unk2.x = br.ReadSingle();
                    nPart.unk2.y = br.ReadSingle();
                    nPart.unk2.z = br.ReadSingle();
                    nPart.unk2.w = br.ReadSingle();
                    nPart.unk3 = new Quaternion();
                    nPart.unk3.x = br.ReadSingle();
                    nPart.unk3.y = br.ReadSingle();
                    nPart.unk3.z = br.ReadSingle();
                    nPart.unk3.w = br.ReadSingle();
                    br.BaseStream.Seek(83, SeekOrigin.Current);
                    parts.Add(nPart);
                }
            }
            else
                return false;

            return true;
        }

        public void SaveToBinary(ref BinaryWriter bw)
        {

            //bw.Write(data);
            bw.Write(ASCIIEncoding.ASCII.GetBytes("HD2"));
            bw.Write(1);
            bw.Write(parts.Count);
            for (int i = 0; i < parts.Count; i++)
            {
                bw.Write(parts[i].treeDepth);
                bw.Write(parts[i].childCount);
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
                bw.Write(parts[i].unk1.x);
                bw.Write(parts[i].unk1.y);
                bw.Write(parts[i].unk1.z);
                bw.Write(parts[i].unk1.w);
                bw.Write(parts[i].unk2.x);
                bw.Write(parts[i].unk2.y);
                bw.Write(parts[i].unk2.z);
                bw.Write(parts[i].unk2.w);
                bw.Write(parts[i].unk3.x);
                bw.Write(parts[i].unk3.y);
                bw.Write(parts[i].unk3.z);
                bw.Write(parts[i].unk3.w);
                bw.BaseStream.Seek(83, SeekOrigin.Current);
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
                    //filePath += ".hod";
                    BinaryWriter bw = new BinaryWriter(File.Open(filePath, FileMode.CreateNew));
                    SaveToBinary(ref bw);
                    bw.Close();

                }
                if (type == 1)
                {
                    // XMLファイルの保存
                    //filePath += ".xml";
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
                        SaveToBinary(ref bw);
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
                    xw.WriteAttributeString("name", parts[i].name);
                    xw.WriteAttributeString("treeDepth", parts[i].treeDepth.ToString());
                    xw.WriteAttributeString("childCount", parts[i].childCount.ToString());

                    xw.WriteStartElement("Rotation");
                    xw.WriteAttributeString("x", parts[i].rotation.x.ToString());
                    xw.WriteAttributeString("y", parts[i].rotation.y.ToString());
                    xw.WriteAttributeString("z", parts[i].rotation.z.ToString());
                    xw.WriteAttributeString("w", parts[i].rotation.w.ToString());
                    xw.WriteEndElement();

                    xw.WriteStartElement("Scale");
                    xw.WriteAttributeString("x", parts[i].scale.x.ToString());
                    xw.WriteAttributeString("y", parts[i].scale.y.ToString());
                    xw.WriteAttributeString("z", parts[i].scale.z.ToString());
                    xw.WriteEndElement();

                    xw.WriteStartElement("Position");
                    xw.WriteAttributeString("x", parts[i].position.x.ToString());
                    xw.WriteAttributeString("y", parts[i].position.y.ToString());
                    xw.WriteAttributeString("z", parts[i].position.z.ToString());
                    xw.WriteEndElement();

                    xw.WriteStartElement("Unk1");
                    xw.WriteAttributeString("x", parts[i].unk1.x.ToString());
                    xw.WriteAttributeString("y", parts[i].unk1.y.ToString());
                    xw.WriteAttributeString("z", parts[i].unk1.z.ToString());
                    xw.WriteAttributeString("w", parts[i].unk1.w.ToString());
                    xw.WriteEndElement();

                    xw.WriteStartElement("Unk2");
                    xw.WriteAttributeString("x", parts[i].unk2.x.ToString());
                    xw.WriteAttributeString("y", parts[i].unk2.y.ToString());
                    xw.WriteAttributeString("z", parts[i].unk2.z.ToString());
                    xw.WriteAttributeString("w", parts[i].unk2.w.ToString());
                    xw.WriteEndElement();

                    xw.WriteStartElement("Unk3");
                    xw.WriteAttributeString("x", parts[i].unk3.x.ToString());
                    xw.WriteAttributeString("y", parts[i].unk3.y.ToString());
                    xw.WriteAttributeString("z", parts[i].unk3.z.ToString());
                    xw.WriteAttributeString("w", parts[i].unk3.w.ToString());
                    xw.WriteEndElement();

                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
        }

        public void loadFromFile2(string filePath, ref hod2v0 structure)
        {
            BinaryReader brr = new BinaryReader(File.Open(filePath, FileMode.Open));
            Console.WriteLine("ファイルパス: " + filePath);
            try
            {
                //using (BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    //Console.WriteLine("ファイルパス: " + filePath);
                    //string signature = new string(br.ReadChars(3));
                    //Console.WriteLine("ファイルシグネチャ: " + signature);
                    //if (signature != "HD2")
                    //{
                    //    Console.WriteLine("無効なHODファイル: " + filePath);
                        //return false;
                    //}
                    
                    loadFromBinary(ref brr, ref structure);
                    brr.Close();
                    //parts = new List<hod2v1_Part>();
                    //int partCount = br.ReadInt32();
                    //Console.WriteLine("パート数: " + partCount);
                    //for (int i = 0; i < partCount; i++)
                    //{
                    //    hod2v1_Part part = new hod2v1_Part();
                        // 各プロパティの読み込み
                    //     part.name = br.ReadString();
                    //    parts.Add(part);
                    //    Console.WriteLine("パート " + i + " を読み込みました: " + part.name);
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HODファイルの読み込み中にエラーが発生しました: " + ex.Message);
                //return false;
            }

            //return true;
        }


        public void loadFromFile(string folderPath, ref hod2v0 structure)
        {
            // 親ディレクトリの取得
            string parentFolderPath = Directory.GetParent(folderPath).FullName;
            Console.WriteLine("親ディレクトリ: " + parentFolderPath);
            // 下位ディレクトリを探索
            //Console.WriteLine("下位ディレクトリ探索開始");
            //for (int i = 0; i < 100; i++)
            //{
                //string subFolder = Path.Combine(parentFolderPath, i.ToString("D2"));

                //if (Directory.Exists(folderPath))
                //{
                    // バイナリファイルを読み込む
                    if (folderPath.EndsWith(".hod"))
                    {
                        Console.WriteLine("HODファイル発見");
                        BinaryReader br = new BinaryReader(File.Open(folderPath, FileMode.Open, FileAccess.Read));
                        loadFromBinary(ref br, ref structure);
                        br.Close();
                        Console.WriteLine("書き込み完了: " + folderPath);
                    }

                    // XMLファイルを読み込む
                    if (folderPath.EndsWith(".xml"))
                    {
                        Console.WriteLine("XMLファイル発見");
                        loadFromXML(folderPath);
                        Console.WriteLine("書き込み完了: " + folderPath);
                    }
                //}
            //}
        }


        public void loadFromXML(string filepath)
        {
            filename = filename.Replace(".xml", "");
            XmlDocument doc = new XmlDocument();
            doc.Load(filepath);
            XmlNode mainNode = doc.SelectSingleNode("HOD");
            XmlNodeList partsList = mainNode.ChildNodes;
            parts = new List<hod2v1_Part>();
            foreach (XmlNode part in partsList)
            {
                hod2v1_Part nPart = new hod2v1_Part();
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

                XmlNode unk1 = part.SelectSingleNode("Unk1");
                nPart.unk1 = new Quaternion();
                if (float.TryParse(unk1.Attributes["x"].Value, out pf))
                    nPart.unk1.x = pf;
                if (float.TryParse(unk1.Attributes["y"].Value, out pf))
                    nPart.unk1.y = pf;
                if (float.TryParse(unk1.Attributes["z"].Value, out pf))
                    nPart.unk1.z = pf;
                if (float.TryParse(unk1.Attributes["w"].Value, out pf))
                    nPart.unk1.w = pf;

                XmlNode unk2 = part.SelectSingleNode("Unk2");
                nPart.unk2 = new Quaternion();
                if (float.TryParse(unk2.Attributes["x"].Value, out pf))
                    nPart.unk2.x = pf;
                if (float.TryParse(unk2.Attributes["y"].Value, out pf))
                    nPart.unk2.y = pf;
                if (float.TryParse(unk2.Attributes["z"].Value, out pf))
                    nPart.unk2.z = pf;
                if (float.TryParse(unk2.Attributes["w"].Value, out pf))
                    nPart.unk2.w = pf;

                XmlNode unk3 = part.SelectSingleNode("Unk3");
                nPart.unk3 = new Quaternion();
                if (float.TryParse(unk3.Attributes["x"].Value, out pf))
                    nPart.unk3.x = pf;
                if (float.TryParse(unk3.Attributes["y"].Value, out pf))
                    nPart.unk3.y = pf;
                if (float.TryParse(unk3.Attributes["z"].Value, out pf))
                    nPart.unk3.z = pf;
                if (float.TryParse(unk3.Attributes["w"].Value, out pf))
                    nPart.unk3.w = pf;

                parts.Add(nPart);
            }
        }
    }
}
