
using Microsoft.Ajax.Utilities;
using Streemline3_1Poco;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClientPortal.ViewModels
{
    public class IssueCreateViewModel
    {
        public IEnumerable<tblSolution> Solutions { get; set; }
        
        public tblIssue Issue { get; set; }

        public tblSolution solution { get; set; }
        public IEnumerable<tblSolutionComponent> Component { get; set; }
        public IEnumerable<tblSolutionComponent> Components { get; set; }

        public string Selectedstatus { get; set; }
        public IEnumerable<tblIssueStatus> Statuses { get; set; }
        
       
        public string SelectedIssueType { get; set; }
        public IEnumerable<tblIssueType> IssueType { get; set; }
        public IEnumerable<tblFileAttachment> fileAttachments { get; set; }

       

    }
}