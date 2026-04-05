using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MovieShop.ViewModels;
using System;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

namespace MovieShop.Views
{
    public sealed partial class SellPage : Page
    {
        public SellEquipmentViewModel ViewModel { get; set; } = new SellEquipmentViewModel();
        private string _selectedLocalPath = string.Empty;

        public SellPage()
        {
            this.InitializeComponent();
        }

        private async void ImageUploadPlaceholder_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var picker = new FileOpenPicker();

            if (App._window != null)
            {
                var hwnd = WindowNative.GetWindowHandle(App._window);
                InitializeWithWindow.Initialize(picker, hwnd);
            }

            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpeg");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                _selectedLocalPath = file.Path;
                FileNameLabel.Text = $"Selected: {file.Name}";
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var category = (CategoryInput.SelectedItem as ComboBoxItem)?.Content.ToString();
                var condition = (ConditionInput.SelectedItem as ComboBoxItem)?.Content.ToString();
                var imageUrl = !string.IsNullOrEmpty(_selectedLocalPath) ? _selectedLocalPath : ImageUrlInput.Text;

                ViewModel.SubmitListing(category, condition, imageUrl);

                if (this.Parent is ContentControl contentArea)
                    contentArea.Content = new StartPageEquipment();
            } 
            catch (ArgumentException ex)
            {
                var dlg = new ContentDialog
                {
                    Title = "Validation error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await dlg.ShowAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SellPage] SubmitListing failed: {ex}");
                var dlg = new ContentDialog
                {
                    Title = "Error",
                    Content = "Could not list the item. Please try again.",
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await dlg.ShowAsync();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent is ContentControl contentArea)
                contentArea.Content = new StartPageEquipment();
        }
    }
}