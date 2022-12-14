using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ModBaseMaker.Properties;

namespace ModBaseMaker
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (FolderDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = FolderDialog.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (FolderDialog.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = FolderDialog.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("One Of The Above Required Fields Is Missing!", "Error");

                return;
            }

            if (!File.Exists(textBox1.Text + "\\ChilloutVR_Data\\Managed\\Assembly-CSharp.dll"))
            {
                MessageBox.Show("Invalid ChilloutVR Folder!", "Error");

                return;
            }

            if (!Directory.Exists(textBox1.Text + "\\MelonLoader"))
            {
                MessageBox.Show("MelonLoader Is Not Installed!", "Error");

                return;
            }

            if (!File.Exists(textBox1.Text + "\\Mods\\ChilloutButtonAPI.dll"))
            {
                MessageBox.Show("ChilloutButtonAPI Is Not Installed!", "Error");

                return;
            }

            // Write ModBase.zip To Output Temporarily
            WriteResourceToFile("ModBaseMaker.Resources.ModBase.zip", textBox2.Text + "\\ModBase.zip");

            // Create Temp Folder For Editing
            var TempDir = textBox2.Text + "\\ModBaseTempDir";

            if (!Directory.Exists(TempDir))
            {
                Directory.CreateDirectory(TempDir);
            }

            ZipFile.ExtractToDirectory(textBox2.Text + "\\ModBase.zip", TempDir); // ModBaseTempDir/ModBase/projfiles

            File.Delete(textBox2.Text + "\\ModBase.zip");

            var ProjDir = TempDir + "\\ModBase";

            File.WriteAllText(ProjDir + "\\Internal - Do Not Code Here!\\MainClass.cs",
                File.ReadAllText(ProjDir + "\\Internal - Do Not Code Here!\\MainClass.cs")
                .Replace("\"ModBase", $"\"{textBox3.Text}")
                .Replace("f>ModBase", $"f>{textBox3.Text}")
                .Replace(@"\\ModBase", $@"\\{textBox3.Text}")
                .Replace("ModBase", $"{textBox3.Text.Replace(" ", "")}")
                .Replace("AuthorName", textBox4.Text));

            File.WriteAllText(ProjDir + "\\Internal - Do Not Code Here!\\Misc Classes.cs",
                File.ReadAllText(ProjDir + "\\Internal - Do Not Code Here!\\Misc Classes.cs")
                    .Replace("ModBase", $"{textBox3.Text.Replace(" ", "")}"));

            File.WriteAllText(ProjDir + "\\Modules - Code Here!\\ExampleModule.cs",
                File.ReadAllText(ProjDir + "\\Modules - Code Here!\\ExampleModule.cs")
                    .Replace("ModBase", $"{textBox3.Text.Replace(" ", "")}"));

            File.WriteAllText(ProjDir + "\\Properties\\AssemblyInfo.cs",
                File.ReadAllText(ProjDir + "\\Properties\\AssemblyInfo.cs")
                    .Replace("\"ModBase", $"\"{textBox3.Text}")
                    .Replace("AuthorName", textBox4.Text));

            var ToDir = textBox1.Text.EndsWith("/") ? textBox1.Text.Substring(0, textBox1.Text.Length - 1) : textBox1.Text; // To Combat Leading /

            File.WriteAllText(ProjDir + "\\ModBase.csproj",
                File.ReadAllText(ProjDir + "\\ModBase.csproj")
                    .Replace("ModBase", $"{textBox3.Text.Replace(" ", "")}")
                    .Replace("..\\..\\Games\\SteamLibrary\\steamapps\\common\\ChilloutVR", ToDir));

            File.Move(ProjDir + "\\ModBase.csproj", ProjDir + $"\\{textBox3.Text.Replace(" ", "")}.csproj");

            File.WriteAllText(ProjDir + "\\ModBase.sln",
                File.ReadAllText(ProjDir + "\\ModBase.sln")
                    .Replace("ModBase", $"{textBox3.Text.Replace(" ", "")}"));

            File.Move(ProjDir + "\\ModBase.sln", ProjDir + $"\\{textBox3.Text.Replace(" ", "")}.sln");

            //Cleanup, Finishing Actions
            Directory.Move(ProjDir, textBox2.Text + $"\\{textBox3.Text.Replace(" ", "")}");

            Directory.Delete(TempDir);

            MessageBox.Show("Done!", "Success");
        }

        public void WriteResourceToFile(string resourceName, string fileName)
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource?.CopyTo(file);
                }
            }
        }
    }
}
