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

namespace CookingBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        { // СТРОКА ПОДКЛЮЧЕНИЯ БД
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Recipes.mdf;Integrated Security=True";
            //string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Projects-C#\\CookingBook\\Recipes.mdf;Integrated Security=True";

            sqlConnection = new SqlConnection(connectionString);
            await sqlConnection.OpenAsync();

            SqlDataReader sqlReader = null;

            SqlCommand command = new SqlCommand("SELECT * FROM [Recipes]", sqlConnection);

            try
            {
                sqlReader = await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {
                    listRecipes.Items.Add(sqlReader["Name"]?.ToString());
                }
            }
            catch
            {

            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
            }

            listRecipes.SelectedItem = listRecipes.Items[0];
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sqlConnection.Close();
        }

        private async void listRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Recipes] WHERE Name = @Name", sqlConnection);
            command.Parameters.AddWithValue("Name", lb.SelectedItems[0]?.ToString());
            cooking.Inlines.Clear();

            try
            {
                sqlReader = await command.ExecuteReaderAsync();
                string ingredientsLong = "";
                while (await sqlReader.ReadAsync())
                {
                    picture.Source = new BitmapImage(new Uri(sqlReader["imgSource"].ToString()));
                    title.Text = sqlReader["Name"].ToString();
                    ingredientsLong = sqlReader["Ingredients"].ToString();
                    string cookingLong = sqlReader["Cooking"].ToString();
                    string[] cookingArr = cookingLong.Split(';');
                    foreach(string s in cookingArr)
                    {
                        string added = "\t" + s;
                        cooking.Inlines.Add(new Run(added));
                        cooking.Inlines.Add(new LineBreak());
                    }

                }
                string[] ingredientsArr = ingredientsLong.Split(';');
                ingredientsList.ListItems.Clear();
                foreach (string s in ingredientsArr)
                {
                    ingredientsList.ListItems.Add(new ListItem(new Paragraph(new Run(s))));

                }
                ingredientsList.ListItems.Remove(ingredientsList.ListItems.Last());

            }
            catch
            {

            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
            }

        }
    }
}
