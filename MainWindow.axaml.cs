namespace CanvasCertificateGenerator;
using CanvasCertificateGenerator.Services;

using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Fonts;

public partial class MainWindow : Window
{
    public record CertificateData
    (
        string Participant,
        string Course,
        DateTime Date,
        string Role
    );

    private string participant = "";
    private string course = "";
    private string role = "";
    private DateTime date;
    private string fileName = "";
    private string fullFilePath = "";
    private string folderPath = "";

    public MainWindow()
    {
        InitializeComponent();

        // Register the custom font resolver
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

            if (result != null && result.Count > 0)
            {
                // sets the selected folder as the path variable
                folderPath = result[0].Path.LocalPath;
                filePathMessage.Text = folderPath;
            }
        } catch (Exception err) {
            Console.WriteLine(err);
        }
    }

    private void GeneratePdfButton_OnClick(object? sender, RoutedEventArgs e)
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

            CertificateData data = new(participant, course, date, role);
            PdfDocument pdf = CertificateService.CreatePdf(data);
            fullFilePath = Path.Combine(folderPath, fileName);
            pdf.Save(fullFilePath);

            message.Classes.Set("success", true);
            message.Text = $"File has been saved to {folderPath}";
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
        course = courseName.Text ?? string.Empty;
        date = completionDate.SelectedDate?.DateTime ?? DateTime.Today;
        fileName = $"{participant.Replace(" ", "_").ToLower()}_{course.Replace(" ", "_").ToLower()}_certificate.pdf";
        role = participantRole.Text ?? string.Empty;
    }

    private bool ValidateInput()
    {
        MarkInvalidControls();

        if (string.IsNullOrWhiteSpace(participant) || string.IsNullOrWhiteSpace(course) || !completionDate.SelectedDate.HasValue)
        {
            message.Text = "Please ensure that all required fields are filled out.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(folderPath))
        {
            message.Text = "Please select a folder to save the file in.";
            return false;
        }

        return true;
    }

    private void MarkInvalidControls()
    {
        participantName.Classes.Set("invalid", string.IsNullOrWhiteSpace(participant));
        courseName.Classes.Set("invalid", string.IsNullOrWhiteSpace(course));
        completionDate.Classes.Set("invalid", !completionDate.SelectedDate.HasValue);
        pdfDestinationButton.Classes.Set("invalid", string.IsNullOrWhiteSpace(folderPath));
    }
}
