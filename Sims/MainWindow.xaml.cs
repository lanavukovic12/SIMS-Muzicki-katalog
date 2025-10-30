using Sims;
using System.Windows;
using System.Windows.Controls;

namespace Sims
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => EmailBox.Focus();
            EmailBox.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter) PasswordBox.Focus();
            };
            PasswordBox.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter) LoginButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            };
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;
            string password = PasswordBox.Password;

            if (email == "admin@gmail.com" && password == "admin")
            {
                MessageBox.Show("Login as admin successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
                this.Close();
            }
            else if (email == "editor@gmail.com" && password == "editor")
            {
                MessageBox.Show("Login as editor successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                EditorWindow editorWindow = new EditorWindow(email);
                editorWindow.Show();
                this.Close();
            }
            else if (email == "editor2@gmail.com" && password == "editor2")
            {
                MessageBox.Show("Login as editor2 successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                EditorWindow editorWindow = new EditorWindow(email);
                editorWindow.Show();
                this.Close();
            }
            else if (email == "music.editor@gmail.com" && password == "music")
            {
                MessageBox.Show("Login as music.editor successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                EditorWindow editorWindow = new EditorWindow(email);
                editorWindow.Show();
                this.Close();
            }

            else if (email == "user@gmail.com" && password == "user")
            {
                MessageBox.Show("Login as user successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                UserWindow userWindow = new UserWindow(email);
                userWindow.Show();
                this.Close();
            }

            else if (email == "user2@gmail.com" && password == "user2")
            {
                MessageBox.Show("Login as user2 successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                UserWindow userWindow = new UserWindow(email);
                userWindow.Show();
                this.Close();
            }

            else if (email == "disco.user@gmail.com" && password == "disco")
            {
                MessageBox.Show("Login as disco.user successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                UserWindow userWindow = new UserWindow(email);
                userWindow.Show();
                this.Close();
            }

            else
            {
                MessageBox.Show("Invalid email or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}