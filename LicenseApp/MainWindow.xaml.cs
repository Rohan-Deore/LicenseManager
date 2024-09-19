using System.Windows;
using DatabaseManager;
using NLog;

namespace LicenseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LicenseDB? licenseDB = null;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Create button clicked.");
            licenseDB?.CreateTables();
        }

        private void ConnectDbBtn_Click(object sender, RoutedEventArgs e)
        {
            if (licenseDB == null)
            {
                logger.Info("Connected to database.");
                licenseDB = new LicenseDB();
            }
        }

        private void AddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            string userName = System.Environment.UserName;
            if (licenseDB == null)
            {
                logger.Error("Database instance not created.");
                return;
            }

            if (string.IsNullOrEmpty(UserNameTB.Text) || 
                string.IsNullOrEmpty(ApplicationNameTB.Text) || 
                StartDateCtl.SelectedDate == null ||
                EndDateCtl.SelectedDate == null)
            {
                logger.Error("User name or application name is not entered");
                MessageBox.Show("Fill all required content.");

                return;
            }

            var status = licenseDB.AddUser(UserNameTB.Text, CompanyNameTB.Text, ApplicationNameTB.Text, IsLicensed.IsChecked.Value,
                DateOnly.FromDateTime(StartDateCtl.SelectedDate.Value), DateOnly.FromDateTime(EndDateCtl.SelectedDate.Value));

            if (!status)
            {
                MessageBox.Show("Entry already exists.");
            }
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            string userName = System.Environment.UserName;
            if (licenseDB == null)
            {
                logger.Error("Database instance not created.");
                return;
            }

            if (string.IsNullOrEmpty(UserNameTB.Text) ||
                string.IsNullOrEmpty(ApplicationNameTB.Text) ||
                string.IsNullOrEmpty(CompanyNameTB.Text))
            {
                logger.Error("User name or application name is not entered");
                MessageBox.Show("Fill all required content.");

                return;
            }

            var errorMsg = string.Empty;
            var status = licenseDB.CheckLicense(UserNameTB.Text, CompanyNameTB.Text, 
                ApplicationNameTB.Text, userName, ref errorMsg);

            if (!status)
            {
                MessageBox.Show(errorMsg);
            }
        }
    }
}