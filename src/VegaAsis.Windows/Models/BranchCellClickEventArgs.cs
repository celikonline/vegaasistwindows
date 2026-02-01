using System;

namespace VegaAsis.Windows.Models
{
    public class BranchCellClickEventArgs : EventArgs
    {
        public string Branch { get; }
        public string CompanyName { get; }

        public BranchCellClickEventArgs(string branch, string companyName)
        {
            Branch = branch ?? string.Empty;
            CompanyName = companyName ?? string.Empty;
        }
    }
}
