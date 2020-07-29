//using ClientPortal.Models;
using Streemline3_1Poco;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClientPortal.ViewModels
{
    public class IssueListViewModel
    {
        public IEnumerable<tblIssue> clientIssues { get; set; }
        public IEnumerable<tblIssue> clientIssuesExceptClosed { get; set; }
     
        public tblSolution Solutions { get; set; }
       
        public tblIssue Issue { get; set; }
        public tblNote Note { get; set; }
        public IEnumerable<tblIssue> Issues { get; set; }
        
        public string SelectedNotes { get; set; }
        public IEnumerable<tblNote> Notes { get; set; }

        public string Selectedstatus { get; set; }
        public IEnumerable<tblIssueStatus> Statuses { get; set; }
        public string Selectedtypes { get; set; }
        public IEnumerable<tblIssueType> Types { get; set; }
        public string SelectedComponents { get; set; }
        public IEnumerable<tblSolutionComponent> Components { get; set; }
    
        public List<tblPerson> People { get; set; }
        public string SelectedAttachments { get; set; }
        public IEnumerable<tblFileAttachment> Attachments { get; set; }
        public IEnumerable<tblPerson> Person { get; set; }
       

    }
}