using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace MySynch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Props

        string leftRootPath;
        string rightRootPath;
        public static readonly string RepositoryFileName = "_JSYNCH.txt";
        public static readonly string TrashFolderName = "_JSYNCH_DELETED_FILES_BACKUP"; 
        List<SOperation> Operations;

        #endregion

        #region CTOR

        public MainWindow()
        {
            InitializeComponent();
            var names = Enum.GetNames(typeof(OperationType));
            foreach (var name in names)
            {
                lst.Items.Add(new CheckBox() { Content = name , IsThreeState = false , IsChecked = true });
            }
            (lst.Items[0] as CheckBox).IsChecked = false;
        } 

        #endregion

        #region Methods

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            var oldCursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;
                var c = new SOperationCollection(leftRootPath, rightRootPath, Operations);
                c.Operate();

                this.Operations = new List<SOperation>(c.OperationList);
                grdMain.ItemsSource = this.Operations;
                btnSync.IsEnabled = false;
                MessageBox.Show("Finished");
            }
            finally
            {
                this.Cursor = oldCursor;
            }
        }

        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            var oldCursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;
                this.leftRootPath = txtLeftPath.Text;
                this.rightRootPath = txtRightPath.Text;
                this.Operations = SynchHelper.GetOprations(this.leftRootPath,this.rightRootPath);
                grdMain.ItemsSource = Operations;
                btnSync.IsEnabled = true;
            }
            finally
            {
                this.Cursor = oldCursor;
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (Operations != null)
            {
                var filter = new OperationsFiltering(lst.Items);
                grdMain.ItemsSource = filter.Filter(this.Operations).ToList();
            }
        }

        #endregion

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtLeftPath.Text = openDialog.SelectedPath;
            }
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtRightPath.Text = openDialog.SelectedPath;
            }
        }

    }
}
