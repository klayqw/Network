using Models;
using System.Net.Http.Json;
using System.Net.Http;
using System.Security.RightsManagement;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.IO;
using System;

namespace ClientApp;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void EnterButtonClick(object sender, RoutedEventArgs e)
    {
        Task.Run(async () =>
        {
            HttpClient httpClient = new HttpClient();

            User newUser = new User()
            {
                login = LoginTextBox.Text,
                password = PasswordTextBox.Text,
                email = EmailTextBox.Text,
            };

            HttpContent jsonContent = JsonContent.Create(newUser);
            HttpResponseMessage response = await httpClient.PostAsync("http://localhost/users/create", jsonContent);

            using var reader = new StreamReader(response.Content.ReadAsStream());
            var responseTxt = await reader.ReadToEndAsync();

            Console.WriteLine(response.StatusCode);
            Console.WriteLine(responseTxt);
        });
    }
}