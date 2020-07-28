using Streemline3_1Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using System.ComponentModel.DataAnnotations;
namespace ClientPortal.ViewModels
{
    public class TasksViewModel
    {
        public IEnumerable<tblTask> Tasks { get; set; }
        public IEnumerable<tblTask> closedTasks { get; set; }
        public tblTask Task { get; set; }
        
        public IEnumerable<tblNote> Notes { get; set; }
        public tblNote Note { get; set; }
        public List<tblPerson> People { get; set; }
        public tblTaskStatus Status { get; set; }
        public tblPerson Person { get; set; }
        public tblCompany Company { get; set; }
        public tblPerson Client { get; set; }

        public tblFileAttachment Attachment { get; set; }
        public tblEstimate Estimate { get; set; }
    }
}