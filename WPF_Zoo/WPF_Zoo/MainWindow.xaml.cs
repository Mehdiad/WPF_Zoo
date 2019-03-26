using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace WPF_Zoo
{
    /// <summary>
    /// A Zoo program that keeps track of different zoos and different animals inside and outside of the zoo.
    /// Followed the guide of a udemy course called Complete C# Master
    /// 
    /// Contains alot of re-used code, could be "enhanced" by lowering the number of lines
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection objConn;

        public MainWindow()
        {
            InitializeComponent();
            //The first step to get data from the database to the DataSet is to establish a database connection, 
            //which requires a System.Data.SqlClient.SqlCommand object and a connection string.
            //    The connection string in the code to follow connects a SQL Server server that is located 
            //    on the local computer(the computer where the code is running).
            //    After the SqlConnection object is created, 
            //    call the Open method of that object to establish the actual database link.

            string connectionString = "Data Source=Name;Initial Catalog=master;User ID=sa;Password=*********";
            //or
            //string connectionString = ConfigurationManager.ConnectionStrings["WPF_Zoo.Properties.Settings.DataSet1"].ConnectionString;
            //top line doesnt not work for some odd reason

            objConn = new SqlConnection(connectionString);

            //Create a DataAdapter object, which represents the link between the database and your DataSet object.
            //    You can specify SQL or another type of command that is used to retrieve data as part of the 
            //    constructor object of the DataAdapter. 
            //    This sample uses a SQL statement that retrieves records from the Authors table in the Pubs database.
            //    SqlDataAdapter daZoo = new SqlDataAdapter(query, objConn);
            ShowZoo();
            ShowAnimal();
        }

        private void ShowZoo()
        {
            try
            {
                string query = "select * from Zoo";
                SqlDataAdapter daZoo = new SqlDataAdapter(query, objConn);

                using (daZoo)
                {
                    DataTable zooTable = new DataTable();
                    daZoo.Fill(zooTable);
                    listZoo.DisplayMemberPath = "Location";
                    listZoo.SelectedValuePath = "Id";
                    listZoo.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            

        }
        
        private void ShowAnimal()
        {
            try
            {
                string query = "select * from Animal";
                SqlDataAdapter daAnimal = new SqlDataAdapter(query, objConn);

                using (daAnimal)
                {
                    DataTable animalTable = new DataTable();
                    daAnimal.Fill(animalTable);
                    listAnimal.DisplayMemberPath = "Name";
                    listAnimal.SelectedValuePath = "Id";
                    listAnimal.ItemsSource = animalTable.DefaultView;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            
        }


        private void ShowAnimalsInZoo()
        {
            try
            {
                string query = "select * from Animal a inner join ZooAnimal za on a.Id = za.AnimalId where za.ZooId = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, objConn);
                SqlDataAdapter daAnimal = new SqlDataAdapter(sqlCommand);

                using (daAnimal)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoo.SelectedValue);

                    DataTable animalTable = new DataTable();
                    daAnimal.Fill(animalTable);
                    AnimalsInZoo.DisplayMemberPath = "Name";
                    AnimalsInZoo.SelectedValuePath = "Id";
                    AnimalsInZoo.ItemsSource = animalTable.DefaultView;
                }

            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        private void ListZoo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAnimalsInZoo();
            try
            {
                string query = "select Location from Zoo where Id = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, objConn);
                SqlDataAdapter daAnimal = new SqlDataAdapter(sqlCommand);

                using (daAnimal)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoo.SelectedValue);
                    DataTable animalTable = new DataTable();
                    daAnimal.Fill(animalTable);
                    AddContent.Text = animalTable.Rows[0]["Location"].ToString();
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(e.ToString());
            }

        }

        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "DELETE FROM Zoo where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, objConn);
                objConn.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoo.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                objConn.Close();
                ShowZoo();
            }
            
            
        }

        private void AddZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "INSERT INTO Zoo VALUES (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, objConn);
                objConn.Open();
                sqlCommand.Parameters.AddWithValue("@Location", AddContent.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                objConn.Close();
                ShowZoo();
            }
        }

        private void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "INSERT INTO ZooAnimal VALUES (@ZooId, @AnimalId)";
                SqlCommand sqlCommand = new SqlCommand(query, objConn);
                objConn.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoo.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimal.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                objConn.Close();
                ShowAnimalsInZoo();
            }
        }

        private void RemoveAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "DELETE FROM ZooAnimal WHERE AnimalId = @AnimalID";
                SqlCommand command = new SqlCommand(query,objConn);
                objConn.Open();
                command.Parameters.AddWithValue("@AnimalID",AnimalsInZoo.SelectedValue);
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                objConn.Close();
                ShowAnimalsInZoo();
            }

        }

        private void ExtinctAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "DELETE FROM Animal WHERE Id = @AnimalID";
                SqlCommand command = new SqlCommand(query, objConn);
                objConn.Open();
                command.Parameters.AddWithValue("@AnimalID", listAnimal.SelectedValue);
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                objConn.Close();
                ShowAnimal();
                ShowAnimalsInZoo();
            }
        }

        private void NewAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "INSERT INTO Animal Values (@AnimalName)";
                SqlCommand command = new SqlCommand(query, objConn);
                objConn.Open();
                command.Parameters.AddWithValue("@AnimalName", AddContent.Text);
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                objConn.Close();
                ShowAnimal();
            }
        }

        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "UPDATE Zoo SET Location = @ZooLocation WHERE Id = @ZooId";
                SqlCommand command = new SqlCommand(query, objConn);
                objConn.Open();
                command.Parameters.AddWithValue("@ZooLocation", AddContent.Text);
                command.Parameters.AddWithValue("@ZooId", listZoo.SelectedValue);
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                objConn.Close();
                ShowZoo();
            }

        }

        private void ListAnimal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string query = "select Name from Animal where Id = @AnimalId";
                SqlCommand command = new SqlCommand(query, objConn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using (adapter)
                {
                    command.Parameters.AddWithValue("@AnimalId", listAnimal.SelectedValue);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    AddContent.Text = table.Rows[0]["Name"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "UPDATE Animal SET Name = @AnimalName WHERE Id = @AnimalId";
                SqlCommand command = new SqlCommand(query, objConn);
                objConn.Open();
                command.Parameters.AddWithValue("@AnimalName", AddContent.Text);
                command.Parameters.AddWithValue("@AnimalId", listAnimal.SelectedValue);
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
            finally
            {
                objConn.Close();
                ShowAnimal();
            }
        }
    }
}
