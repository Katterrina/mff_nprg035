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

namespace EasyPattern
{
    public partial class Form1 : Form
    {
        public readonly string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;
                                             AttachDbFilename=C:\Users\Kateřina\Desktop\C#_zapoctak\EasyPattern\EasyPattern\MeasursDatabase.mdf;
                                             Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
            measures.Visible = false;
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
                welcome.Visible = false;
                // zobrazení pdf
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
                                if (reader["len_front"] != DBNull.Value) { len_front.Value = Convert.ToDecimal(reader["len_front"]); }
                                if (reader["len_breast"] != DBNull.Value) { len_breast.Value = Convert.ToDecimal(reader["len_breast"]); }
                            }
                        }
                    }
                }
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            // todo
            
        }

        private void InsertNewSetToDatabase()
        {
            // todo

            string sql = "INSERT INTO MeasuresSets (RegionID, RegionDescription) VALUES (5, 'NorthWestern')";

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
