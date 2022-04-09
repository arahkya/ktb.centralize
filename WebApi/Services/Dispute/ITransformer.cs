using System.Data;
using WebLibrary;

namespace WebApi.Services.Dispute
{
    public interface ITransformer
    {
        IEnumerable<DisputeModel> Transform(DataTable datatable);
    }
}