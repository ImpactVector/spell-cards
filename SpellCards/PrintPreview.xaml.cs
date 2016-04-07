﻿using System;
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
using System.Windows.Shapes;

namespace SpellCards
{
    /// <summary>
    /// Interaction logic for PowersPrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : Window
    {
        private List<Business.Spell> _spells;
        public PrintPreview(List<Business.Spell> spells)
        {
            try
            {
                InitializeComponent();
                _spells = spells;
                _reportViewer.Load += ReportViewer_Load;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.PowerDataListBindingSource.DataSource = _powers;
        //    this.reportViewer1.RefreshReport();
        //}

        private bool _isReportViewerLoaded;

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            try
            {
                if (!_isReportViewerLoaded)
                {
                    Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();

                    reportDataSource1.Name = "DataSet1"; //Name of the report dataset in our .RDLC file
                    reportDataSource1.Value = _spells;
                    this._reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                    this._reportViewer.LocalReport.ReportEmbeddedResource = "SpellCards.Reports.Spells.rdlc";

                    this._reportViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    _reportViewer.RefreshReport();

                    _isReportViewerLoaded = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
            }
        }
    }
}
