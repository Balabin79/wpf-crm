using DevExpress.DataAccess.ObjectBinding;
using System.ComponentModel;
using Dental.Models;


namespace Dental.Reports
{
    class SourceReportData
    {
        public Organization Organization { get; set; }
        public Employee Employee { get; set; }
        public Client PatientInfo { get; set; }

        public string AditionalString1 { get; set; }
        public string AditionalString2 { get; set; }
        public string AditionalString3 { get; set; }
        public string AditionalDateTime1 { get; set; }
        public string AditionalDateTime2 { get; set; }
        public string AditionalDateTime3 { get; set; }
        public string AditionalTime1 { get; set; }
        public string AditionalTime2 { get; set; }
        public string AditionalTime3 { get; set; }
    }

    [DisplayName("ReportData")]
    [HighlightedClass]
    class DataSource
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
