using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        private User user;

        public MainWindow()
        {
            InitializeComponent();
            user = new User();
            LoadPasswords();
        }

        private void SavePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = LabelTextBox.Text.Trim();
                string password = PasswordTextBox.Text;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ResultTextBlock.Text = "Please enter both Email/User and password.";
                    return;
                }

                user.SavePassword(email, password);
                ResultTextBlock.Text = "Password saved successfully!";
                PasswordTextBox.Clear();
                LabelTextBox.Clear();
                LoadPasswords();
            }
            catch (ArgumentException ex)
            {
                ResultTextBlock.Text = $"Input error: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                ResultTextBlock.Text = $"Operation error: {ex.Message}";
            }
            catch (IOException ex)
            {
                ResultTextBlock.Text = $"File error: {ex.Message}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Unexpected error: {ex.Message}";
            }
        }

        private void ShowAllPasswordsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPasswords();
        }

        private void LoadPasswords()
        {
            try
            {
                var passwords = user.GetPasswords();
                PasswordsListBox.Items.Clear();

                foreach (var kvp in passwords)
                {
                    var item = new ListBoxItem
                    {
                        Content = $"Email/User: {kvp.Key}, Password: {kvp.Value}",
                        Background = Brushes.Black,
                        Foreground = Brushes.White
                    };

                    var removeButton = new Button
                    {
                        Content = "Remove",
                        Width = 60,
                        Margin = new Thickness(5),
                        Background = Brushes.Red,
                        Foreground = Brushes.White
                    };

                    removeButton.Click += (s, args) => RemovePassword(kvp.Key);

                    var stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal
                    };
                    stackPanel.Children.Add(item);
                    stackPanel.Children.Add(removeButton);

                    PasswordsListBox.Items.Add(stackPanel);
                }

                if (passwords.Count == 0)
                {
                    PasswordsListBox.Items.Add(new ListBoxItem { Content = "No passwords found.", Background = Brushes.Black, Foreground = Brushes.White });
                }
            }
            catch (IOException ex)
            {
                ResultTextBlock.Text = $"File error: {ex.Message}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Unexpected error: {ex.Message}";
            }
        }

        private void RemovePassword(string email)
        {
            try
            {
                user.RemovePassword(email);
                ResultTextBlock.Text = $"Password for {email} removed successfully!";
                LoadPasswords();
            }
            catch (ArgumentException ex)
            {
                ResultTextBlock.Text = $"Input error: {ex.Message}";
            }
            catch (IOException ex)
            {
                ResultTextBlock.Text = $"File error: {ex.Message}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Unexpected error: {ex.Message}";
            }
        }

        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Text == "Enter password")
            {
                PasswordTextBox.Text = "";
                PasswordTextBox.Foreground = Brushes.White;
            }
        }

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                PasswordTextBox.Text = "Enter password";
                PasswordTextBox.Foreground = Brushes.Gray;
            }
        }

        private void LabelTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (LabelTextBox.Text == "Enter Email/User")
            {
                LabelTextBox.Text = "";
                LabelTextBox.Foreground = Brushes.White;
            }
        }

        private void LabelTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LabelTextBox.Text))
            {
                LabelTextBox.Text = "Enter Email/User";
                LabelTextBox.Foreground = Brushes.Gray;
            }
        }

        private void ThanksButton_Click(object sender, RoutedEventArgs e)
        {
            ThanksWindow thanksWindow = new ThanksWindow();
            thanksWindow.ShowDialog();
        }
    }
}
