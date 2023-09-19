namespace ManageInstallPackages_1
{
	using System;
	using System.IO;
	using ManageInstallPackages_1.PackageOverview;
	using ManageInstallPackages_1.UploadWindow;
	using ManageInstallPackages_1.View;
	using Newtonsoft.Json;
	using NuGet.Versioning;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.Packages;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private InteractiveController controller;

		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			// engine.ShowUI();
			engine.FindInteractiveClient("Launching Package Manager", 100, "user:" + engine.UserLoginName, AutomationScriptAttachOptions.AttachImmediately);
			controller = new InteractiveController(engine);
			engine.Timeout = new TimeSpan(1, 0, 0);

			PackageInfo filter = GetFilterFromInput(engine, engine.GetScriptParam("Filter").Value);
			string folderPath = @"C:\Skyline DataMiner\Documents\DMA_COMMON_DOCUMENTS\InstallPackages";

			try
			{
				Directory.CreateDirectory(folderPath);
				ManagePackagesView packagesOverview = new ManagePackagesView(engine);
				ManagePackageController packagesController = new ManagePackageController(engine, packagesOverview, folderPath, filter);
				UploadView uploadView = new UploadView(engine);
				UploadController uploadController = new UploadController(engine, uploadView, folderPath);
				packagesController.RetrievePackagesFromFolder();

				packagesController.Finish += (sender, args) =>
				{
					engine.ExitSuccess("Manage Packages Completed.");
				};

				packagesController.Upload += (sender, args) =>
				{
					uploadView.Initialize();
					controller.ShowDialog(uploadView);
				};

				uploadController.Close += (sender, args) =>
				{
					packagesController.RetrievePackagesFromFolder();
					controller.ShowDialog(packagesOverview);
				};

				controller.Run(packagesOverview);
			}
			catch (ScriptAbortException ex)
			{
				if (ex.Message.Contains("ExitFail"))
				{
					HandleknownException(engine, ex);
				}
				else
				{
					// Do nothing as it's an exitsuccess event
				}
			}
			catch (Exception ex)
			{
				HandleUnknownException(engine, ex);
			}
			finally
			{
				engine.AddScriptOutput("status", "success");
			}
		}

		private void HandleUnknownException(IEngine engine, Exception ex)
		{
			var message = "ERR| An unexpected error occurred, please contact skyline and provide the following information: \n" + ex;
			try
			{
				controller.Run(new ErrorView(engine, ex));
			}
			catch (Exception ex_two)
			{
				engine.GenerateInformation("ERR| Unable to show error message window: " + ex_two);
			}

			engine.GenerateInformation(message);
		}

		private void HandleknownException(IEngine engine, Exception ex)
		{
			var message = "ERR| Script has been canceled because of the following error: \n" + ex;
			try
			{
				controller.Run(new ErrorView(engine, ex));
			}
			catch (Exception ex_two)
			{
				engine.GenerateInformation("ERR| Unable to show error message window: " + ex_two);
			}

			engine.GenerateInformation(message);
		}

		private PackageInfo GetFilterFromInput(IEngine engine, string input)
		{
			try
			{
				var filter = JsonConvert.DeserializeObject<PackageInfo>(input);
				if (filter != null)
				{
					if (string.IsNullOrWhiteSpace(filter.Name))
					{
						filter.Name = "*";
					}

					if (filter.Version == null)
					{
						filter.Version = new SemanticVersion(0, 0, 0);
					}

					if (string.IsNullOrWhiteSpace(filter.OS))
					{
						filter.OS = "*";
					}

					if (string.IsNullOrWhiteSpace(filter.Arch))
					{
						filter.Arch = "*";
					}

					return filter;
				}
			}
			catch (Exception ex)
			{
				engine.GenerateInformation($"Failed to parse input filter: {ex}");
			}

			return new PackageInfo("*", new SemanticVersion(0, 0, 0), "*", "*");
		}
	}
}