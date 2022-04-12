using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.ObjectModel;
using WebLibrary;
using WebUi.Services;
using WebUi.ViewModels;

namespace WebUi.Pages
{
    public partial class UploadFile : IObserver<IEnumerable<DisputeViewModel>>
    {
        private readonly long fileSizeLimit = 31457280;
        private IBrowserFile? fileUpload;        

        public bool IsProcessing { get; set; }
        public string FileErrorMessage { get; set; } = string.Empty;
        public float FileSizeLimitMB
        {
            get => (fileSizeLimit / 1024) / 1024;
        }
        public string StatusMessage { get; set; } = string.Empty;

        [Inject]
        public FileReaderService? FileReaderService { get; set; }

        [Inject]
        private DisputeManipulatorService? DisputeManipulatorService { get; set; }

        [Inject]
        private DisputeWorkerManagerService? DisputeWorkerManagerService { get; set; }        

        public IBrowserFile? FileUpload
        {
            get => fileUpload;
            set
            {
                if (value == null) return;

                if (value.Size > fileSizeLimit)
                {
                    FileErrorMessage = "File size over limit.";
                    return;
                }
                
                fileUpload = value;
            }
        }

        public void OnInputFile_Changed(InputFileChangeEventArgs e)
        {
            FileUpload = e.File;
        }

        public async Task Upload()
        {
            IsProcessing = true;

            if (FileUpload == null || FileReaderService == null || DisputeWorkerManagerService == null) return;

            DisputeWorkerManagerService.Setup(5);
            FileReaderService.Subscribe(this);

            using var browserFileStream = FileUpload.OpenReadStream(maxAllowedSize: fileSizeLimit);
            await FileReaderService.ReadFileAsync(browserFileStream);

            IsProcessing = false;
        }
        
        #region Observer Implement
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(IEnumerable<DisputeViewModel> value)
        {
            StatusMessage = $"Record Read: {value.Count()}";
        } 
        #endregion
    }
}