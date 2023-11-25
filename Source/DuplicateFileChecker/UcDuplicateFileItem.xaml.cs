using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace DuplicateFileChecker
{
    /// <summary>
    /// Interaction logic for UcDuplicateFileItem.xaml
    /// </summary>
    public partial class UcDuplicateFileItem
    {
        public DuplicateDosya Dosya { get; set; }
        private Dosya _seciliDosya;
        public UcDuplicateFileItem(DuplicateDosya dosya)
        {
            Dosya = dosya;
            InitializeComponent();
        }
        private void KonumAcClick(object sender, RoutedEventArgs e)
        {
            var link = e.OriginalSource as Hyperlink;
            if (link != null)
            {
                _seciliDosya = ((Dosya) link.DataContext);
                if (File.Exists(_seciliDosya.Konum))
                {
                    Process.Start("explorer.exe", "/select, " + _seciliDosya.Konum);
                }
            }
        } 
    }
}
