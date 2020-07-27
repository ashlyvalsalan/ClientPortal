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
    }
}