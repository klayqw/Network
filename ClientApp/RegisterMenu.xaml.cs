using Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ClientApp;
public partial class RegisterMenu : Window
{
    public RegisterMenu()
    {
        InitializeComponent();
    }

    private async void RegisterButtonClick(object sender, RoutedEventArgs e)
    {
        if(string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
            string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
            string.IsNullOrWhiteSpace(PasswordTextBox.Text))
        {
            MessageBox.Show("Fields can not be empty");
            return;
        }

        HttpClient httpClient = new HttpClient();

        User newUser = new User()
        {
            login = LoginTextBox.Text,
            password = PasswordTextBox.Text,
            email = EmailTextBox.Text
        };

        HttpContent jsonContent = JsonContent.Create(newUser);
        HttpResponseMessage response = await httpClient.PostAsync("http://localhost/users/create", jsonContent);

        using var reader = new StreamReader(response.Content.ReadAsStream());
        var responseTxt = await reader.ReadToEndAsync();

        System.Console.WriteLine(response.StatusCode);
        System.Console.WriteLine(responseTxt);

        if(response.StatusCode == System.Net.HttpStatusCode.Accepted)
            MessageBox.Show("User Created Successfully");

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            MessageBox.Show(responseTxt);
            return;
        }

        new MainMenu(newUser).Show();
        this.Close();
    }
}