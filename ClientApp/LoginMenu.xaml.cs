using Models;
using System.Net.Http.Json;
using System.Net.Http;
using System.Security.RightsManagement;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.IO;
using System;
using System.Text.Json;

namespace ClientApp;
public partial class LoginMenu : Window
{
    public LoginMenu()
    {
        InitializeComponent();
    }

    private async void LoginButtonClick(object sender, RoutedEventArgs e)
    {
        if(string.IsNullOrWhiteSpace(LoginTextBox.Text) || string.IsNullOrWhiteSpace(PasswordTextBox.Text))
        {
            MessageBox.Show("Fields can not be empty");
            return;
        }

        HttpClient httpClient = new HttpClient();

        User newUser = new User()
        {
            login = LoginTextBox.Text,
            password = PasswordTextBox.Text,
        };

        HttpContent jsonContent = JsonContent.Create(newUser);
        HttpResponseMessage response = await httpClient.PostAsync("http://localhost/users/login", jsonContent);

        using var reader = new StreamReader(response.Content.ReadAsStream());
        var responseTxt = await reader.ReadToEndAsync();

        Console.WriteLine(responseTxt);
        if(response.StatusCode == System.Net.HttpStatusCode.Found)
        {
            newUser = JsonSerializer.Deserialize<User>(responseTxt); 
        }


        else if(response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            MessageBox.Show(responseTxt);
            return;
        }

        new MainMenu(newUser).Show();
        this.Close();
    }

    private void GoToRegisterClick(object sender, RoutedEventArgs e)
    {
        new RegisterMenu().Show();
        this.Close();
    }
}