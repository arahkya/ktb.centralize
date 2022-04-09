using System.Data;
using WebLibrary;

namespace WebApi.Services.Dispute
{
    public class Transformer : ITransformer
    {
        protected readonly DisputeTypes DisputeType;
        protected readonly string BranchCodeColumnName;
        protected readonly string TerminalColumnName;
        protected readonly string TransactionDateColumnName;

        public Transformer(DisputeTypes disputeType, string branchCodeColumnName, string terminalColumnName, string transactionDateColumnName)
        {
            this.DisputeType = disputeType;
            this.BranchCodeColumnName = branchCodeColumnName;
            this.TerminalColumnName = terminalColumnName;
            this.TransactionDateColumnName = transactionDateColumnName;
        }

        public IEnumerable<DisputeModel> Transform(DataTable datatable)
        {
            foreach (DataRow dr in datatable.Rows)
            {
                var transactionDate = DateTime.MinValue;

                if (!DateTime.TryParse(dr[TransactionDateColumnName]?.ToString(), out transactionDate))
                {
                    throw new FormatException($"Transaction Date format invalid ({dr[TransactionDateColumnName]?.ToString()})");
                }

                var branchCodeNumber = -1;
                if (!Int32.TryParse(dr[BranchCodeColumnName]?.ToString(), System.Globalization.NumberStyles.Number, null, out branchCodeNumber))
                {
                    throw new FormatException($"Branch code format invalid ({dr[BranchCodeColumnName]?.ToString()})");
                }

                yield return new DisputeModel
                {
                    BranchCode = branchCodeNumber.ToString(),
                    TerminalId = dr[TerminalColumnName]?.ToString() ?? string.Empty,
                    TransactionDate = transactionDate,
                    DisputeType = DisputeType
                };
            }
        }
    }
}