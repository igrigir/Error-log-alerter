using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Error_Logger
{
    public partial class Form1 : Form
    {

        private const string ConfigFileName = "Config.ini";
        private string previousContent = string.Empty;
        private bool fileErrorOccurred = false;

        public Form1()
        {
            InitializeComponent();
            LoadPaths();
        }


        private void LoadPaths()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
            if (File.Exists(configPath))
            {
                var paths = new List<string>();
                var lines = File.ReadAllLines(configPath);
                bool inPathsSection = false;

                foreach (var line in lines)
                {
                    if (line.Trim().Equals("[paths]", StringComparison.OrdinalIgnoreCase))
                    {
                        inPathsSection = true;
                        continue;
                    }

                    if (inPathsSection)
                    {
                        if (line.Trim().StartsWith("["))
                        {
                            break; // End of section
                        }

                        var match = Regex.Match(line, @"^(?<key>[^=]+)=(?<value>.+)$");
                        if (match.Success)
                        {
                            paths.Add(match.Groups["value"].Value.Trim());
                        }
                    }
                }

                cboPath.DataSource = paths;

                if (paths.Count > 0)
                {
                    cboPath.SelectedIndex = 0; // Select first path
                }
            }
            else
            {
                MessageBox.Show("Config.ini file not found.");
            }
        }


        private void SavePaths(List<string> paths)
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
            using (StreamWriter writer = new StreamWriter(configPath))
            {
                writer.WriteLine("[paths]");
                foreach (var path in paths)
                {
                    writer.WriteLine($"path={path}");
                }
            }
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(cboInterval.Text) * 1000;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string newPath = cboPath.Text; // Pretpostavljamo da imaš TextBox za unos nove putanje
            var paths = cboPath.DataSource as List<string>;
            paths.Add(newPath);
            SavePaths(paths);
            LoadPaths(); // Reload paths after saving
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            btnStart.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            cboInterval.Text = "1";

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                btnStart.Enabled = false;
            }else
            {
                btnStart.Enabled = true;
            }
            
            
            string selectedPath = cboPath.Text;

            if (File.Exists(selectedPath))
            {
                string fileContent = File.ReadAllText(selectedPath);
                txtLogText.Text = fileContent;
                fileErrorOccurred = false; // Resetuj grešku

                // Proveri da li je checkBox1 uključen
                if (chkSound.Checked)
                {
                    // Ako je sadržaj različit, daj zvučnu poruku
                    if (fileContent != previousContent)
                    {
                        Console.Beep(); // Zvučna poruka
                        previousContent = fileContent; // Ažuriraj prethodni sadržaj
                    }
                }
            }
            else
            {
                if (!fileErrorOccurred) // Prikazuj grešku samo jednom
                {
                    MessageBox.Show("Fajl ne postoji: " + selectedPath);
                    fileErrorOccurred = true; // Postavi grešku
                    timer1.Enabled = false; // Stopiraj timer
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Učitaj putanju iz ComboBox
            string selectedPath = cboPath.Text;

            // Obriši sadržaj TextBox-a
            txtLogText.Clear();

            // Proveri da li fajl postoji
            if (File.Exists(selectedPath))
            {
                // Isprazni sadržaj fajla
                File.WriteAllText(selectedPath, string.Empty);
                MessageBox.Show("Sadržaj fajla je ispraznjen: " + selectedPath);
            }
            else
            {
                MessageBox.Show("Fajl ne postoji: " + selectedPath);
            }
        }

        private void cboFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Učitaj izabranu veličinu fonta iz ComboBox-a
            if (int.TryParse(cboFontSize.SelectedItem.ToString(), out int fontSize))
            {
                // Postavi font veličinu za txtLogText
                txtLogText.Font = new Font(txtLogText.Font.FontFamily, fontSize);
            }
        }
    }
}
