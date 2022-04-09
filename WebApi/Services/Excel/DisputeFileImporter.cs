using System.Data;
using ExcelDataReader;

namespace WebApi.Services.Excel
{
    public class DisputeFileImporter : IFileByteImporter
    {
        public Tuple<DataTable, DataTable> Import(byte[] fileBytes)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var fileStream = new MemoryStream(fileBytes);
            using var excelReader = ExcelReaderFactory.CreateReader(fileStream);

            var excelDataSetConfiguration = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = ((cfg) => new ExcelDataTableConfiguration { UseHeaderRow = true })
            };

            var dataset = excelReader.AsDataSet(excelDataSetConfiguration);
            var atmDataTable = dataset.Tables["DISPUTE_ATM"];
            var rcmDataTable = dataset.Tables["DISPUTE_RCM"];

            if (atmDataTable == null)
            {
                throw new Exception("Dispute ATM sheet not found (DISPUTE_ATM)");
            }

            if (rcmDataTable == null)
            {
                throw new Exception("Dispute RCM sheet not found (DISPUTE_RCM)");
            }

            return new Tuple<DataTable, DataTable>(atmDataTable, rcmDataTable);
        }
    }
}