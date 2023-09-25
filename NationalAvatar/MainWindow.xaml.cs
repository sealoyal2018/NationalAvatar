using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using HandyControl.Controls;

using Microsoft.Win32;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace NationalAvatar;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : HandyControl.Controls.Window
{
    private readonly List<AvatarTemplate> templates;
    private string avatarPath = string.Empty;
    private AvatarTemplate currentTemplate;
    public List<AvatarTemplate> Templates => templates;

    public MainWindow()
    {
        templates = new List<AvatarTemplate>
        {
            new AvatarTemplate("pack://application:,,,/Resources/gq0.png"),
            new AvatarTemplate("pack://application:,,,/Resources/gq1.png"),
            new AvatarTemplate("pack://application:,,,/Resources/gq2.png"),
            new AvatarTemplate("pack://application:,,,/Resources/gq20.png"),
            new AvatarTemplate("pack://application:,,,/Resources/gq3.png"),
            new AvatarTemplate("pack://application:,,,/Resources/gq30.png"),
            new AvatarTemplate("pack://application:,,,/Resources/gq4.png"),
            new AvatarTemplate("pack://application:,,,/Resources/gq5.png"),
        };
        currentTemplate = templates[0];
        InitializeComponent();
        DataContext = this;
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "图片文件(*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";
        dialog.Multiselect = false;
        if (dialog.ShowDialog()??false)
        {
            avatarPath = dialog.FileName;
            await Refresh();
            downloadBtn.IsEnabled = true;
        }
    }

    private async void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var item = e.AddedItems[0] as AvatarTemplate;
        if (item is null)
            return;
        currentTemplate = item;
        await Refresh();
    }


    private async Task Refresh()
    {
        if (string.IsNullOrWhiteSpace(avatarPath))
            return;
        using var memory = await Live();
        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = memory;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        LiveImage.Source = image;
    }


    private async Task<MemoryStream> Live()
    {
        Uri uri = new Uri(currentTemplate.Path);
        var memory = new MemoryStream();
        var sourceImage = await Image.LoadAsync(avatarPath);
        var s = Application.GetResourceStream(uri);
        var temp = await Image.LoadAsync(s.Stream);
        temp.Mutate(ctx=> ctx.Resize(sourceImage.Size));
        sourceImage.Mutate(async ctx =>
        {
            ctx.DrawImage(temp, 1f);
        });
        await sourceImage.SaveAsync(memory, new PngEncoder());
        return memory;
    }

    private async void LiveImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        await SaveAvatar();
    }

    private async void Button_Click_1(object sender, RoutedEventArgs e)
    {
        await SaveAvatar();
    }

    private async Task SaveAvatar()
    {
        if (string.IsNullOrWhiteSpace(avatarPath))
        {
            Growl.Error("请先上传图像");
            return;
        }

        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Filter = "图片文件(*.png)|*.png";
        var r = dialog.ShowDialog() ?? false;
        if (!r)
            return;
        var fileName = dialog.FileName;

        Uri uri = new Uri(currentTemplate.Path);
        var sourceImage = await Image.LoadAsync(avatarPath);
        var s = Application.GetResourceStream(uri);
        var temp = await Image.LoadAsync(s.Stream);
        temp.Mutate(ctx => ctx.Resize(sourceImage.Size));
        sourceImage.Mutate(async ctx => ctx.DrawImage(temp, 1f));
        await sourceImage.SaveAsPngAsync(fileName);
    }

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start("github.com/");
    }
}
