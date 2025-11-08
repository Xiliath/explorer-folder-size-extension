using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderSizeExtension
{
    /// <summary>
    /// UI form for displaying folder size calculation progress
    /// </summary>
    public class FolderSizeCalculatorForm : Form
    {
        private readonly string _folderPath;
        private readonly bool _includeSubfolders;
        private CancellationTokenSource? _cancellationTokenSource;

        private Label _titleLabel = null!;
        private Label _currentFolderLabel = null!;
        private ProgressBar _progressBar = null!;
        private Label _statusLabel = null!;
        private Button _cancelButton = null!;
        private Button _closeButton = null!;
        private TextBox _resultsTextBox = null!;

        public FolderSizeCalculatorForm(string folderPath, bool includeSubfolders)
        {
            _folderPath = folderPath;
            _includeSubfolders = includeSubfolders;

            InitializeComponents();
            StartCalculation();
        }

        private void InitializeComponents()
        {
            // Form properties
            Text = "Calculating Folder Size";
            Size = new Size(600, 400);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Title
            _titleLabel = new Label
            {
                Text = _includeSubfolders ? "Calculating sizes for all subfolders..." : "Calculating folder size...",
                Location = new Point(20, 20),
                Size = new Size(560, 25),
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold)
            };

            // Current folder label
            _currentFolderLabel = new Label
            {
                Text = $"Folder: {_folderPath}",
                Location = new Point(20, 50),
                Size = new Size(560, 40),
                AutoEllipsis = true
            };

            // Progress bar
            _progressBar = new ProgressBar
            {
                Location = new Point(20, 100),
                Size = new Size(560, 25),
                Style = ProgressBarStyle.Marquee
            };

            // Status label
            _statusLabel = new Label
            {
                Text = "Starting...",
                Location = new Point(20, 135),
                Size = new Size(560, 20),
                AutoEllipsis = true
            };

            // Results text box
            _resultsTextBox = new TextBox
            {
                Location = new Point(20, 165),
                Size = new Size(560, 150),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Consolas", 9)
            };

            // Cancel button
            _cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(420, 325),
                Size = new Size(75, 30)
            };
            _cancelButton.Click += CancelButton_Click;

            // Close button
            _closeButton = new Button
            {
                Text = "Close",
                Location = new Point(505, 325),
                Size = new Size(75, 30),
                Enabled = false
            };
            _closeButton.Click += (s, e) => Close();

            // Add controls
            Controls.AddRange(new Control[]
            {
                _titleLabel,
                _currentFolderLabel,
                _progressBar,
                _statusLabel,
                _resultsTextBox,
                _cancelButton,
                _closeButton
            });
        }

        private async void StartCalculation()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                if (_includeSubfolders)
                {
                    await CalculateSubfoldersAsync();
                }
                else
                {
                    await CalculateSingleFolderAsync();
                }
            }
            catch (OperationCanceledException)
            {
                _statusLabel.Text = "Calculation cancelled";
                AppendResult("Calculation was cancelled by user.");
            }
            catch (Exception ex)
            {
                _statusLabel.Text = "Error occurred";
                AppendResult($"Error: {ex.Message}");
            }
            finally
            {
                CalculationComplete();
            }
        }

        private async Task CalculateSingleFolderAsync()
        {
            var progress = new Progress<string>(currentPath =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => _statusLabel.Text = $"Scanning: {Path.GetFileName(currentPath)}..."));
                }
                else
                {
                    _statusLabel.Text = $"Scanning: {Path.GetFileName(currentPath)}...";
                }
            });

            var result = await FolderSizeCalculator.CalculateAsync(_folderPath, _cancellationTokenSource!.Token, progress);

            if (result.WasCompleted)
            {
                // Cache the result
                FolderSizeCache.Instance.SetFolderInfo(_folderPath, result.TotalSize, result.TotalItemCount);

                // Display results
                AppendResult($"Folder: {_folderPath}");
                AppendResult($"Total Size: {FormatUtils.FormatSize(result.TotalSize)} ({result.TotalSize:N0} bytes)");
                AppendResult($"Files: {result.FileCount:N0}");
                AppendResult($"Folders: {result.FolderCount:N0}");
                AppendResult($"Total Items: {result.TotalItemCount:N0}");

                _statusLabel.Text = $"Complete - {FormatUtils.FormatSize(result.TotalSize)}";
            }
        }

        private async Task CalculateSubfoldersAsync()
        {
            int completedCount = 0;
            var subfolders = Directory.GetDirectories(_folderPath);
            int totalCount = subfolders.Length;

            AppendResult($"Found {totalCount} subfolder(s) to calculate...\n");

            var progress = new Progress<(string folder, FolderSizeCalculator.CalculationResult result)>(tuple =>
            {
                completedCount++;
                string folderName = Path.GetFileName(tuple.folder);
                string sizeStr = FormatUtils.FormatSize(tuple.result.TotalSize);

                AppendResult($"[{completedCount}/{totalCount}] {folderName}: {sizeStr}");
                _statusLabel.Text = $"Processed {completedCount} of {totalCount} folders...";
            });

            await FolderSizeCalculator.CalculateSubfoldersAsync(_folderPath, progress, _cancellationTokenSource!.Token);

            _statusLabel.Text = $"Complete - {completedCount} folder(s) processed";
        }

        private void AppendResult(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendResult(text)));
                return;
            }

            _resultsTextBox.AppendText(text + Environment.NewLine);
        }

        private void CalculationComplete()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(CalculationComplete));
                return;
            }

            _progressBar.Style = ProgressBarStyle.Continuous;
            _progressBar.Value = 100;
            _cancelButton.Enabled = false;
            _closeButton.Enabled = true;

            // Refresh Explorer
            AppendResult("\nCalculation complete. Refresh Windows Explorer (F5) to see updated sizes.");
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            _cancelButton.Enabled = false;
            _statusLabel.Text = "Cancelling...";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            base.OnFormClosing(e);
        }
    }
}
