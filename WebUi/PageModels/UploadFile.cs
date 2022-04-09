using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebLibrary;

namespace WebUi.Pages
{
    public partial class UploadFile
    {
        private long fileSizeLimit = 31457280;
        private IBrowserFile? fileUpload;
        private FilePostModel? filePostModel;

        public bool IsFileUploaded { get; set; }
        public bool IsProcessing { get; set; }
        public string FileErrorMessage { get; set; } = string.Empty;
        public float FileSizeLimitMB
        {
            get => (fileSizeLimit / 1024) / 1024;
        }
        public string StatusMessage { get; set; } = string.Empty;

        [Inject]
        public HttpClient? httpClient { get; set; }

        public ObservableCollection<DisputeModel> Items = new ObservableCollection<DisputeModel>();

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

                filePostModel = new FilePostModel
                {
                    FileName = Path.GetFileNameWithoutExtension(value.Name),
                    FileExtension = Path.GetExtension(value.Name)
                };

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

            if (FileUpload == null || filePostModel == null || httpClient == null) return;

            using var fs = FileUpload.OpenReadStream(maxAllowedSize: fileSizeLimit);
            using var ms = new MemoryStream();
            var buffer = new byte[16 * 1024];
            var index = 0;

            while ((index = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, index);
            }

            filePostModel.Content = ms.ToArray();

            var json = string.Empty;

            using (var sm = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync<FilePostModel>(sm, filePostModel);
                sm.Position = 0;

                using (var sr = new StreamReader(sm))
                {
                    json = await sr.ReadToEndAsync();
                }
            }

            var url = "http://localhost:5263/filemanager";
            var stringContent = new StringContent(json, Encoding.Default, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Content = stringContent;

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            
            var responseStream = await response.Content.ReadAsStreamAsync();
            var disputeModels = JsonSerializer.DeserializeAsyncEnumerable<DisputeModel>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (disputeModels != null)
            {
                await foreach (DisputeModel? item in disputeModels)
                {
                    if (item == null) continue;
             
                    Items.Add(item);
                }
            }

            IsFileUploaded = true;
            IsProcessing = false;
        }
    }
}