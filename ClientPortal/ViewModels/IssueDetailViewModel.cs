﻿//using ClientPortal.Models;
using Streemline3_1Poco;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClientPortal.ViewModels
{
    public class IssueDetailViewModel
    {
        public IEnumerable<tblIssueStatus> IssueStatuses { get; set; }

        public tblIssue Issue { get; set; }
        public tblNote Note { get; set; }
        public tblIssueStatus IssueStatus { get; set; }
        public string SelectedNotes { get; set; }
        public IEnumerable<tblNote> Notes { get; set; }
        public List<tblPerson> People { get; set; }
        public IEnumerable<tblFileAttachment> Attachments { get; set; }
        public tblFileAttachment Attachment { get; set; }
        public IEnumerable<tblPerson> Person { get; set; }
        public tblSolution Solutions { get; set; }
        public IEnumerable<tblIssueStatus> Statuses { get; set; }
        public string SelectedComponents { get; set; }
        public IEnumerable<tblSolutionComponent> Components { get; set; }
        public IEnumerable<tblIssueType> Types { get; set; }
      


    }
} 