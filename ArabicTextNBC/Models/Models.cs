using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATNBC.ViewModel
{
   
    public class JobVM
    {
        System.Web.Mvc.SelectList inputList;
        public System.Web.Mvc.SelectList InputList
        {
            get
            {
                List<SelectListItem> list  ;

                if (!string.IsNullOrEmpty(InputPath))
                {
                    DirectoryInfo di = new DirectoryInfo(InputPath);
                    list = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly).Select(item => new SelectListItem { Text = item.Name, Value = item.Name }).ToList();
                }

                else
                    list = new List<SelectListItem>();

                inputList = new SelectList(list, "Value", "Text");
                return inputList;
            }


        }

        [Required]
        [Display(Name = "Output Folder Name")]
        public string OutputFolder { get; set; }

        [Required]
        [Display(Name = "Input Folder Name")]
        public string InputFolder { get; set; }

        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string Message { get; set; }

        public string ClientId { get; set; }

        public JobVM()
        {
            
        }
    }
}