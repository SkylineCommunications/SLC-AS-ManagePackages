using ManageInstallPackages_1.View;

namespace ManageInstallPackages_1.PackageOverview
{
    using System.Collections.Generic;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Utils.Packages;

    internal class ManagePackagesView : Dialog
    {
        private readonly string[] headers = { "Name", "Version", "OS", "Arch" };

        public ManagePackagesView(IEngine engine) : base(engine)
        {
            Title = "Manage Packages";
            Width = 700;
            Height = 450;
            SetColumnWidth(0, 50);
            SetColumnWidth(1, 110);
            SetColumnWidth(2, 110);
            SetColumnWidth(3, 110);
            SetColumnWidth(4, 110);
            SetColumnWidth(5, 180);

            FinishButton = new Button("Finish");
            Packages = new TableSelection(engine, headers);
            DeleteButton = new Button("Delete");
            UploadNewButton = new Button("Upload New Package...") { Width = 180 };
        }

        public Button FinishButton { get; set; }

        public Button DeleteButton { get; set; }

        public Button UploadNewButton { get; set; }

        public TableSelection Packages { get; set; }

        public void Initialize(Dictionary<string, PackageInfo> packages)
        {
            Clear();
            if (Packages != null)
            {
                Packages.Clear();
            }

            FinishButton.IsEnabled = true;
            int row = 0;

            // First Column
            var tableRows = GetTableRowsFromPackagePaths(packages);
            Packages.Initialize(tableRows);
            AddSection(Packages, row, 0);
            row += Packages.RowCount;

            AddWidget(DeleteButton, row, 3);
            AddWidget(UploadNewButton, row++, 4, 1, 2, HorizontalAlignment.Right);
            AddWidget(new Label(string.Empty), row++, 0);
            AddWidget(FinishButton, row, 4, 1, 2, HorizontalAlignment.Right);
        }

        internal IEnumerable<string> GetSelectedPackages()
        {
            return Packages.Selected;
        }

        private Dictionary<string, Widget[]> GetTableRowsFromPackagePaths(Dictionary<string, PackageInfo> packages)
        {
            Dictionary<string, Widget[]> tableRows = new Dictionary<string, Widget[]>();
            foreach (var package in packages)
            {
                tableRows[package.Key] = new Widget[]
                {
                    new Label(package.Value.Name),
                    new Label(package.Value.Version.ToString()),
                    new Label(package.Value.OS),
                    new Label(package.Value.Arch),
                };
            }

            return tableRows;
        }
    }
}
