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

namespace FootBallWinForms
{
    public partial class FootBall : Form
    {
        public FootBall()
        {
            InitializeComponent();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            //I don't have much knowledge about Football game. So making the following assumptions on the requirements

            //Assumption 01: The csv data file supplied has hiphen '-' as the column header for the 7th Column. 
            //Assumming this is the format expected.

            //Assumption 02: It's been mentioned in the instructions document to validate only the columns. 
            //So assuming just to validate the header row for the column headings and 
            //row data is not required to be validated and ignoring the invalid row data. 
            // The data file supplied has just dashes '-' as the 19th row data and ignoring this row.

            //Assumption 03: The requirement is to find the team name which has the smallest difference between Columns 'F' and 'A' data
            //which means the smallest absolute difference.
            
            try
            {
                //clearing the results so that multiple executions of the functionality will be showing the correct results.
                lblResult.Visible = false;
                lblResult.Text = string.Empty;

                //showing the file dialog to choose the file
                DialogResult result = openFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtDataFile.Text = openFileDialog.FileName;

                    string strFilePath = openFileDialog.FileName;

                    string[] strFilePathSplit = strFilePath.Split('\\');

                    string strFileName = @"Data\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + strFilePathSplit[strFilePathSplit.Length - 1];

                    //saving the file to the solution to have the history of the files.
                    //This step may not be required for this case. But in real applications we may want to keep the history
                    File.Copy(strFilePath, strFileName);

                    //Reading the csv file data
                    string csvData = File.ReadAllText(txtDataFile.Text);

                    DataTable dtCsvData = new DataTable();

                    dtCsvData.Columns.AddRange(new DataColumn[9] { new DataColumn("Team", typeof(string)),
                            new DataColumn("P", typeof(int)),
                            new DataColumn("W",typeof(int)),
                            new DataColumn("L",typeof(int)),
                            new DataColumn("D", typeof(int)),
                            new DataColumn("F",typeof(int)),
                            new DataColumn("-",typeof(string)),
                            new DataColumn("A", typeof(int)),
                            new DataColumn("Pts",typeof(int))
                    });

                    for (int i = 0; i < csvData.Split('\n').Length; i++)
                    {
                        string row = csvData.Split('\n')[i];
                        //assuming the first row should be the header row
                        //validating the columns names of the header row
                        if (i == 0 && !string.IsNullOrEmpty(row))//header row
                        {
                            string[] strHeaderRowSplit = row.Split(',');
                            if (strHeaderRowSplit.Length != 9)
                            {
                                MessageBox.Show("Invalid Header Row. The total number of expected columns is 9.", "FootBall", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            for (int j = 0; j < strHeaderRowSplit.Length; j++)
                            {
                                if (strHeaderRowSplit[j] != dtCsvData.Columns[j].ColumnName)
                                {
                                    MessageBox.Show(String.Format("Invalid Header Row.The column name in csv file at position {0} should be {1}", j + 1, dtCsvData.Columns[j].ColumnName), "FootBall", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            continue;//header row is valid. So continue to read the rows data.
                        }
                        if (!string.IsNullOrEmpty(row))
                        {
                            if (row.Split(',').Length != 9)
                                continue;//invalid row data. So as per the assumption skipping the invalid row

                            dtCsvData.Rows.Add();
                            int j = 0;

                            foreach (string cell in row.Split(','))
                            {
                                dtCsvData.Rows[dtCsvData.Rows.Count - 1][j] = cell.Trim();
                                j++;
                            }
                        }
                    }
                    //ordering the data table data by abosulte difference of Column's 'F' and 'A' and getting the smallest valued row.
                    var dataRow = dtCsvData.AsEnumerable().OrderBy(r => Math.Abs((r.Field<int>("F") - r.Field<int>("A")))).FirstOrDefault();
                    if (dataRow != null)
                    {
                        lblResult.Text = "The team with the smallest difference in ‘for’ and ‘against’ goals is : " + dataRow.Field<string>("Team");
                        lblResult.Visible = true;
                    }

                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("The following error occurred while processing the request. "+ex.Message, "FootBall", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
