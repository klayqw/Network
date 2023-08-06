using Models;
using System.IO;
using System.Net.Http.Json;
using System.Net.Http;
using System.Windows;
using System;
using System.Text.Json;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Controls;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.Windows.Markup;
using System.Text;

namespace ClientApp;
public partial class MainMenu : Window
{
    User nowUser = new User();
    public MainMenu(User user)
    {
        InitializeComponent();

        nowUser = user;
    }
    private void SetAllVisibility()
    {
        UsersListView.Visibility = Visibility.Hidden;
        AccountGrid.Visibility = Visibility.Hidden;
    }
    private async void ShowUsersClick(object sender, RoutedEventArgs e)
    {
        SetAllVisibility();
        UsersListView.Visibility = Visibility.Visible;

        HttpClient httpClient = new HttpClient();

        HttpResponseMessage response = await httpClient.GetAsync("http://localhost/users/getall");

        string responseBodyGet = await response.Content.ReadAsStringAsync();

        IEnumerable<User> users = JsonSerializer.Deserialize<IEnumerable<User>>(responseBodyGet);

        if (users != null)
        {
            UsersListView.ItemsSource = users;
        }
    }
    private void AccountClick(object sender, RoutedEventArgs e)
    {
        SetAllVisibility();
        AccountGrid.Visibility = Visibility.Visible;
        EmailTextBox.Text = nowUser.email;
        PasswordTextBox.Text = nowUser.password;
        LoginTextBox.Text = nowUser.login;
    }
    private void DeleteAccountClick(object sender, RoutedEventArgs e)
    {
        SetAllVisibility();
        DeleteGrid.Visibility = Visibility.Visible;
    }
    private void SearchClick(object sender, RoutedEventArgs e)
    {

    }



    private async void SaveChangesClick(object sender, RoutedEventArgs e)
    {
        HttpClient httpClient = new HttpClient();

        User user = new User()
        {
            email = EmailTextBox.Text,
            login = LoginTextBox.Text,
            password = PasswordTextBox.Text
        };
    }

    private async void DeleteClick(object sender, RoutedEventArgs e)
    {
        HttpClient httpClient = new HttpClient();
        
        HttpResponseMessage response = await httpClient.DeleteAsync($"http://localhost/users/delete/{IdTextBox.Text}");
    }
}