using Maruwa_Emgmt.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Maruwa_Emgmt.Models;
using Maruwa_Emgmt.Emanagement.SqlHelper;

namespace Maruwa_Emgmt.Repositories
{
    public class DepartmentRepository
    {
        private string _connectionString;

        public DepartmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Get all departments with pagination and search
        /// </summary>
        public List<DepartmentGridViewModel> GetAllDepartments(int pageNumber, int pageSize, string searchTerm = "", string sortBy = "RecordNo", bool sortDescending = false)
        {
            try
            {
                string query = @"
                    SELECT RecordNo, DepartmentCode, DepartmentName, JapanHead, Office, 
                           GotSection, Prefix, CreatedBy, CreatedOn, EditedBy, EditedOn, 
                           ISNULL(IsActive, 1) as IsActive
                    FROM department
                    WHERE 1=1";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += @" AND (DepartmentCode LIKE '%' + @searchTerm + '%' 
                               OR DepartmentName LIKE '%' + @searchTerm + '%'
                               OR Prefix LIKE '%' + @searchTerm + '%')";
                }

                query += @" ORDER BY " + (sortDescending ? sortBy + " DESC" : sortBy + " ASC");
                query += @" OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@offset", (pageNumber - 1) * pageSize);
                    command.Parameters.AddWithValue("@pageSize", pageSize);

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        command.Parameters.AddWithValue("@searchTerm", searchTerm);
                    }

                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    var departments = new List<DepartmentGridViewModel>();
                    foreach (DataRow row in dt.Rows)
                    {
                        departments.Add(new DepartmentGridViewModel
                        {
                            RecordNo = Convert.ToInt32(row["RecordNo"]),
                            DepartmentCode = row["DepartmentCode"].ToString(),
                            DepartmentName = row["DepartmentName"].ToString(),
                            JapanHead = row["JapanHead"].ToString(),
                            Office = row["Office"].ToString(),
                            GotSection = row["GotSection"].ToString(),
                            Prefix = row["Prefix"].ToString(),
                            CreatedBy = row["CreatedBy"].ToString(),
                            CreatedOn = row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]) : (DateTime?)null,
                            EditedBy = row["EditedBy"].ToString(),
                            EditedOn = row["EditedOn"] != DBNull.Value ? Convert.ToDateTime(row["EditedOn"]) : (DateTime?)null,
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        });
                    }

                    return departments;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving departments: " + ex.Message);
            }
        }

        /// <summary>
        /// Get total department count
        /// </summary>
        public int GetTotalDepartmentCount(string searchTerm = "")
        {
            try
            {
                string query = "SELECT COUNT(*) FROM department WHERE 1=1";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += @" AND (DepartmentCode LIKE '%' + @searchTerm + '%' 
                               OR DepartmentName LIKE '%' + @searchTerm + '%'
                               OR Prefix LIKE '%' + @searchTerm + '%')";
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        command.Parameters.AddWithValue("@searchTerm", searchTerm);
                    }

                    connection.Open();
                    return (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting department count: " + ex.Message);
            }
        }

        /// <summary>
        /// Get department by ID
        /// </summary>
        public DepartmentMaster GetDepartmentById(int recordNo)
        {
            try
            {
                string query = @"
                    SELECT RecordNo, DepartmentCode, DepartmentName, JapanHead, Office, 
                           GotSection, Prefix, ParentPositionCode, ParentDepartmentId,
                           CreatedBy, CreatedOn, EditedBy, EditedOn, ISNULL(IsActive, 1) as IsActive, Remarks
                    FROM department
                    WHERE RecordNo = @RecordNo";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RecordNo", recordNo);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return new DepartmentMaster
                        {
                            RecordNo = Convert.ToInt32(reader["RecordNo"]),
                            DepartmentCode = reader["DepartmentCode"].ToString(),
                            DepartmentName = reader["DepartmentName"].ToString(),
                            JapanHead = reader["JapanHead"].ToString(),
                            Office = reader["Office"].ToString(),
                            GotSection = reader["GotSection"].ToString(),
                            Prefix = reader["Prefix"].ToString(),
                            ParentPositionCode = reader["ParentPositionCode"].ToString(),
                            ParentDepartmentId = reader["ParentDepartmentId"] != DBNull.Value ? Convert.ToInt32(reader["ParentDepartmentId"]) : (int?)null,
                            CreatedBy = reader["CreatedBy"].ToString(),
                            CreatedOn = reader["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedOn"]) : (DateTime?)null,
                            EditedBy = reader["EditedBy"].ToString(),
                            EditedOn = reader["EditedOn"] != DBNull.Value ? Convert.ToDateTime(reader["EditedOn"]) : (DateTime?)null,
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            Remarks = reader["Remarks"].ToString()
                        };
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving department: " + ex.Message);
            }
        }

        /// <summary>
        /// Check if department already exists
        /// </summary>
        public bool DepartmentExists(string departmentCode, int? excludeRecordNo = null)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM department WHERE DepartmentCode = @DepartmentCode";

                if (excludeRecordNo.HasValue)
                {
                    query += " AND RecordNo != @RecordNo";
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DepartmentCode", departmentCode);

                    if (excludeRecordNo.HasValue)
                    {
                        command.Parameters.AddWithValue("@RecordNo", excludeRecordNo.Value);
                    }

                    connection.Open();
                    return (int)command.ExecuteScalar() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking department existence: " + ex.Message);
            }
        }

        /// <summary>
        /// Add new department
        /// </summary>
        public bool AddDepartment(DepartmentMaster department, string currentUserId)
        {
            try
            {
                string query = @"
                    INSERT INTO department (DepartmentCode, DepartmentName, JapanHead, Office, 
                                           GotSection, Prefix, ParentPositionCode, ParentDepartmentId,
                                           CreatedBy, CreatedOn, IsActive, Remarks)
                    VALUES (@DepartmentCode, @DepartmentName, @JapanHead, @Office, 
                            @GotSection, @Prefix, @ParentPositionCode, @ParentDepartmentId,
                            @CreatedBy, @CreatedOn, @IsActive, @Remarks)";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DepartmentCode", department.DepartmentCode);
                    command.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
                    command.Parameters.AddWithValue("@JapanHead", department.JapanHead);
                    command.Parameters.AddWithValue("@Office", department.Office);
                    command.Parameters.AddWithValue("@GotSection", department.GotSection);
                    command.Parameters.AddWithValue("@Prefix", department.Prefix);
                    command.Parameters.AddWithValue("@ParentPositionCode", department.ParentPositionCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ParentDepartmentId", department.ParentDepartmentId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedBy", currentUserId);
                    command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    command.Parameters.AddWithValue("@IsActive", department.IsActive);
                    command.Parameters.AddWithValue("@Remarks", department.Remarks ?? (object)DBNull.Value);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding department: " + ex.Message);
            }
        }

        /// <summary>
        /// Update existing department
        /// </summary>
        public bool UpdateDepartment(DepartmentMaster department, string currentUserId)
        {
            try
            {
                string query = @"
                    UPDATE department 
                    SET DepartmentName = @DepartmentName, JapanHead = @JapanHead, Office = @Office, 
                        GotSection = @GotSection, Prefix = @Prefix, ParentPositionCode = @ParentPositionCode,
                        ParentDepartmentId = @ParentDepartmentId, EditedBy = @EditedBy, EditedOn = @EditedOn,
                        IsActive = @IsActive, Remarks = @Remarks
                    WHERE RecordNo = @RecordNo";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RecordNo", department.RecordNo);
                    command.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
                    command.Parameters.AddWithValue("@JapanHead", department.JapanHead);
                    command.Parameters.AddWithValue("@Office", department.Office);
                    command.Parameters.AddWithValue("@GotSection", department.GotSection);
                    command.Parameters.AddWithValue("@Prefix", department.Prefix);
                    command.Parameters.AddWithValue("@ParentPositionCode", department.ParentPositionCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ParentDepartmentId", department.ParentDepartmentId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EditedBy", currentUserId);
                    command.Parameters.AddWithValue("@EditedOn", DateTime.Now);
                    command.Parameters.AddWithValue("@IsActive", department.IsActive);
                    command.Parameters.AddWithValue("@Remarks", department.Remarks ?? (object)DBNull.Value);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating department: " + ex.Message);
            }
        }

        /// <summary>
        /// Delete department (soft delete by marking as inactive)
        /// </summary>
        public bool DeleteDepartment(int recordNo, string currentUserId)
        {
            try
            {
                string query = @"
                    UPDATE department 
                    SET IsActive = 0, EditedBy = @EditedBy, EditedOn = @EditedOn
                    WHERE RecordNo = @RecordNo";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RecordNo", recordNo);
                    command.Parameters.AddWithValue("@EditedBy", currentUserId);
                    command.Parameters.AddWithValue("@EditedOn", DateTime.Now);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting department: " + ex.Message);
            }
        }

        /// <summary>
        /// Search departments by column
        /// </summary>
        public List<DepartmentGridViewModel> SearchDepartments(string columnName, string searchValue, int pageNumber, int pageSize)
        {
            try
            {
                string allowedColumns = "DepartmentCode,DepartmentName,JapanHead,Office,GotSection,Prefix,CreatedBy,EditedBy";
                if (!allowedColumns.Contains(columnName))
                {
                    throw new Exception("Invalid column name");
                }

                string query = $@"
                    SELECT RecordNo, DepartmentCode, DepartmentName, JapanHead, Office, 
                           GotSection, Prefix, CreatedBy, CreatedOn, EditedBy, EditedOn, 
                           ISNULL(IsActive, 1) as IsActive
                    FROM department
                    WHERE {columnName} LIKE '%' + @searchValue + '%'
                    ORDER BY RecordNo ASC
                    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@searchValue", searchValue);
                    command.Parameters.AddWithValue("@offset", (pageNumber - 1) * pageSize);
                    command.Parameters.AddWithValue("@pageSize", pageSize);

                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    var departments = new List<DepartmentGridViewModel>();
                    foreach (DataRow row in dt.Rows)
                    {
                        departments.Add(new DepartmentGridViewModel
                        {
                            RecordNo = Convert.ToInt32(row["RecordNo"]),
                            DepartmentCode = row["DepartmentCode"].ToString(),
                            DepartmentName = row["DepartmentName"].ToString(),
                            JapanHead = row["JapanHead"].ToString(),
                            Office = row["Office"].ToString(),
                            GotSection = row["GotSection"].ToString(),
                            Prefix = row["Prefix"].ToString(),
                            CreatedBy = row["CreatedBy"].ToString(),
                            CreatedOn = row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]) : (DateTime?)null,
                            EditedBy = row["EditedBy"].ToString(),
                            EditedOn = row["EditedOn"] != DBNull.Value ? Convert.ToDateTime(row["EditedOn"]) : (DateTime?)null,
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        });
                    }

                    return departments;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching departments: " + ex.Message);
            }
        }

        /// <summary>
        /// Export all departments to DataTable
        /// </summary>
        public DataTable ExportDepartments()
        {
            try
            {
                string query = @"
                    SELECT 
                        RecordNo as 'Record No',
                        DepartmentCode as 'Department Code',
                        DepartmentName as 'Department Name',
                        JapanHead as 'Japan Head',
                        Office as 'Office',
                        GotSection as 'Got Section',
                        Prefix as 'Prefix',
                        CreatedBy as 'Created By',
                        CreatedOn as 'Created On',
                        EditedBy as 'Edited By',
                        EditedOn as 'Edited On',
                        CASE WHEN ISNULL(IsActive, 1) = 1 THEN 'Active' ELSE 'Inactive' END as 'Status'
                    FROM department
                    ORDER BY RecordNo ASC";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error exporting departments: " + ex.Message);
            }
        }
    }
}
