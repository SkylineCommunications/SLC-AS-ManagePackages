namespace ManageInstallPackages_1.PackageOverview
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using ManageInstallPackages_1.Extensions;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.Packages;

	internal class ManagePackageController
	{
		private readonly ManagePackagesView view;
		private readonly IEngine engine;
		private readonly string folderPath;
		private readonly PackageInfo filter;
		private Dictionary<string, PackageInfo> packages;

		public ManagePackageController(IEngine engine, ManagePackagesView view, string folderPath, PackageInfo filter)
		{
			this.view = view;
			this.engine = engine;
			this.folderPath = folderPath;
			this.filter = filter;
			view.FinishButton.Pressed += OnNextButtonPressed;
			view.DeleteButton.Pressed += OnDeleteButtonPressed;
			view.UploadNewButton.Pressed += OnUploadButtonPressed;
		}

		internal event EventHandler<EventArgs> Finish;

		internal event EventHandler<EventArgs> Upload;

		public void RetrievePackagesFromFolder()
		{
			packages = new Dictionary<string, PackageInfo>();
			string[] packageNameFolders = Directory.GetDirectories(folderPath);
			foreach (string packageFolder in packageNameFolders)
			{
				string[] packageVersionFolders = Directory.GetDirectories(packageFolder);
				foreach (var packageVersionFolder in packageVersionFolders)
				{
					try
					{
						var packageInfo = Packages.GetUnZippedPackage(packageVersionFolder).PackageInfo;
						if (MatchesFilter(packageInfo))
						{
							packages[packageVersionFolder] = packageInfo;
						}
					}
					catch (Exception ex)
					{
						engine.GenerateInformation($"Failed to parse package '{packageVersionFolder}': {ex}");
					}
				}
			}

			view.Initialize(packages);
		}

		public void OnNextButtonPressed(object sender, EventArgs e)
		{
			Finish?.Invoke(this, EventArgs.Empty);
		}

		public void OnUploadButtonPressed(object sender, EventArgs e)
		{
			Upload?.Invoke(this, EventArgs.Empty);
		}

		public void OnDeleteButtonPressed(object sender, EventArgs e)
		{
			var folders = view.GetSelectedPackages();
			foreach (var folder in folders)
			{
				var unzipped = Packages.GetUnZippedPackage(folder);
				engine.GenerateInformation($"Deleting {folder}");
				unzipped.Delete();
			}

			RetrievePackagesFromFolder();
		}

		private bool MatchesFilter(PackageInfo packageInfo)
		{
			if (!packageInfo.Name.Like(filter.Name))
			{
				return false;
			}

			if (packageInfo.Version < filter.Version)
			{
				return false;
			}

			if (!packageInfo.OS.Like(filter.OS))
			{
				return false;
			}

			if (!packageInfo.Arch.Like(filter.Arch))
			{
				return false;
			}

			return true;
		}
	}
}
