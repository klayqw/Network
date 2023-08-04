using Models;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
namespace ClientApp;
public partial class RegisterMenu : Window
{
    public RegisterMenu()
    {
        InitializeComponent();
    }

    private void RegisterButtonClick(object sender, RoutedEventArgs e)
    {
        Task.Run(async () =>
        {
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

            if (string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                MessageBox.Show("field can not be empty");
            }
            else
            {
                new MainMenu().Show();
                this.Close();
            }
        });
    }
}