using Dental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Repositories
{
    public static class PatientInfoRepository
    {
        public static List<PatientInfo> PatientInfo { get => GetPatientInfo();  }

        public static List<PatientInfo> GetPatientInfo() => new ApplicationContext().PatientInfo.ToList();        
    }
}
