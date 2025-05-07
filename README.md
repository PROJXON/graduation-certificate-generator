# Graduation Certificate Generator

The purpose of this application is to generate certificates for the successful completion of the PROJXON Momentum Internship Program. It works by taking user input for the participant name and beginning/completion dates, then overlays this information over an image template and generates a pdf for the user. This should hopefully reduce time needed to create certificates.

Generated PDFs can be saved on the user's computer, or automatically sent to the recipient if an email address is supplied.

## How to Use

1. Download the latest [release](https://github.com/PROJXON/graduation-certificate-generator/releases) for your operating system.
2. Extract the zip file to an accessible location on your computer.
    - On Windows, right click the zip folder, select **Extract All**... and select where you would like the extracted files to go.
3. Open the folder where you extracted the files and double click the executable name CanvasCertificateGenerator.
    - You will most likely see a popup confirming that you want to run the executable if you are using Windows. If this happens, click "More info" and then select "Run Anyway".
4. Fill out the input and click "Generate". Then, locate the generated PDF(s) in the destination folder you selected.
    - If sending an email is selected, a password prompt will appear.

## Having Issues?

If you experience any issues, please report them with the application feedback form located on the **IT Team -> Applications** dashboard page.

## Technologies Used

Written with C#/.NET, the GUI for this application was created using Avalonia to allow for its usage on any operating system.