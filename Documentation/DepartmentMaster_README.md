# Department Master MVC Implementation - Documentation

## Overview
This is a complete ASP.NET MVC implementation of a Department Master module for the E-Management application. It includes all CRUD operations, advanced searching, pagination, and export functionality.

## Features Implemented

### 1. Grid Display with Pagination
- Display records with customizable page size (5, 10, 25, 50, 100 records)
- Show total record count
- Display columns: Edit, Delete, Department Code, Department Name, Japan Head, Office, Got Section, Prefix, Created By, Created On, Edited By, Edited On, Active Status

### 2. Validation
- Prevent duplicate department code creation
- Display "Department Already Exists" validation message
- All fields are mandatory
- Client-side and server-side validation

### 3. Search Functionality
- Global search across all columns
- Column-level search for each column
- Search results update grid in real-time

### 4. Sorting
- Sort by any column
- Ascending and descending order

### 5. CRUD Operations
- Create new department
- Read/View department details
- Update existing department
- Delete department (soft delete - marked as inactive)

### 6. Export Functionality
- Export to Excel (.xlsx)
- Export to CSV (.csv)
- Export to PDF (.pdf)
- Print option

### 7. User Interface
- Display Employee Number and Name before Sign Out button
- Responsive design
- Modal form for Create/Edit operations
- After successful save, newly added record displays immediately

## Project Structure

```
E_Management/
├── Models/
│   └── DepartmentMaster.cs
├── Repositories/
│   └── DepartmentRepository.cs
├── Controllers/
│   └── DepartmentMasterController.cs
├── Views/
│   └── DepartmentMaster/
│       ├── Index.cshtml
│       ├── Create.cshtml
│       ├── Edit.cshtml
│       └── Print.cshtml
└── Database/
    └── DepartmentMaster_Schema.sql
```

## Installation Steps

### 1. Database Setup
- Execute the SQL script: `Database/DepartmentMaster_Schema.sql`
- This creates the `department` table with necessary columns and indexes

### 2. Add Required NuGet Packages
```
Install-Package ClosedXML
Install-Package iTextSharp
```

### 3. Update RouteConfig
Add the following route to `RouteConfig.cs`:
```csharp
routes.MapRoute(
    name: "DepartmentMaster",
    url: "{controller}/{action}/{id}",
    defaults: new { controller = "DepartmentMaster", action = "Index", id = UrlParameter.Optional }
);
```

### 4. Configure Connection String
Update `Web.config` with your database connection string:
```xml
<connectionStrings>
    <add name="DefaultConnection" connectionString="Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;" providerName="System.Data.SqlClient" />
</connectionStrings>
```

## Usage

### Access the Module
Navigate to: `/DepartmentMaster/Index`

### Create New Department
1. Click "+ Add New Department" button
2. Fill in all required fields
3. Click "Save" button
4. New record appears immediately in the grid

### Edit Department
1. Click "Edit" button in the grid
2. Update required fields
3. Click "Update Department" button

### Delete Department
1. Click "Delete" button in the grid
2. Confirm deletion in the popup
3. Record is marked as inactive

### Search
- **Global Search**: Use the search box at the top to search across all columns
- **Column Search**: Type in the search box under each column header for specific column search

### Export
- Click "Export to Excel" - Downloads Excel file
- Click "Export to CSV" - Downloads CSV file
- Click "Export to PDF" - Downloads PDF file
- Click "Print" - Opens print preview

## Database Schema

### Department Table
```sql
CREATE TABLE [dbo].[department]
(
    [RecordNo] [int] IDENTITY(1,1) PRIMARY KEY,
    [DepartmentCode] [varchar](50) NOT NULL UNIQUE,
    [DepartmentName] [varchar](100) NOT NULL,
    [JapanHead] [varchar](1) NOT NULL,  -- 'Y' or 'N'
    [Office] [varchar](50) NOT NULL,
    [GotSection] [varchar](1) NOT NULL, -- 'Y' or 'N'
    [Prefix] [varchar](50) NOT NULL,
    [ParentPositionCode] [varchar](100) NULL,
    [ParentDepartmentId] [int] NULL,
    [CreatedBy] [varchar](100) NULL,
    [CreatedOn] [datetime] NULL,
    [EditedBy] [varchar](100) NULL,
    [EditedOn] [datetime] NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [Remarks] [varchar](500) NULL
)
```

## API Endpoints

### GET /DepartmentMaster/Index
- Returns paginated list of departments
- Parameters: `page`, `pageSize`, `search`, `sortBy`, `sortDesc`

### POST /DepartmentMaster/Create
- Create new department
- Returns JSON response

### GET /DepartmentMaster/Edit/{id}
- Display edit form for department
- Returns view with department details

### POST /DepartmentMaster/Edit
- Update existing department
- Returns JSON response

### POST /DepartmentMaster/Delete
- Delete department (soft delete)
- Returns JSON response

### POST /DepartmentMaster/ColumnSearch
- Search by specific column
- Parameters: `columnName`, `searchValue`, `page`, `pageSize`
- Returns JSON with search results

### GET /DepartmentMaster/ExportToExcel
- Export all departments to Excel

### GET /DepartmentMaster/ExportToCSV
- Export all departments to CSV

### GET /DepartmentMaster/ExportToPDF
- Export all departments to PDF

### GET /DepartmentMaster/Print
- Display print preview

## Validation Rules

1. **Department Code**: 
   - Required
   - Maximum 50 characters
   - Must be unique

2. **Department Name**:
   - Required
   - Maximum 100 characters

3. **Japan Head**:
   - Required
   - Values: 'Y' or 'N'

4. **Office**:
   - Required
   - Maximum 50 characters

5. **Got Section**:
   - Required
   - Values: 'Y' or 'N'

6. **Prefix**:
   - Required
   - Maximum 50 characters

## Security Features

1. **Authentication**: Requires user login (Session["empcode"])
2. **Authorization**: [Authorize] attribute on controller
3. **CSRF Protection**: Anti-forgery tokens on all forms
4. **SQL Injection Prevention**: Parameterized queries
5. **Data Validation**: Both client and server-side

## Performance Considerations

1. **Pagination**: Limits database queries to required page size
2. **Indexing**: Indexes on frequently searched columns
3. **Connection Pooling**: Utilizes connection pooling
4. **Lazy Loading**: Only retrieves required data

## Troubleshooting

### Issue: "Department Already Exists" on Create
- Verify the DepartmentCode is unique
- Check database for existing records

### Issue: Export not working
- Ensure ClosedXML and iTextSharp packages are installed
- Check server has write permissions to temporary folder

### Issue: Session expires
- Check session timeout in Web.config
- Ensure authentication is configured

## Future Enhancements

1. Batch operations (delete multiple records)
2. Department hierarchy/parent-child relationships
3. Department usage reports
4. Integration with employee assignments
5. Audit trail for changes
6. Duplicate record detection

## Support

For issues or questions, contact the development team or refer to the ASP.NET MVC documentation.

## Version History

- **v1.0** - Initial implementation with complete CRUD, search, export, and pagination features

