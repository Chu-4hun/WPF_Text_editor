using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;



namespace WPF_Text_editor
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source); 
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        }
        public String path = "";
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e) // открыть файл
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                path = dlg.FileName;
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
            }
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e) 
        {
            try
            {
                object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
                btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold)); // жирный шрифт
                temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
                btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));  // курсив
                temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline)); // подчёркивание
                temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty); // выбор шрифта
                cmbFontFamily.SelectedItem = temp;
                temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty); // размер шрифта
                cmbFontSize.Text = temp.ToString();
            }
            catch (System.Exception) { } 
        }

        private void SaveEX(object sender, RoutedEventArgs routedEventArgs)
        {
             TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            try
            {
                if (path != "") 
                {
                    FileStream fileStream = new FileStream(path, FileMode.Create);
                    range.Save(fileStream, DataFormats.Rtf);
                    return;
                }
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                if (dlg.ShowDialog() == true) 
                {
                    FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                    range.Save(fileStream, DataFormats.Rtf);
                }
            }
            catch (System.Exception) { }
            
            Process.GetCurrentProcess().Kill();
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e) //Сохраниене файла
        {
            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            try
            {
                if (path != "") 
                {
                    FileStream fileStream = new FileStream(path, FileMode.Create);
                    range.Save(fileStream, DataFormats.Rtf);
                    return;
                }
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                if (dlg.ShowDialog() == true) 
                {
                    FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                    range.Save(fileStream, DataFormats.Rtf);
                }
            }
            catch (System.Exception) { }
        }
        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
            }
            catch (System.Exception) { }
        }
        private void ClrPcker_Background_SelectedColorChanged(object sender, EventArgs e) 
        {
            var Text_Color = ClrPcker_Background.SelectedColor.ToString();
            Char[] ColorChar = { '{', '}' };
            string Colorval = Text_Color.TrimEnd(ColorChar); 
            rtbEditor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, value: Colorval); 
        }

        private void Leftalig_Click(object sender, RoutedEventArgs e) => EditingCommands.AlignLeft.Execute(null, rtbEditor); // выравнимание по левому краю
        private void Rightalig_Click(object sender, RoutedEventArgs e) => EditingCommands.AlignRight.Execute(null, rtbEditor); // по правому краю
        private void Centalig_Click(object sender, RoutedEventArgs e) => EditingCommands.AlignCenter.Execute(null, rtbEditor); // по центру
        private void Justtalig_Click(object sender, RoutedEventArgs e) => EditingCommands.AlignJustify.Execute(null, rtbEditor); // выровнять по ширине
        private void Print_Click(object sender, RoutedEventArgs e) // предпросмотр печати
        {
            PrintDialog pd = new PrintDialog();
            if ((pd.ShowDialog() == true))
            {
                pd.PrintDocument((((IDocumentPaginatorSource)rtbEditor.Document).DocumentPaginator), "printing as paginator");
            }
        }
    }
}