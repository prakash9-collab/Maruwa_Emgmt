using iTextSharp.text;
using iTextSharp.text.pdf;
using Maruwa_Emgmt.Models;
//using System.Web.Mvc;
using Maruwa_Emgmt.Models;
using Maruwa_Emgmt.Repositories;
using Maruwa_Emgmt.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Maruwa_Emgmt.Models;
using Maruwa_Emgmt.Repositories;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Maruwa_Emgmt.Controllers
{
    [Authorize]
    public class DepartmentMasterController : Controller
    {
        private DepartmentRepository _departmentRepository;
        private string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public DepartmentMasterController()
        {
            _departmentRepository = new DepartmentRepository(_connectionString);
        }

        /// <summary>
        /// Display Department Master List with Pagination and Search
        /// </summary>
        public ActionResult Index(int page = 1, int pageSize = 10, string search = "", string sortBy = "RecordNo", bool sortDesc = false)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 5) pageSize = 5;
                if (pageSize > 100) pageSize = 100;

                var departments = _departmentRepository.GetAllDepartments(page, pageSize, search, sortBy, sortDesc);
                int totalRecords = _departmentRepository.GetTotalDepartmentCount(search);

                var viewModel = new DepartmentMasterViewModel
                {
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    SearchTerm = search,
                    SortBy = sortBy,
                    SortDescending = sortDesc
                };

                ViewBag.Departments = departments;
                ViewBag.TotalPages = Math.Ceiling((decimal)totalRecords / pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentPageSize = pageSize;
                ViewBag.SearchTerm = search;
                ViewBag.SortBy = sortBy;
                ViewBag.SortDescending = sortDesc;

                if (Session["empcode"] != null)
                {
                    ViewBag.EmployeeCode = Session["empcode"].ToString();
                    ViewBag.EmployeeName = Session["empname"] ?? "";
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading departments: " + ex.Message;
                return View(new DepartmentMasterViewModel { PageNumber = page, PageSize = pageSize });
            }
        }

        /// <summary>
        /// Create Department - GET
        /// </summary>
        public ActionResult Create()
        {
            try
            {
                if (Session["empcode"] == null)
                    return RedirectToAction("Login", "Account");

                return View(new DepartmentMaster { IsActive = true });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading create page: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Create Department - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DepartmentMaster department)
        {
            try
            {
                if (Session["empcode"] == null)
                    return Json(new { success = false, message = "Session expired. Please login again." });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    return Json(new { success = false, message = "Validation failed.", errors = errors.Select(e => e.ErrorMessage).ToList() });
                }

                if (_departmentRepository.DepartmentExists(department.DepartmentCode))
                {
                    return Json(new { success = false, message = "Department Already Exists. Please enter a different department code." });
                }

                string userId = Session["empcode"].ToString();

                if (_departmentRepository.AddDepartment(department, userId))
                {
                    TempData["SuccessMessage"] = "Department created successfully!";
                    return Json(new { success = true, message = "Department created successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to create department." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Edit Department - GET
        /// </summary>
        public ActionResult Edit(int id)
        {
            try
            {
                if (Session["empcode"] == null)
                    return RedirectToAction("Login", "Account");

                var department = _departmentRepository.GetDepartmentById(id);
                if (department == null)
                {
                    TempData["ErrorMessage"] = "Department not found.";
                    return RedirectToAction("Index");
                }

                return View(department);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading edit page: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Edit Department - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DepartmentMaster department)
        {
            try
            {
                if (Session["empcode"] == null)
                    return Json(new { success = false, message = "Session expired. Please login again." });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    return Json(new { success = false, message = "Validation failed.", errors = errors.Select(e => e.ErrorMessage).ToList() });
                }

                if (_departmentRepository.DepartmentExists(department.DepartmentCode, department.RecordNo))
                {
                    return Json(new { success = false, message = "Department Already Exists. Please enter a different department code." });
                }

                string userId = Session["empcode"].ToString();

                if (_departmentRepository.UpdateDepartment(department, userId))
                {
                    TempData["SuccessMessage"] = "Department updated successfully!";
                    return Json(new { success = true, message = "Department updated successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update department." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Delete Department
        /// </summary>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                if (Session["empcode"] == null)
                    return Json(new { success = false, message = "Session expired. Please login again." });

                string userId = Session["empcode"].ToString();

                if (_departmentRepository.DeleteDepartment(id, userId))
                {
                    return Json(new { success = true, message = "Department deleted successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete department." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Export to Excel
        /// </summary>
        public ActionResult ExportToExcel()
        {
            try
            {
                DataTable dt = _departmentRepository.ExportDepartments();

                if (dt.Rows.Count == 0)
                {
                    TempData["ErrorMessage"] = "No data available to export.";
                    return RedirectToAction("Index");
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Department Master");
                    worksheet.Cell(1, 1).InsertTable(dt);

                    using (var stream = new System.IO.MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DepartmentMaster_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting to Excel: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Export to CSV
        /// </summary>
        public ActionResult ExportToCSV()
        {
            try
            {
                DataTable dt = _departmentRepository.ExportDepartments();

                if (dt.Rows.Count == 0)
                {
                    TempData["ErrorMessage"] = "No data available to export.";
                    return RedirectToAction("Index");
                }

                var csv = new System.Text.StringBuilder();

                foreach (DataColumn column in dt.Columns)
                {
                    csv.Append('"' + column.ColumnName + "\",");
                }
                csv.AppendLine();

                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        csv.Append('"' + row[column].ToString().Replace("\"", "\\\"") + "\",");
                    }
                    csv.AppendLine();
                }

                return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "DepartmentMaster_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting to CSV: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Export to PDF
        /// </summary>
        public ActionResult ExportToPDF()
        {
            try
            {
                DataTable dt = _departmentRepository.ExportDepartments();

                if (dt.Rows.Count == 0)
                {
                    TempData["ErrorMessage"] = "No data available to export.";
                    return RedirectToAction("Index");
                }

                Document doc = new Document(PageSize.A4.Rotate());
                PdfPTable table = new PdfPTable(dt.Columns.Count);

                foreach (DataColumn column in dt.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName));
                    cell.BackgroundColor = new BaseColor(41, 128, 185);
                    cell.FixedHeight = 20f;
                    table.AddCell(cell);
                }

                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        table.AddCell(row[column].ToString());
                    }
                }

                using (var stream = new System.IO.MemoryStream())
                {
                    PdfWriter.GetInstance(doc, stream);
                    doc.Open();
                    doc.Add(new Paragraph("Department Master", new Font(Font.FontFamily.HELVETICA, 16f, Font.BOLD)));
                    doc.Add(new Paragraph(" "));
                    doc.Add(table);
                    doc.Close();

                    return File(stream.ToArray(), "application/pdf", "DepartmentMaster_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting to PDF: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Print Department List
        /// </summary>
        public ActionResult Print()
        {
            try
            {
                var departments = _departmentRepository.GetAllDepartments(1, 10000, "", "RecordNo", false);
                ViewBag.Departments = departments;
                return View(departments);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error preparing print: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
