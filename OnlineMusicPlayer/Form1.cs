using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Diagnostics;

namespace OnlineMusicPlayer
{
    public partial class Form1 : Form
    {
        MySqlConnection conn;
        List<String> listName = new List<String>();
        List<String> listURL = new List<String>();
        public Form1()
        {
            InitializeComponent();
            string constr;
            constr = "server = HOSTNAME; port = 3306; uid = USERNAME; pwd = PASSWORD; database = DATABASE; OldGuids=True;";
            conn = new MySqlConnection(constr);
            conn.Open();
            MySqlCommand createTable = new MySqlCommand("CREATE TABLE IF NOT EXISTS `music` (" +
            "`username` VARCHAR(30)," +
            "`name` VARCHAR(60)," +
            "`url` TINYTEXT" + ")" , conn);
            MySqlDataReader reader = createTable.ExecuteReader();
            conn.Close();
            //Console.WriteLine("Connection Open!");
            conn.Open();
            string sql = "SELECT * FROM music";
            var cmd = new MySqlCommand(sql, conn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                songList.Items.Add(reader.GetString(1));
                listName.Add(reader.GetString(1));
                listURL.Add(reader.GetString(2));
            }
            conn.Close();

        }
        private void songName_Click(object sender, EventArgs e)
        {
            if(songName.Text == "Song Name")
            {
                songName.Text = "";
            }
        }
        private void songURL_Click(object sender, EventArgs e)
        {
            if (songName.Text == "Song Name")
            {
                songName.Text = "";
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void songList_SelectedIndexChanged(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = listURL[songList.SelectedIndex];
        }

        private void button1_Click(object sender, EventArgs e)
        {

            String name = songName.Text;
            String url = songURL.Text;
            if (String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(url))
            {
                string title = "Error";
                string message = "Please enter both the name and the URL";
                MessageBox.Show(message, title);
            }
            else
            {
                
                //add to db, open then close conn
                string last3 = url.Substring(url.Length - 4);
                if (last3.ToLower() == ".mp3" || last3.ToLower() == ".m4a")
                {
                    songList.Items.Add(name);
                    listName.Add(name);
                    listURL.Add(url);
                    conn.Open();
                    String query = "INSERT INTO music(username,name,url) VALUES (@username,@name, @url)";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.Parameters.Add("@username", MySqlDbType.VarString).Value = "test";
                    command.Parameters.Add("@name", MySqlDbType.VarString).Value = name;
                    command.Parameters.Add("@url", MySqlDbType.VarString).Value = url;
                    command.ExecuteNonQuery();
                    conn.Close();
                }
                else
                {
                    string title = "Invalid URL";
                    string message = "Only .mp3/.m4a allowed";
                    MessageBox.Show(message, title);
                }
            }
        }

        public void updateToNext(int index)
        {
            int next_index = (index + 1)%songList.Items.Count;
            songList.SelectedIndex = next_index;
            axWindowsMediaPlayer1.URL = listURL[next_index];
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int indexToDelete = songList.SelectedIndex;
            String name = listName[indexToDelete];
            if (!name.Contains("DEFAULT"))
            {
                updateToNext(songList.SelectedIndex);
                listName.RemoveAt(indexToDelete);
                listURL.RemoveAt(indexToDelete);
                songList.Items.RemoveAt(indexToDelete);
                conn.Open();
                String query = "DELETE FROM music WHERE name = @name AND username = @username";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.Add("@name", MySqlDbType.VarString).Value = name;
                command.Parameters.Add("@username", MySqlDbType.VarString).Value = "test";
                command.ExecuteNonQuery();
                conn.Close();
            }
            else
            {
                string title = "Error";
                string message = "You cannot delete the default radio stream.";
                MessageBox.Show(message, title);
            }
        }
        private void githubBtn_Click(Object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/priyanshu-7");
        }

    }
}
