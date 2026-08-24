using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using WinRT.Interop;
using SubConverter.Engine;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SubConverter_WindowsApp;

/// <summary>
/// The main content page displayed inside the application window.
/// Add your UI logic, event handlers, and data binding here.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();

        InputFormatBox.ItemsSource = Enum.GetValues<SupportedFormat>();
        OutputFormatBox.ItemsSource = Enum.GetValues<SupportedFormat>();

        InputFormatBox.SelectedItem = SupportedFormat.sub;
        OutputFormatBox.SelectedItem = SupportedFormat.srt;
    }

    private async void Execute_Click(object sender, RoutedEventArgs e)
    {
        float framerate = (float)FramerateBox.Value;
        if (!TryGetTimeSpan(out var offset))
            return;

        var inputFile = InputFileTextBox.Text;
        var inputFormat = (SupportedFormat)InputFormatBox.SelectedItem;
        var outputFormat = (SupportedFormat)OutputFormatBox.SelectedItem;
        var outputFormatName = Enum.GetName<SupportedFormat>(outputFormat);

        var picker = new FileSavePicker
        {
            SuggestedFileName = "output." + outputFormatName
        };
        picker.FileTypeChoices.Add(
            outputFormatName,
            [$".{outputFormatName}"]
        );
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);
        var outputFile = await picker.PickSaveFileAsync();
        if (outputFile == null)
            return; // User cancelled
        
        var inputFileInfo = new FileInfo(inputFile);
        var outputFileInfo = new FileInfo(outputFile.Path);

        AdditionalInfo info = new()
        {
            Framerate = framerate,
            Offset = offset
        };

        try
        {
            SubtitlesConverter.Convert(inputFileInfo, inputFormat, outputFileInfo, outputFormat, info);
        } catch(Exception ex)
        {
            ExecuteErrorText.Visibility = Visibility.Visible;
            ExecuteErrorText.Text = ex.Message;
        }

    }

    private async void BrowseFile_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();

        picker.FileTypeFilter.Add("*");

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();

        if (file != null)
        {
            SetInputFile(file.Path);
        }
    }

    private bool TryGetTimeSpan(out TimeSpan value)
    {
        if (string.IsNullOrWhiteSpace(OffsetTextBox.Text))
        {
            OffsetErrorText.Visibility = Visibility.Collapsed;
            value = TimeSpan.Zero;
            return true;
        }
        if (TimeSpan.TryParseExact(
            OffsetTextBox.Text,
            @"hh\:mm\:ss\.fff",
            CultureInfo.InvariantCulture,
            out value))
        {
            OffsetErrorText.Visibility = Visibility.Collapsed;
            return true;
        }

        OffsetErrorText.Visibility = Visibility.Visible;
        return false;
    }

    private void OffsetTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(OffsetTextBox.Text))
        {
            OffsetErrorText.Visibility = Visibility.Collapsed;
            return;
        }

        if (TimeSpan.TryParseExact(
            OffsetTextBox.Text,
            @"hh\:mm\:ss\.fff",
            CultureInfo.InvariantCulture,
            out _))
        {
            OffsetErrorText.Visibility = Visibility.Collapsed;
        }
        else
        {
            OffsetErrorText.Visibility = Visibility.Visible;
        }
    }

    private void InputFile_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
    }

    private async void InputFile_Drop(object sender, DragEventArgs e)
    {
        if (!e.DataView.Contains(StandardDataFormats.StorageItems))
            return;

        var items = await e.DataView.GetStorageItemsAsync();

        if (items.Count != 1)
            return;

        if (items[0] is not StorageFile file)
            return;

        SetInputFile(file.Path);
    }

    private void SetInputFile(string path)
    {
        InputFileTextBox.Text = path;

        try
        {
            var format = SubtitlesConverter.GetFormat(new FileInfo(path));
            InputFormatBox.SelectedItem = format;
        }
        catch
        {
            // Do nothing, if format is not supported, let the user decide
        }
    }
}
