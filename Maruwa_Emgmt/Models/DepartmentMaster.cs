
using System;
using System.ComponentModel.DataAnnotations;

namespace Maruwa_Emgmt.Models
{
    public class DepartmentMaster
    {
        [Key]
        public int RecordNo { get; set; }

        [Required(ErrorMessage = "Department Code is required")]
        [StringLength(50, ErrorMessage = "Department Code cannot exceed 50 characters")]
        [Display(Name = "Department Code")]
        public string DepartmentCode { get; set; }

        [Required(ErrorMessage = "Department Name is required")]
        [StringLength(100, ErrorMessage = "Department Name cannot exceed 100 characters")]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Japan Head is required")]
        [StringLength(1, ErrorMessage = "Japan Head must be Y or N")]
        [Display(Name = "Japan Head")]
        public string JapanHead { get; set; }

        [Required(ErrorMessage = "Office is required")]
        [StringLength(50, ErrorMessage = "Office cannot exceed 50 characters")]
        [Display(Name = "Office")]
        public string Office { get; set; }

        [Required(ErrorMessage = "Got Section is required")]
        [StringLength(1, ErrorMessage = "Got Section must be Y or N")]
        [Display(Name = "Got Section")]
        public string GotSection { get; set; }

        [Required(ErrorMessage = "Prefix is required")]
        [StringLength(50, ErrorMessage = "Prefix cannot exceed 50 characters")]
        [Display(Name = "Prefix")]
        public string Prefix { get; set; }

        [StringLength(100)]
        [Display(Name = "Parent Position Code")]
        public string ParentPositionCode { get; set; }

        [Display(Name = "Parent Department ID")]
        public int? ParentDepartmentId { get; set; }

        [StringLength(100, ErrorMessage = "Created By cannot exceed 100 characters")]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; }

        [StringLength(100, ErrorMessage = "Edited By cannot exceed 100 characters")]
        [Display(Name = "Edited By")]
        public string EditedBy { get; set; }

        [Display(Name = "Edited On")]
        public DateTime? EditedOn { get; set; }

        [Required(ErrorMessage = "Active Status is required")]
        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters")]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
    }

    public class DepartmentMasterViewModel
    {
        public DepartmentMaster Department { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public string SearchTerm { get; set; }
        public string SortBy { get; set; } = "RecordNo";
        public bool SortDescending { get; set; }
    }

    public class DepartmentGridViewModel
    {
        public int RecordNo { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string JapanHead { get; set; }
        public string Office { get; set; }
        public string GotSection { get; set; }
        public string Prefix { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
