using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;

namespace DuplicateFileChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public ObservableCollection<UcDuplicateFileItem> DuplicateItemList { get; set; }
        public List<DuplicateDosya> DuplicateDosyaList { get; set; }
        public List<Dosya> DosyaList { get; set; }



        private static string ComputeHash(string fileName)
        {
            var hashAlgorithm = new SHA1CryptoServiceProvider();
            FileStream stmcheck = File.OpenRead(fileName);
            try
            {
                byte[] hash = hashAlgorithm.ComputeHash(stmcheck);
                string computed = BitConverter.ToString(hash).Replace("-", "");
                return computed;
            }
            finally
            {
                stmcheck.Close();
            }
        }

        private void DosyaBul(string dizin)
        {
            foreach (var dosyaYol in Directory.GetFiles(dizin))
            {
                var dosyaAd = dosyaYol.Split('\\').Last();
                var dosya = new Dosya
                    {
                        Ad = dosyaAd,
                        DegistirmeTarih = File.GetLastWriteTime(dosyaYol),
                        HashKod = ComputeHash(dosyaYol),
                        Konum = dosyaYol
                    };
                DosyaList.Add(dosya);
            }
            foreach (var altDizin in Directory.GetDirectories(dizin))
            {
                DosyaBul(altDizin);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            
            DuplicateDosyaList = new List<DuplicateDosya>();
            DosyaList = new List<Dosya>();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
           var result = dialog.ShowDialog();
            if (result.Value)
            {
                //1-Her dosya bilgisini Dosya listesine ekle.
                Thread.Sleep(500);
                LblStatus.Dispatcher.Invoke(DispatcherPriority.Background, (Action)(() => LblStatus.Content = "1- Dosyalar bulunuyor..."));
                //LblStatus.Content = "Dosyalar bulunuyor...";
                DosyaList = new List<Dosya>();
                DosyaBul(dialog.SelectedPath);
                
                
                //2-Mukerrer dosyalari tespit et. hash kodlarini kullanarak DuplicateDosyaList olustur.
                Thread.Sleep(100);

                LblStatus.Dispatcher.Invoke(DispatcherPriority.Background, (Action)(() => LblStatus.Content = "2- Mükerrer dosyalar tespit ediliyor..."));
                //LblStatus.Content = "Mükerrer dosyalar tespit ediliyor...";

                var duplicateHashList = (from d in DosyaList
                                         group d by d.HashKod
                                         into grp
                                         where grp.Count() > 1
                                         select grp.Key).ToList();

                //3-Mukerrer hash kodlarından ayni dosyalari bul ve ilgili DuplicateDosyaList elemaninin DosyaList esini gucelle.

                Thread.Sleep(100);
                LblStatus.Dispatcher.Invoke(DispatcherPriority.Background, (Action)(() => LblStatus.Content = "3- Mükerrer dosyalar listesi oluşturuluyor..."));
                //LblStatus.Content = "Mükerrer dosyalar listesi oluşturuluyor...";


                DuplicateDosyaList= new List<DuplicateDosya>();
                foreach (var hash in duplicateHashList)
                {
                    DuplicateDosyaList.Add(new DuplicateDosya
                        {
                            HashKod = hash,
                            DosyaList = DosyaList.Where(s => s.HashKod == hash).ToList()
                        });
                }

                //4- Liste olustur.

                Thread.Sleep(100);
                LblStatus.Dispatcher.Invoke(DispatcherPriority.Background, (Action)(() => LblStatus.Content = "4- Mükerrer dosyalar listeleniyor..."));
                //LblStatus.Content = "Mükerrer dosyalar listeleniyor...";

                DuplicateItemList = new ObservableCollection<UcDuplicateFileItem>();
                
                foreach (var duplicateDosya in DuplicateDosyaList)
                {
                    DuplicateItemList.Add( new UcDuplicateFileItem(duplicateDosya)); 
                }
                
                LbDupFileList.ItemsSource = DuplicateItemList;

                LblStatus.Content = string.Format("\"{0}\" için işlem tamam.", dialog.SelectedPath);

            }
            
        }
    }
}
