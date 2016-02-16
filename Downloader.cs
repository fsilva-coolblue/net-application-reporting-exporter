using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using net_coolblue_datastore_clients.RavenDb;

namespace ApplicationReportingDataDownloader
{
    public partial class Downloader : Form
    {
        private const string DEFAULT_FILENAME = "reporting_events.xlsx";
        private const string ACCEPTANCE_ENVIRONMENT = "Acceptance";
        private const string PRODUCTION_ENVIRONMENT = "Production";

        private ReportingEventsExporter _acceptanceExporter;
        private ReportingEventsExporter _productionExporter;

        public Downloader()
        {
            InitializeComponent();
        }

        private void Downloader_Load(object sender, EventArgs e)
        {
            comboEnvironment.DataSource = new List<string>
                                     {
                                         ACCEPTANCE_ENVIRONMENT,
                                         PRODUCTION_ENVIRONMENT
                                     };

            _acceptanceExporter =
                new ReportingEventsExporter(
                    new RavenRepository<ReportingEventDataEntity, int>("RavenDbConnectionString_Acceptance"),
                    new EventExcelExporter());

            _productionExporter =
                new ReportingEventsExporter(
                    new RavenRepository<ReportingEventDataEntity, int>("RavenDbConnectionString_Production"),
                    new EventExcelExporter());

            comboEnvironment.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fileName = txtFileName.Text;

            if(string.IsNullOrWhiteSpace(fileName))
                fileName = DEFAULT_FILENAME;
            else if(Directory.Exists(fileName))
                fileName = Path.Combine(fileName, DEFAULT_FILENAME);

            var fromDate = fromDatePicker.Value.Date;
            var toDate = fromDatePicker.Value.Date.AddDays(1);

            ReportingEventsExporter exporter;

            if(((string)comboEnvironment.SelectedItem) == PRODUCTION_ENVIRONMENT)
                exporter = _productionExporter;
            else
                exporter = _acceptanceExporter;

            ExportResult result = exporter.Export(fromDate, toDate, fileName);

            if(result == ExportResult.Ok)
                MessageBox.Show("Downloaded!");
            else if(result == ExportResult.NoEvents)
                MessageBox.Show("No events found.");
            else
                MessageBox.Show("Couldn't download due to an error. Check the log file.");
        }

        private void btnPickFile_Click(object sender, EventArgs e)
        {
            var saveResult = saveFileDialog.ShowDialog();

            if(saveResult == DialogResult.OK)
                txtFileName.Text = saveFileDialog.FileName;
        }
    }
}