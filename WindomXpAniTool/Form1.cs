using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace WindomXpAniTool
{
    public partial class Form1 : Form
    {
        ani2 file = new ani2();
        List<string> recentFiles = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }

        private void loadAniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Windom XP アニメーションデータ (.ani)|*.ani";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // 一度に削除する要素をキャプチャするために別リストを使用
                List<int> indexesToRemove = new List<int>();

                for (int i = 0; i < recentFiles.Count; i++)
                {
                    if (recentFiles[i] == ofd.FileName)
                        indexesToRemove.Add(i);
                }

                // 逆順に削除することでインデックスのずれを防ぐ
                for (int i = indexesToRemove.Count - 1; i >= 0; i--)
                {
                    recentFiles.RemoveAt(indexesToRemove[i]);
                }

                recentFiles.Insert(0, ofd.FileName);

                // 要素が10を超える場合は最後の要素を削除
                if (recentFiles.Count > 10)
                    recentFiles.RemoveAt(recentFiles.Count - 1);

                updateRecent();

                try
                {
                    saveData();
                    file.load(ofd.FileName);
                    lstAnimations.Items.Clear();
                    for (int i = 0; i < file.animations.Count; i++)
                    {
                        if (file.animations[i].name == "")
                        {
                            file.animations[i].name = "Empty";
                        }
                        else
                        {
                            // 禁止文字を置換する
                            string invalidChars = @"\/:*?""<>|";
                            foreach (char c in invalidChars)
                            {
                                file.animations[i].name = file.animations[i].name.Replace(c, '@');
                            }
                        }
                        lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
                    }

                    MsgLog.Text = "成功：Aniファイルのロードが完了しました。";
                }
                catch
                {
                    MsgLog.Text = "失敗：Aniファイルがロードできませんでした。";
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (lstAnimations.SelectedItems.Count > 0 && cbScriptFormat.SelectedIndex > -1 && cbHodFormat.SelectedIndex > -1)
            {
                DirectoryInfo di = new DirectoryInfo(file._filename);
                for (int i = 0; i < lstAnimations.Items.Count; i++)
                {

                    if (lstAnimations.GetSelected(i))
                    {
                        string dir = Path.Combine(di.Parent.Name, helper.replaceUnsupportedChar(lstAnimations.Items[i].ToString()));
                        if (Directory.Exists(dir))
                        {
                            MessageBox.Show("既にアニメーションが展開されています。 " + dir, "既存のファイルを削除してから実行してください。", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                            break;
                        }
                            switch (cbScriptFormat.SelectedIndex)
                        {
                            case 0:
                                file.animations[i].extractToFolderXML(dir, cbHodFormat.SelectedIndex);
                                MsgLog.Text = "成功：アニメーションの展開が完了しました。";
                                break;
                            case 1:
                                file.animations[i].extractToFolderTXT(dir, cbHodFormat.SelectedIndex);
                                MsgLog.Text = "成功：アニメーションの展開が完了しました。";
                                break;
                        }

                    }
                }
            }
            else
            {
                if (lstAnimations.SelectedItems.Count <= 0)
                    MsgLog.Text = "警告：選択されたアイテムにアニメーションが含まれていません。";
                else
                {
                    if (cbScriptFormat.SelectedIndex < 0 && cbHodFormat.SelectedIndex < 0)
                        MsgLog.Text = "失敗：ファイルフォーマットが選択されていません。";
                    else if (cbScriptFormat.SelectedIndex < 0)
                        MsgLog.Text = "失敗：Scriptファイルのフォーマットが選択されていません。";
                    else if (cbHodFormat.SelectedIndex > 0)
                        MsgLog.Text = "失敗：Hodファイルのフォーマットが選択されていません。";
                    else
                        MsgLog.Text = "失敗：例外エラーが発生しました。";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (file != null && file.structure != null)
            {
                DirectoryInfo di = new DirectoryInfo(file._filename);

                if (cbHodFormat.SelectedIndex > -1)
                {
                    // UIスレッドでSaveToFileを呼び出す（必要な場合）
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            file.structure.SaveToFile(di.Parent.Name, cbHodFormat.SelectedIndex);
                        }));
                        MsgLog.Text = "成功：Robo.hodをフォルダに出力しました。";
                    }
                    else
                    {
                        file.structure.SaveToFile(di.Parent.Name, cbHodFormat.SelectedIndex);
                        MsgLog.Text = "成功：Robo.hodをフォルダに出力しました。";
                    }
                }
                else
                {
                    MsgLog.Text = "Hodファイルのフォーマットが選択されていません。";
                }
            }
            else
            {
                MsgLog.Text = "失敗：ファイル構造が存在しませんでした。";
            }
        }

        private void SaveAniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Windom Animation Data (.ani)|*.ani";
                DirectoryInfo di = new DirectoryInfo(file._filename);
                sfd.InitialDirectory = di.FullName;

                // UIスレッドでShowDialogを呼び出す
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            file.save(sfd.FileName);
                        }
                    }));
                    MsgLog.Text = "成功：Aniファイルを保存しました。";
                }
                else
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        file.save(sfd.FileName);
                    }
                    MsgLog.Text = "成功：Aniファイルを保存しました。";
                }
            }
            else
            {
                MsgLog.Text = "失敗：ファイルが存在しませんでした。";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(file._filename);
            for (int i = 0; i < lstAnimations.Items.Count; i++)
            {
                if (lstAnimations.GetSelected(i))
                {
                    string dir = Path.Combine(di.Parent.Name, helper.replaceUnsupportedChar(lstAnimations.Items[i].ToString()));
                    if (Directory.Exists(dir))
                    {
                        if (File.Exists(Path.Combine(dir, "script.xml"))) 
                            file.animations[i].injectFromFolderXML(dir, ref file.structure);
                        else if (File.Exists(Path.Combine(dir, "script.txt")))
                            file.animations[i].injectFromFolderTXT(dir, ref file.structure);
                    }
                }
            }

            lstAnimations.Items.Clear();
            for (int i = 0; i < file.animations.Count; i++)
            {
                lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
            }
            MsgLog.Text = "成功：アニメーションの取り込みが完了しました。";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("file._filename: " + file._filename);
            Console.WriteLine("file.structure.filename: " + file.structure.filename);

            try
            {
                DirectoryInfo di = new DirectoryInfo(file._filename);


                di = di.Parent;

                if (di == null || di.Parent == null)
                {
                    Console.WriteLine("Parent directory is null. Check the file path.");
                    return;
                }

                string parentPath = di.Name;

                // 無効な文字を置き換える
                parentPath = ReplaceInvalidPathChars(parentPath);
                string exten = "";
                string cleanedFilename = ReplaceInvalidPathChars(file.structure.filename) + exten;

                string combinedPath = Path.Combine(parentPath, cleanedFilename);
                Console.WriteLine("Combined Path: " + combinedPath);

                // ファイルが存在するかどうかチェック
                if (!File.Exists(combinedPath))
                {
                    // ファイルが存在しない場合、ダイアログを表示
                    MessageBox.Show("指定のファイルが存在しません: " + combinedPath, "ファイルが見つかりません", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                file.structure.loadFromFile(combinedPath);
                MsgLog.Text = "成功：Robo.hodをフォルダから取り込みました。";
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("ArgumentException: " + ex.Message);
            }
        }

        private string ReplaceInvalidPathChars(string path)
        {
            char[] invalidChars = Path.GetInvalidPathChars();
            foreach (char c in invalidChars)
            {
                path = path.Replace(c, '_');
            }
            return path;
        }

        private bool ContainsInvalidPathChars(string path)
        {
            char[] invalidChars = Path.GetInvalidPathChars();
            foreach (char c in path)
            {
                if (Array.Exists(invalidChars, element => element == c))
                {
                    Console.WriteLine("Invalid character found: " + c);
                    return true;
                }
            }
            return false;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstAnimations.Items.Count; i++)
                lstAnimations.SetSelected(i, true);
        }

        private void deselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstAnimations.Items.Count; i++)
                lstAnimations.SetSelected(i, false);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("Settings.txt"))
            {
                StreamReader sr = new StreamReader("Settings.txt");
                int pInt = 0;
                if (int.TryParse(sr.ReadLine(), out pInt))
                    cbScriptFormat.SelectedIndex = pInt;
                if (int.TryParse(sr.ReadLine(), out pInt))
                    cbHodFormat.SelectedIndex = pInt;
                while (sr.EndOfStream != true)
                {
                    recentFiles.Add(sr.ReadLine());
                }
                updateRecent();
                sr.Close();
            }
        }

        private void saveData()
        {
            if (file != null)
            {
                try
            {
                StreamWriter sw = new StreamWriter("Settings.txt");
                sw.WriteLine(cbScriptFormat.SelectedIndex);
                sw.WriteLine(cbHodFormat.SelectedIndex);
                for (int i = 0; i < recentFiles.Count; i++)
                    sw.WriteLine(recentFiles[i]);
                sw.Close();
            }
            catch { };
            }
            else
            {
                MsgLog.Text = "失敗：ファイルが存在しませんでした。";
            }
        }

        private void updateRecent()
        {
            if (recentFiles.Count > 0)
            {
                toolStripMenuItem2.Text = "1. " + recentFiles[0];
                toolStripMenuItem2.Visible = true;
            }
            else
                toolStripMenuItem2.Visible = false;

            if (recentFiles.Count > 1)
            {
                toolStripMenuItem3.Text = "2. " + recentFiles[1];
                toolStripMenuItem3.Visible = true;
            }
            else
                toolStripMenuItem3.Visible = false;

            if (recentFiles.Count > 2)
            {
                toolStripMenuItem4.Text = "3. " + recentFiles[2];
                toolStripMenuItem4.Visible = true;
            }
            else
                toolStripMenuItem4.Visible = false;

            if (recentFiles.Count > 3)
            {
                toolStripMenuItem5.Text = "4. " + recentFiles[3];
                toolStripMenuItem5.Visible = true;
            }
            else
                toolStripMenuItem5.Visible = false;

            if (recentFiles.Count > 4)
            {
                toolStripMenuItem6.Text = "5. " + recentFiles[4];
                toolStripMenuItem6.Visible = true;
            }
            else
                toolStripMenuItem6.Visible = false;

            if (recentFiles.Count > 5)
            {
                toolStripMenuItem7.Text = "6. " + recentFiles[5];
                toolStripMenuItem7.Visible = true;
            }
            else
                toolStripMenuItem7.Visible = false;

            if (recentFiles.Count > 6)
            {
                toolStripMenuItem8.Text = "7. " + recentFiles[6];
                toolStripMenuItem8.Visible = true;
            }
            else
                toolStripMenuItem8.Visible = false;

            if (recentFiles.Count > 7)
            {
                toolStripMenuItem9.Text = "8. " + recentFiles[7];
                toolStripMenuItem9.Visible = true;
            }
            else
                toolStripMenuItem9.Visible = false;

            if (recentFiles.Count > 8)
            {
                toolStripMenuItem10.Text = "9. " + recentFiles[8];
                toolStripMenuItem10.Visible = true;
            }
            else
                toolStripMenuItem10.Visible = false;

            if (recentFiles.Count > 9)
            {
                toolStripMenuItem11.Text = "10. " + recentFiles[9];
                toolStripMenuItem11.Visible = true;
            }
            else
                toolStripMenuItem11.Visible = false;

        }

        private void loadFileIntoRecent(string filename)
        {
            for (int i = 0; i < recentFiles.Count; i++)
            {
                if (recentFiles[i] == filename)
                    recentFiles.RemoveAt(i);
            }
            recentFiles.Insert(0, filename);
            if (recentFiles.Count > 10)
                recentFiles.RemoveAt(11);
            updateRecent();
            saveData();
        }

        //recent files
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            file.load(recentFiles[0]);
            loadFileIntoRecent(recentFiles[0]);
            lstAnimations.Items.Clear();
            for (int i = 0; i < file.animations.Count; i++)
            {
                if (file.animations[i].name == "")
                {
                    file.animations[i].name = "Empty";
                }
                else
                {
                    // 禁止文字を置換する
                    string invalidChars = @"\/:*?""<>|";
                    foreach (char c in invalidChars)
                    {
                        file.animations[i].name = file.animations[i].name.Replace(c, '@');
                    }
                }
                lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
            }

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            file.load(recentFiles[1]);
            loadFileIntoRecent(recentFiles[1]);
            lstAnimations.Items.Clear();
            for (int i = 0; i < file.animations.Count; i++)
            {
                if (file.animations[i].name == "")
                {
                    file.animations[i].name = "Empty";
                }
                else
                {
                    // 禁止文字を置換する
                    string invalidChars = @"\/:*?""<>|";
                    foreach (char c in invalidChars)
                    {
                        file.animations[i].name = file.animations[i].name.Replace(c, '@');
                    }
                }
                lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            file.load(recentFiles[2]);
            loadFileIntoRecent(recentFiles[2]);
            lstAnimations.Items.Clear();
            for (int i = 0; i < file.animations.Count; i++)
            {
                if (file.animations[i].name == "")
                {
                    file.animations[i].name = "Empty";
                }
                else
                {
                    // 禁止文字を置換する
                    string invalidChars = @"\/:*?""<>|";
                    foreach (char c in invalidChars)
                    {
                        file.animations[i].name = file.animations[i].name.Replace(c, '@');
                    }
                }
                lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            file.load(recentFiles[3]);
            loadFileIntoRecent(recentFiles[3]);
            lstAnimations.Items.Clear();
            for (int i = 0; i < file.animations.Count; i++)
            {
                if (file.animations[i].name == "")
                {
                    file.animations[i].name = "Empty";
                }
                else
                {
                    // 禁止文字を置換する
                    string invalidChars = @"\/:*?""<>|";
                    foreach (char c in invalidChars)
                    {
                        file.animations[i].name = file.animations[i].name.Replace(c, '@');
                    }
                }
                lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
            }
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            file.load(recentFiles[4]);
            loadFileIntoRecent(recentFiles[4]);
            lstAnimations.Items.Clear();
            for (int i = 0; i < file.animations.Count; i++)
            {
                if (file.animations[i].name == "")
                {
                    file.animations[i].name = "Empty";
                }
                else
                {
                    // 禁止文字を置換する
                    string invalidChars = @"\/:*?""<>|";
                    foreach (char c in invalidChars)
                    {
                        file.animations[i].name = file.animations[i].name.Replace(c, '@');
                    }
                }
                lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            file.load(recentFiles[5]);
            loadFileIntoRecent(recentFiles[5]);
            lstAnimations.Items.Clear();
            for (int i = 0; i < file.animations.Count; i++)
            {
                if (file.animations[i].name == "")
                {
                    file.animations[i].name = "Empty";
                }
                else
                {
                    // 禁止文字を置換する
                    string invalidChars = @"\/:*?""<>|";
                    foreach (char c in invalidChars)
                    {
                        file.animations[i].name = file.animations[i].name.Replace(c, '@');
                    }
                }
                lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
            }
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            file.load(recentFiles[6]);
            loadFileIntoRecent(recentFiles[6]);
            lstAnimations.Items.Clear();
            for (int i = 0; i < file.animations.Count; i++)
            {
                if (file.animations[i].name == "")
                {
                    file.animations[i].name = "Empty";
                }
                else
                {
                    // 禁止文字を置換する
                    string invalidChars = @"\/:*?""<>|";
                    foreach (char c in invalidChars)
                    {
                        file.animations[i].name = file.animations[i].name.Replace(c, '@');
                    }
                }
                lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
            }
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            file.load(recentFiles[7]);
            loadFileIntoRecent(recentFiles[7]);
            lstAnimations.Items.Clear();
            for (int i = 0; i < file.animations.Count; i++)
            {
                if (file.animations[i].name == "")
                {
                    file.animations[i].name = "Empty";
                }
                else
                {
                    // 禁止文字を置換する
                    string invalidChars = @"\/:*?""<>|";
                    foreach (char c in invalidChars)
                    {
                        file.animations[i].name = file.animations[i].name.Replace(c, '@');
                    }
                }
                lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
            }
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            file.load(recentFiles[8]);
            loadFileIntoRecent(recentFiles[8]);
            lstAnimations.Items.Clear();
            for (int i = 0; i < file.animations.Count; i++)
            {
                if (file.animations[i].name == "")
                {
                    file.animations[i].name = "Empty";
                }
                else
                {
                    // 禁止文字を置換する
                    string invalidChars = @"\/:*?""<>|";
                    foreach (char c in invalidChars)
                    {
                        file.animations[i].name = file.animations[i].name.Replace(c, '@');
                    }
                }
                lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
            }
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            if (file != null && file.animations != null && recentFiles.Count > 9)
            {
                file.load(recentFiles[9]);
                loadFileIntoRecent(recentFiles[9]);
                lstAnimations.Items.Clear();
                for (int i = 0; i < file.animations.Count; i++)
                {
                    if (file.animations[i].name == "")
                    {
                        file.animations[i].name = "Empty";
                    }
                    else
                    {
                        // 禁止文字を置換する
                        string invalidChars = @"\\/:*?""<>|";
                        foreach (char c in invalidChars)
                        {
                            file.animations[i].name = file.animations[i].name.Replace(c, '@');
                        }
                    }
                    lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
                }
            }
            else
            {
                MsgLog.Text = "警告：アニメーション、または最近使用したファイルが無効です。";
            }
        }



        private void cbScriptFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveData();
        }

        private void cbHodFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveData();
        }


        private void renameSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file != null && file.animations != null)
            {
                for (int i = 0; i < lstAnimations.Items.Count; i++)
                {
                    if (lstAnimations.GetSelected(i))
                    {
                        RenameAnimation ra = new RenameAnimation();
                        ra.setTxtName(file.animations[i].name);
                        ra.ShowDialog();
                        file.animations[i].name = ra.getTxtName();
                    }
                }

                lstAnimations.Items.Clear();
                for (int i = 0; i < file.animations.Count; i++)
                {
                    lstAnimations.Items.Add(i.ToString() + " - " + file.animations[i].name);
                }
            }
            else
            {
                MsgLog.Text = "警告：ファイル、またはアニメーションが存在しません。";
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
