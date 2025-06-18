using System;
using System.Data;

namespace BlogProject.ViewModels
{
    public class SqlQueryViewModel
    {
        public string SqlQuery { get; set; }
        public DataTable Result { get; set; }
        public string ErrorMessage { get; set; }
    }
}

