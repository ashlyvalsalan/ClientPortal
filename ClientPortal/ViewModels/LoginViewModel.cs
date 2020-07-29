using System.Collections.Generic;
using Streemline3_1Poco;
using System.ComponentModel.DataAnnotations;
namespace ClientPortal.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter user name.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter the password.")]
        public string UserPassword { get; set; }
        public string LoginErrorMessage { get; set; }
        public IEnumerable<tblSolution> Solutions { get; set; }
        public tblIssue Issue { get; set; }
        public IEnumerable<tblSolutionComponent> Component { get; set; }
        public IEnumerable<tblSolutionComponent> Components { get; set; }
        public string Selectedstatus { get; set; }
        public IEnumerable<tblIssueStatus> Statuses { get; set; }
        public string SelectedIssueType { get; set; }
        public IEnumerable<tblIssueType> IssueType { get; set; }
    }
}