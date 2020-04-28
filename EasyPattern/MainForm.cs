using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;

namespace EasyPattern
{
    public partial class MainForm : Form
    {
        public readonly string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;
                                             AttachDbFilename=C:\Users\Kateřina\Desktop\C#_zapoctak\EasyPattern\EasyPattern\MeasursDatabase.mdf;
                                             Integrated Security=True";
        public MainForm()
        {
            InitializeComponent();
            measuresPanel.Visible = false;
            patternChoicePanel.Visible = false;
            viewerPanel.Visible = false;
            loadMeasureChoice();
            loadPatternChoice();
        }

        private void loadPatternChoice()
        {
            patternToDo.DataSource = Enum.GetValues(typeof(PatternControl.Pattern))
                                         .Cast<Enum>()
                                         .Select(value => new
                                         {
                                            (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description,
                                            value
                                         })
                                         .OrderBy(item => item.value)
                                         .ToList();
            patternToDo.DisplayMember = "Description";
            patternToDo.ValueMember = "value";
        }

        private void loadMeasureChoice()
        {
            choiceMeasuresSet.Items.Clear();

            string sql = "SELECT name FROM MeasuresSets";

            choiceMeasuresSet.Items.Add("");

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                choiceMeasuresSet.Items.Add(reader["name"]);
                            }
                        }
                    }
                }
            }
        }

        private void next1_Click(object sender, EventArgs e)
        {
            if (updateMeasures.Checked)
            {
                welcome.Visible = false;
                measuresPanel.Visible = true;
                nameAndNoteMeasures.Visible = false;


                if ((string)choiceMeasuresSet.SelectedItem != null && (string)choiceMeasuresSet.SelectedItem != "")
                {
                    PrefillForm((string)choiceMeasuresSet.SelectedItem);
                    nameAndNoteMeasures.Visible = true;
                }
            }

            else
            {
                string selected = (string)choiceMeasuresSet.SelectedItem;
                if (selected == null || selected == "")
                {
                    MessageBox.Show("Vyberte prosím z uložených sad nebo si vytvořte novou.", "Neplatná hodnota", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    PrefillForm(selected);
                    welcome.Visible = false;
                    patternChoicePanel.Visible = true;
                }
                    
            }
            
        }

        private void PrefillForm(string nameOfMeasuresSet)
        {
            string sql = $"SELECT * FROM MeasuresSets WHERE name='{nameOfMeasuresSet}'";

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                nameOfDataSet.Text = (string)reader["name"];
                                noteDataSet.Text = (string)reader["note"];
                                height.Value = Convert.ToDecimal(reader["height"]);
                                circ_bust.Value = Convert.ToDecimal(reader["circ_bust"]);
                                circ_waist.Value = Convert.ToDecimal(reader["circ_waist"]);
                                circ_hips.Value = Convert.ToDecimal(reader["circ_hips"]);
                                len_back.Value = Convert.ToDecimal(reader["len_back"]);
                                wid_back.Value = Convert.ToDecimal(reader["wid_back"]);
                                len_knee.Value = Convert.ToDecimal(reader["len_knee"]);
                                len_shoulder.Value = Convert.ToDecimal(reader["len_shoulder"]);
                                len_sleeve.Value = Convert.ToDecimal(reader["len_sleeve"]);
                                circ_neck.Value = Convert.ToDecimal(reader["circ_neck"]);
                                circ_sleeve.Value = Convert.ToDecimal(reader["circ_sleeve"]);
                                len_front.Value = Convert.ToDecimal(reader["len_front"]);
                                len_breast.Value = Convert.ToDecimal(reader["len_breast"]);
                                len_hips.Value = Convert.ToDecimal(reader["len_hips"]);
                            }
                        }
                    }
                }
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            SaveDialogForm popup = new SaveDialogForm();
            popup.ShowDialog();

            if (popup.DialogResult == DialogResult.OK)
            {
                string name = popup.nameOfMSet.Text;
                string note = popup.note.Text;

                next2_Click(sender, e);
                InsertDataSetToDatabase(name, note, fromFormToMeasures());
            }

            else
            {
                popup.Dispose();
                MessageBox.Show("Ukládání nebylo dokončeno!", "Neuloženo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void InsertDataSetToDatabase(string name, string note, MeasuresData m)
        {
            string sql = $"INSERT INTO MeasuresSets " +
                $"(name, note, height, circ_bust, circ_waist, circ_hips, len_back, wid_back, len_knee, " +
                $"len_shoulder, len_sleeve, circ_neck, circ_sleeve, len_front, len_breast, len_hips)" +
                $" VALUES " +
                $"('{name}','{note}',{m.height},{m.circ_bust},{m.circ_waist},{m.circ_hips},{m.len_back},{m.wid_back},{m.len_knee}," +
                $"{m.len_shoulder},{m.len_sleeve},{m.circ_neck},{m.circ_sleeve},{m.len_front},{m.len_breast},{m.len_hips})";

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void toWelcome1_Click(object sender, EventArgs e)
        {
            measuresPanel.Visible = false;
            welcome.Visible = true;
            patternChoicePanel.Visible = false;
            viewerPanel.Visible = false;
            loadMeasureChoice();
        }


        private void next2_Click(object sender, EventArgs e)
        {
            measuresPanel.Visible = false;
            patternChoicePanel.Visible = true;
        }
        private void backToPatternChoice_Click(object sender, EventArgs e)
        {
            patternChoicePanel.Visible = true;
            viewerPanel.Visible = false;
        }

        private MeasuresData fromFormToMeasures()
        {
            return new MeasuresData((int)height.Value, (int)circ_bust.Value, (int)circ_waist.Value,
                                                (int)circ_hips.Value, (int)len_back.Value, (int)wid_back.Value,
                                                (int)len_knee.Value, (int)len_shoulder.Value, (int)len_sleeve.Value,
                                                (int)circ_neck.Value, (int)circ_sleeve.Value, (int)len_front.Value,
                                                (int)len_breast.Value, (int)len_hips.Value);
        }

        private void doPattern_Click(object sender, EventArgs e)
        {
           
            PatternControl control = new PatternControl(fromFormToMeasures(), drawNet.Checked);

            patternChoicePanel.Visible = false;
            measuresPanel.Visible = false;
            viewerPanel.Visible = true;

            folderBrowserDialog.ShowDialog();
            string path = folderBrowserDialog.SelectedPath;
            string viewPath = control.PdfPattern((PatternControl.Pattern)patternToDo.SelectedIndex, path);


            pdfViewer.Document = Patagames.Pdf.Net.PdfDocument.Load(viewPath);
        }

        private void end_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

}
