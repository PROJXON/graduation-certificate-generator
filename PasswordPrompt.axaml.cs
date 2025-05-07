using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GraduationCertificateGenerator;

public partial class PasswordPrompt : Window
{
    public PasswordPrompt()
    {
        InitializeComponent();
    }

    private void OnSubmitClick(object? sender, RoutedEventArgs e)
    {
        MainWindow.EmailPassword = passwordBox.Text;
        Close();
    }
}