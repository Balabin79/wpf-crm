using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.DataAccess.ObjectBinding;
using System.ComponentModel;

namespace Dental.Reports
{
    public class SourceReportData
    {
        public string DocumentSource { get; set; }
        public string OrganizationName { get; set; } = "dsadadasds";
        public string PatientFullNameName { get; set; }
        public string EmployeeFullNameName { get; set; }
    }

    [DisplayName("ReportData")]
    [HighlightedClass]
    public class DataSource
    {

        private SourceReportData data;
        public DataSource(SourceReportData source)
        {
            data = source;
        }

        [HighlightedMember]
        public SourceReportData GetData()
        {
            return data; 
        }
    }


}
