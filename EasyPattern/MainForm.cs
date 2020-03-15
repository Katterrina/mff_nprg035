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
    public struct measures
    public partial class MainForm : Form
    {
        public readonly string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;
                                             AttachDbFilename=C:\Users\Kateřina\Desktop\C#_zapoctak\EasyPattern\EasyPattern\MeasursDatabase.mdf;
                                             Integrated Security=True";
        public MainForm()
        {
            InitializeComponent();
            measures.Visible = false;
            patternChoice.Visible = false;
            viewer.Visible = false;
            loadMeasureChoice();
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
                measures.Visible = true;
                nameAndNoteMeasures.Visible = false;


                if ((string)choiceMeasuresSet.SelectedItem != null && (string)choiceMeasuresSet.SelectedItem != "")
                {
                    PrefillForm((string)choiceMeasuresSet.SelectedItem);
                    nameAndNoteMeasures.Visible = true;
                }
            }

            else
            {
                if ((string)choiceMeasuresSet.SelectedItem == null || (string)choiceMeasuresSet.SelectedItem == "")
                {
                    MessageBox.Show("Vyberte prosím z uložených sad nebo si vytvořte novou.", "Neplatná hodnota", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    welcome.Visible = false;
                    patternChoice.Visible = true;
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

            if (popup.DialogResult == DialogResult.Cancel)
            {
                popup.Dispose();
            }

            if (popup.DialogResult == DialogResult.OK)
            {
                string name = popup.nameOfMSet.Text;
                string note = popup.note.Text;
                PatternGeometry.measures = new MeasuresData();
                InsertDataSetToDatabase(name, note, PatternGeometry.measures);
            }

            measures.Visible = false;
            patternChoice.Visible = true;
        }

        private void InsertDataSetToDatabase(string name, string note, MeasuresData m)
        {
            string sql = $"INSERT INTO MeasuresSets " +
                $"(name, note, height, circ_bust, circ_waist, circ_hips, len_back, wid_back, len_knee, " +
                $"len_shoulder, len_sleeve, circ_neck, circ_sleeve, len_front, len_breast)" +
                $" VALUES " +
                $"('{name}','{note}',{m.height},{m.circ_bust},{m.circ_waist},{m.circ_hips},{m.len_back},{m.wid_back},{m.len_knee}," +
                $"{m.len_shoulder},{m.len_sleeve},{m.circ_neck},{m.circ_sleeve},{m.len_front},{m.len_breast})";

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void toWlecome_Click(object sender, EventArgs e)
        {
            measures.Visible = false;
            welcome.Visible = true;
        }

        private void backWelcome_Click(object sender, EventArgs e)
        {
            patternChoice.Visible = false;
            welcome.Visible = true;
            loadMeasureChoice();
        }

        private void next2_Click(object sender, EventArgs e)
        {
            measures.Visible = false;
            patternChoice.Visible = true;
        }

        private void doPattern_Click(object sender, EventArgs e)
        {
            patternChoice.Visible = false;
            viewer.Visible = true;



            PdfPage page = PatternGeometry.CreatePdfPage();
            PatternGeometry.Skirt(700, 300, page);


        }
    }

}
