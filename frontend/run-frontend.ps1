Write-Host "Starting HRMS Frontend with Vite..." -ForegroundColor Green

# Create a simple HTML file that will load our app
$htmlContent = @"
<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>HRMS - Human Resource Management System</title>
    <style>
      /* Tailwind-like styles */
      body {
        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif;
        background-color: #f3f4f6;
        margin: 0;
        padding: 0;
      }
      .container {
        max-width: 1200px;
        margin: 0 auto;
        padding: 1rem;
      }
      .header {
        background-color: #2563eb;
        color: white;
        padding: 1rem 0;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      }
      .header-content {
        display: flex;
        justify-content: space-between;
        align-items: center;
      }
      .nav {
        display: flex;
        gap: 1rem;
      }
      .nav a {
        color: white;
        text-decoration: none;
      }
      .nav a:hover {
        text-decoration: underline;
      }
      .main {
        padding: 2rem 0;
      }
      .card {
        background-color: white;
        border-radius: 0.5rem;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        padding: 1.5rem;
        margin-bottom: 1.5rem;
      }
      .title {
        font-size: 1.5rem;
        font-weight: bold;
        margin-bottom: 1rem;
      }
      .table {
        width: 100%;
        border-collapse: collapse;
      }
      .table th, .table td {
        padding: 0.75rem;
        text-align: left;
        border-bottom: 1px solid #e5e7eb;
      }
      .table th {
        background-color: #f9fafb;
      }
      .button {
        display: inline-block;
        background-color: #2563eb;
        color: white;
        padding: 0.5rem 1rem;
        border-radius: 0.25rem;
        text-decoration: none;
        cursor: pointer;
      }
      .button-secondary {
        background-color: #e5e7eb;
        color: #1f2937;
      }
      .form-group {
        margin-bottom: 1rem;
      }
      .form-label {
        display: block;
        margin-bottom: 0.5rem;
        font-weight: 500;
      }
      .form-input {
        width: 100%;
        padding: 0.5rem;
        border: 1px solid #d1d5db;
        border-radius: 0.25rem;
      }
    </style>
  </head>
  <body>
    <div id="app">
      <header class="header">
        <div class="container">
          <div class="header-content">
            <h1>HRMS</h1>
            <nav class="nav">
              <a href="#" id="dashboard-link">Dashboard</a>
              <a href="#" id="add-employee-link">Add Employee</a>
              <a href="#" id="request-leave-link">Request Leave</a>
            </nav>
          </div>
        </div>
      </header>
      
      <main class="container main">
        <div id="dashboard-page">
          <h2 class="title">Employee Dashboard</h2>
          <div class="card">
            <h3 class="title">Employees</h3>
            <div id="employees-table-container">
              <p>Loading employees...</p>
            </div>
          </div>
          
          <div class="card">
            <h3 class="title">Recent Leave Requests</h3>
            <div id="leave-requests-table-container">
              <p>Loading leave requests...</p>
            </div>
          </div>
        </div>
        
        <div id="add-employee-page" style="display: none;">
          <h2 class="title">Add New Employee</h2>
          <div class="card">
            <form id="add-employee-form">
              <div class="form-group">
                <label class="form-label" for="name">Employee Name</label>
                <input type="text" id="name" class="form-input" placeholder="Enter employee name" required>
              </div>
              
              <div class="form-group">
                <label class="form-label" for="department">Department</label>
                <input type="text" id="department" class="form-input" placeholder="Enter department" required>
              </div>
              
              <div style="display: flex; justify-content: space-between;">
                <button type="button" class="button button-secondary" id="cancel-add-employee">Cancel</button>
                <button type="submit" class="button">Add Employee</button>
              </div>
            </form>
          </div>
        </div>
        
        <div id="request-leave-page" style="display: none;">
          <h2 class="title">Request Leave</h2>
          <div class="card">
            <form id="request-leave-form">
              <div class="form-group">
                <label class="form-label" for="employee-id">Employee</label>
                <select id="employee-id" class="form-input" required>
                  <option value="">Select an employee</option>
                </select>
              </div>
              
              <div class="form-group">
                <label class="form-label" for="leave-date">Leave Date</label>
                <input type="date" id="leave-date" class="form-input" required>
              </div>
              
              <div class="form-group">
                <label class="form-label" for="reason">Reason</label>
                <textarea id="reason" class="form-input" rows="3" placeholder="Enter reason for leave" required></textarea>
              </div>
              
              <div style="display: flex; justify-content: space-between;">
                <button type="button" class="button button-secondary" id="cancel-request-leave">Cancel</button>
                <button type="submit" class="button">Submit Request</button>
              </div>
            </form>
          </div>
        </div>
      </main>
    </div>
    
    <script>
      // API URL
      const API_URL = 'http://localhost:5170/api';
      
      // DOM Elements
      const dashboardPage = document.getElementById('dashboard-page');
      const addEmployeePage = document.getElementById('add-employee-page');
      const requestLeavePage = document.getElementById('request-leave-page');
      
      const dashboardLink = document.getElementById('dashboard-link');
      const addEmployeeLink = document.getElementById('add-employee-link');
      const requestLeaveLink = document.getElementById('request-leave-link');
      
      const employeesTableContainer = document.getElementById('employees-table-container');
      const leaveRequestsTableContainer = document.getElementById('leave-requests-table-container');
      
      const addEmployeeForm = document.getElementById('add-employee-form');
      const cancelAddEmployee = document.getElementById('cancel-add-employee');
      
      const requestLeaveForm = document.getElementById('request-leave-form');
      const cancelRequestLeave = document.getElementById('cancel-request-leave');
      const employeeIdSelect = document.getElementById('employee-id');
      
      // Navigation
      function showPage(page) {
        dashboardPage.style.display = 'none';
        addEmployeePage.style.display = 'none';
        requestLeavePage.style.display = 'none';
        
        page.style.display = 'block';
      }
      
      dashboardLink.addEventListener('click', (e) => {
        e.preventDefault();
        showPage(dashboardPage);
        fetchEmployees();
        fetchLeaveRequests();
      });
      
      addEmployeeLink.addEventListener('click', (e) => {
        e.preventDefault();
        showPage(addEmployeePage);
      });
      
      requestLeaveLink.addEventListener('click', (e) => {
        e.preventDefault();
        showPage(requestLeavePage);
        populateEmployeeSelect();
      });
      
      cancelAddEmployee.addEventListener('click', () => {
        showPage(dashboardPage);
      });
      
      cancelRequestLeave.addEventListener('click', () => {
        showPage(dashboardPage);
      });
      
      // API Functions
      async function fetchEmployees() {
        try {
          const response = await fetch(`${API_URL}/employees`);
          const employees = await response.json();
          
          if (employees.length === 0) {
            employeesTableContainer.innerHTML = '<p>No employees found. Add some employees to get started.</p>';
          } else {
            let tableHtml = `
              <table class="table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Name</th>
                    <th>Department</th>
                  </tr>
                </thead>
                <tbody>
            `;
            
            employees.forEach(employee => {
              tableHtml += `
                <tr>
                  <td>${employee.id}</td>
                  <td>${employee.name}</td>
                  <td>${employee.department}</td>
                </tr>
              `;
            });
            
            tableHtml += `
                </tbody>
              </table>
            `;
            
            employeesTableContainer.innerHTML = tableHtml;
          }
        } catch (error) {
          console.error('Error fetching employees:', error);
          employeesTableContainer.innerHTML = '<p class="error">Failed to load employees. Please try again later.</p>';
        }
      }
      
      async function fetchLeaveRequests() {
        try {
          const response = await fetch(`${API_URL}/leaveRequests`);
          const leaveRequests = await response.json();
          
          if (leaveRequests.length === 0) {
            leaveRequestsTableContainer.innerHTML = '<p>No leave requests found.</p>';
          } else {
            let tableHtml = `
              <table class="table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Employee ID</th>
                    <th>Date</th>
                    <th>Reason</th>
                  </tr>
                </thead>
                <tbody>
            `;
            
            leaveRequests.forEach(request => {
              tableHtml += `
                <tr>
                  <td>${request.id}</td>
                  <td>${request.employeeId}</td>
                  <td>${new Date(request.date).toLocaleDateString()}</td>
                  <td>${request.reason}</td>
                </tr>
              `;
            });
            
            tableHtml += `
                </tbody>
              </table>
            `;
            
            leaveRequestsTableContainer.innerHTML = tableHtml;
          }
        } catch (error) {
          console.error('Error fetching leave requests:', error);
          leaveRequestsTableContainer.innerHTML = '<p class="error">Failed to load leave requests. Please try again later.</p>';
        }
      }
      
      async function populateEmployeeSelect() {
        try {
          const response = await fetch(`${API_URL}/employees`);
          const employees = await response.json();
          
          // Clear existing options except the first one
          while (employeeIdSelect.options.length > 1) {
            employeeIdSelect.remove(1);
          }
          
          employees.forEach(employee => {
            const option = document.createElement('option');
            option.value = employee.id;
            option.textContent = `${employee.name} - ${employee.department}`;
            employeeIdSelect.appendChild(option);
          });
        } catch (error) {
          console.error('Error fetching employees for select:', error);
        }
      }
      
      // Form Submissions
      addEmployeeForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const nameInput = document.getElementById('name');
        const departmentInput = document.getElementById('department');
        
        const employee = {
          name: nameInput.value,
          department: departmentInput.value
        };
        
        try {
          const response = await fetch(`${API_URL}/employees`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify(employee)
          });
          
          if (response.ok) {
            // Reset form
            addEmployeeForm.reset();
            
            // Show dashboard and refresh data
            showPage(dashboardPage);
            fetchEmployees();
            fetchLeaveRequests();
            
            alert('Employee added successfully!');
          } else {
            alert('Failed to add employee. Please try again.');
          }
        } catch (error) {
          console.error('Error adding employee:', error);
          alert('Failed to add employee. Please try again.');
        }
      });
      
      requestLeaveForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const employeeIdInput = document.getElementById('employee-id');
        const leaveDateInput = document.getElementById('leave-date');
        const reasonInput = document.getElementById('reason');
        
        const leaveRequest = {
          employeeId: parseInt(employeeIdInput.value),
          date: new Date(leaveDateInput.value).toISOString(),
          reason: reasonInput.value
        };
        
        try {
          const response = await fetch(`${API_URL}/leaveRequests`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify(leaveRequest)
          });
          
          if (response.ok) {
            // Reset form
            requestLeaveForm.reset();
            
            // Show dashboard and refresh data
            showPage(dashboardPage);
            fetchEmployees();
            fetchLeaveRequests();
            
            alert('Leave request submitted successfully!');
          } else {
            alert('Failed to submit leave request. Please try again.');
          }
        } catch (error) {
          console.error('Error submitting leave request:', error);
          alert('Failed to submit leave request. Please try again.');
        }
      });
      
      // Initialize
      fetchEmployees();
      fetchLeaveRequests();
    </script>
  </body>
</html>
"@

# Write the HTML file
$htmlFile = Join-Path $PSScriptRoot "hrms-app.html"
$htmlContent | Out-File -FilePath $htmlFile -Encoding utf8

# Open the HTML file in the default browser
Write-Host "Opening HRMS App in your default browser..." -ForegroundColor Green
Start-Process $htmlFile

Write-Host "HRMS Frontend is running!" -ForegroundColor Green
Write-Host "Press Ctrl+C to stop the server." -ForegroundColor Yellow

# Keep the script running
try {
    while ($true) {
        Start-Sleep -Seconds 1
    }
} finally {
    Write-Host "Shutting down HRMS Frontend..." -ForegroundColor Red
}
