namespace ManageInstallPackages_1.UploadWindow
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.SoftwareBundle;

	internal class UploadController
	{
		private readonly UploadView view;
		private readonly IEngine engine;
		private readonly string folderPath;
		private HashSet<string> UnzippedPackages = new HashSet<string>();

		public UploadController(IEngine engine, UploadView view, string folderPath)
		{
			this.view = view;
			this.engine = engine;
			this.folderPath = folderPath;
			view.CloseButton.Pressed += OnCloseButtonPressed;
			view.FinishButton.Pressed += OnFinishButtonPressed;
		}

		internal event EventHandler<EventArgs> Close;

		public void OnCloseButtonPressed(object sender, EventArgs e)
		{
			Close?.Invoke(this, EventArgs.Empty);
		}

		public void OnFinishButtonPressed(object sender, EventArgs e)
		{
			if (UnpackPackages())
			{
				Close?.Invoke(this, EventArgs.Empty);
			}
		}

		public bool UnpackPackages()
		{
			var filePaths = view.GetUploadedPackages();
			if (!filePaths.Any())
			{
				return false;
			}

			bool succeeded = true;
			foreach (var file in filePaths)
			{
				if (UnzippedPackages.Contains(file))
				{
					// Already unzipped and deleted
					continue;
				}

				try
				{
					var zipped = SoftwareBundles.GetZippedSoftwareBundle(file);
					string packageFolderPath = Path.Combine(folderPath, zipped.SoftwareBundleInfo.Name, zipped.SoftwareBundleInfo.Version.ToString());
					if (Directory.Exists(packageFolderPath))
					{
						Directory.Delete(packageFolderPath, true);
					}

					Directory.CreateDirectory(packageFolderPath);
					zipped.CreateUnzippedSoftwareBundle(packageFolderPath);
					zipped.Delete();
					UnzippedPackages.Add(file);
				}
				catch (Exception ex)
				{
					view.SetFeedback(ex.ToString());
					succeeded = false;
				}
			}

			return succeeded;
		}
	}
}