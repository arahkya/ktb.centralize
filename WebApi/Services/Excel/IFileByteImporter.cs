using System.Data;

namespace WebApi.Services.Excel
{
    public interface IFileByteImporter
    {
        Tuple<DataTable,DataTable> Import(byte[] fileByte);
    }
}