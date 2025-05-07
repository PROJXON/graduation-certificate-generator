using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using GraduationCertificateGenerator.Services;
using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Fonts;

namespace GraduationCertificateGenerator;

public partial class MainWindow : Window
{
    public record CertificateData
    (
        string Participant,
        DateTime StartDate,
        DateTime EndDate,
        string Role
    );

    private string participant = "";
    private string role = "";
    private string email = "";
    private DateTime startDate;
    private DateTime endDate;
    private bool isEmailChecked = false;
    private bool isSaveLocallyChecked = false;
    private string fileName = "";
    private string fullFilePath = "";
    private string folderPath = "";
    public static string EmailPassword = "";

    public MainWindow()
    {
        InitializeComponent();
        
        GlobalFontSettings.FontResolver = new CustomFontResolver();
    }

    private async void FileDestinationButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var options = new FolderPickerOpenOptions
            {
                Title = "Choose Certificate Folder Destination",
            };

            // Opens a dialog and allows the user to select a destination folder
            var result = await StorageProvider.OpenFolderPickerAsync(options);

            if (result.Count > 0)
            {
                // sets the selected folder as the path variable
                folderPath = result[0].Path.LocalPath;
                filePathMessage.Text = folderPath;
            }
        } catch (Exception err) {
            Console.WriteLine(err);
        }
    }
    
    private void SaveLocallyCheckbox_OnChecked(object? sender, RoutedEventArgs e)
    {
        pdfDestinationLabel.IsVisible = true;
        pdfDestinationButton.IsVisible = true;
        filePathDisplayLabel.IsVisible = true;
        filePathMessage.IsVisible = true;
        Height += 75;
    }

    private void SaveLocallyCheckbox_OnUnChecked(object? sender, RoutedEventArgs e)
    {
        pdfDestinationLabel.IsVisible = false;
        pdfDestinationButton.IsVisible = false;
        filePathDisplayLabel.IsVisible = false;
        filePathMessage.IsVisible = false;
        Height -= 75;
    }
    private void SendEmailCheckbox_OnChecked(object? sender, RoutedEventArgs e)
    {
        studentEmail.IsVisible = true;
        studentEmailLabel.IsVisible = true;
        Height += 40;
    }
    private void SendEmailCheckbox_OnUnChecked(object? sender, RoutedEventArgs e)
    {
        studentEmail.IsVisible = false;
        studentEmailLabel.IsVisible = false;
        Height -= 40;
    }
    
    private async void ReEnterPassword_OnClick(object? sender, RoutedEventArgs e)
    {
        await OpenPasswordPrompt();
    }

    private async void GeneratePdfButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            message.Text = "Generating PDF...";
            GatherInput();

            if (!ValidateInput())
            {
                message.Classes.Set("invalid", true);
                return;
            }

            CertificateData data = new(participant, startDate, endDate, role);
            PdfDocument pdf = CertificateService.CreatePdf(data);
            fullFilePath = Path.Combine(folderPath, fileName);
            pdf.Save(fullFilePath);
            
            if (isEmailChecked)
            {
                if (string.IsNullOrWhiteSpace(EmailPassword))
                {
                    await OpenPasswordPrompt();
                }
                
                using var ms = new MemoryStream();
                pdf.Save(ms, false);
                var pdfBytes = ms.ToArray();
                await EmailService.SendEmailViaLambdaAsync(email, participant, pdfBytes, fileName, EmailPassword);
                message.Classes.Set("success", true);
                message.Text = "Email sent successfully!";
            }

            if (!isSaveLocallyChecked)
            {
                File.Delete(fullFilePath);
            }
            else
            {
                message.Classes.Set("success", true);
                message.Text = $"File has been saved to {folderPath}";
            }
        }
        catch (Exception ex)
        {
            File.AppendAllText("error.log", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}");
            message.Text = "An error occurred. Please check the logs.";
        }
    }

    private void GatherInput()
    {
        participant = participantName.Text ?? string.Empty;
        startDate = onboardingDate.SelectedDate?.DateTime ?? DateTime.Today;
        endDate = completionDate.SelectedDate?.DateTime ?? DateTime.Today;
        fileName = $"{participant.Replace(" ", "_").ToLower()}_graduation_certificate.pdf";
        role = participantRole.Text ?? string.Empty;
        isEmailChecked = sendEmail.IsChecked ?? false;
        isSaveLocallyChecked = saveLocally.IsChecked ?? false;
        email = studentEmail.Text ?? string.Empty;
    }

    private bool ValidateInput()
    {
        MarkInvalidControls();

        if (string.IsNullOrWhiteSpace(participant) || !completionDate.SelectedDate.HasValue || !onboardingDate.SelectedDate.HasValue)
        {
            message.Text = "Please ensure that all required fields are filled out.";
            return false;
        }
        
        if (isEmailChecked && !EmailService.Validate(email))
        {
            message.Text = "Missing or invalid email address. Please provide a valid email.";
            return false;
        }
        
        if (!isEmailChecked && !isSaveLocallyChecked)
        {
            message.Text = "Please select whether you would like to save the pdf, email it, or both.";
            return false;
        }

        if (isSaveLocallyChecked && string.IsNullOrWhiteSpace(folderPath))
        {
            message.Text = "Please select a folder to save the file in.";
            return false;
        }

        return true;
    }

    private void MarkInvalidControls()
    {
        participantName.Classes.Set("invalid", string.IsNullOrWhiteSpace(participant));
        onboardingDate.Classes.Set("invalid", !onboardingDate.SelectedDate.HasValue);
        completionDate.Classes.Set("invalid", !completionDate.SelectedDate.HasValue);
        pdfDestinationButton.Classes.Set("invalid", string.IsNullOrWhiteSpace(folderPath));
        studentEmail.Classes.Set("invalid", !EmailService.Validate(email) && isEmailChecked);
    }
    
    private async Task OpenPasswordPrompt()
    {
        var passwordPrompt = new PasswordPrompt();
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            await passwordPrompt.ShowDialog(desktop.MainWindow);
        }
    }
}
