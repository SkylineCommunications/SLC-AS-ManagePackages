namespace ManageInstallPackages_1.UploadWindow
{
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.SoftwareBundle;

	internal class UploadView : Dialog
	{
		public UploadView(IEngine engine) : base(engine)
		{
			Title = "Upload Package";
			Width = 400;
			Height = 150;
			SetColumnWidth(0, 110);
			SetColumnWidth(1, 110);
			SetColumnWidth(2, 110);

			CloseButton = new Button("Close");
			FinishButton = new Button("Finish Upload");
			FileSelector = new FileSelector();
			Feedback = new Label() { IsVisible = false };
		}

		public Button CloseButton { get; set; }

		public Button FinishButton { get; set; }

		internal FileSelector FileSelector { get; set; }

		internal Label Feedback { get; set; }

		public void Initialize()
		{
			Clear();
			int row = 0;
			AddWidget(FileSelector, row++, 0, 1, 3);
			AddWidget(CloseButton, row, 0);
			AddWidget(FinishButton, row++, 2);
			AddWidget(Feedback, row, 0, 1, 3);
		}

		public void SetFeedback(string message)
		{
			Feedback.IsVisible = true;
			Feedback.Text = message;
		}

		public IEnumerable<string> GetUploadedPackages()
		{
			return FileSelector.UploadedFilePaths;
		}
	}
}