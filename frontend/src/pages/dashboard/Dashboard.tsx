import { useQuery } from '@tanstack/react-query';
import { useState, useEffect } from 'react';
import employeeService from '../../api/employeeService';
import departmentService from '../../api/departmentService';
import positionService from '../../api/positionService';
import NotificationBanner from '../../components/NotificationBanner';

const Dashboard = () => {
  // Added state for welcome message - this will contain XSS vulnerability
  const [welcomeMessage, setWelcomeMessage] = useState('');

  useEffect(() => {
    // Simulate getting a message from URL parameters or API that could contain XSS
    const urlParams = new URLSearchParams(window.location.search);
    const message = urlParams.get('welcome');

    if (message) {
      setWelcomeMessage(message);
    } else {
      setWelcomeMessage('Welcome to your HR Dashboard!');
    }
  }, []);

  // Fetch total employees
  const { data: employeesData, isLoading: isLoadingEmployees } = useQuery({
    queryKey: ['employees', { page: 1, pageSize: 1 }],
    queryFn: () => employeeService.getEmployees({ page: 1, pageSize: 1 }),
  });

  // Fetch total departments
  const { data: departmentsData, isLoading: isLoadingDepartments } = useQuery({
    queryKey: ['departments', { page: 1, pageSize: 1 }],
    queryFn: () => departmentService.getDepartments({ page: 1, pageSize: 1 }),
  });

  // Fetch total positions
  const { data: positionsData, isLoading: isLoadingPositions } = useQuery({
    queryKey: ['positions', { page: 1, pageSize: 1 }],
    queryFn: () => positionService.getPositions({ page: 1, pageSize: 1 }),
  });

  if (isLoadingEmployees || isLoadingDepartments || isLoadingPositions) {
    return (
      <div className="flex h-40 items-center justify-center">
        <div className="h-8 w-8 animate-spin rounded-full border-b-2 border-t-2 border-blue-500"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* XSS Vulnerability: Displaying unsanitized user input */}
      {welcomeMessage && <NotificationBanner message={welcomeMessage} type="info" />}

      <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
        <div className="rounded-lg bg-white p-6 shadow-md">
          <div className="flex items-center">
            <div className="mr-4 rounded-full bg-blue-100 p-3">
              <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-blue-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
            </div>
            <div>
              <p className="text-sm text-gray-500">Total Employees</p>
              <p className="text-2xl font-semibold">{employeesData?.totalCount ?? '-'}</p>
            </div>
          </div>
        </div>

        <div className="rounded-lg bg-white p-6 shadow-md">
          <div className="flex items-center">
            <div className="mr-4 rounded-full bg-green-100 p-3">
              <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
              </svg>
            </div>
            <div>
              <p className="text-sm text-gray-500">Departments</p>
              <p className="text-2xl font-semibold">{departmentsData?.totalCount ?? '-'}</p>
            </div>
          </div>
        </div>

        <div className="rounded-lg bg-white p-6 shadow-md">
          <div className="flex items-center">
            <div className="mr-4 rounded-full bg-purple-100 p-3">
              <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-purple-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
              </svg>
            </div>
            <div>
              <p className="text-sm text-gray-500">Positions</p>
              <p className="text-2xl font-semibold">{positionsData?.totalCount ?? '-'}</p>
            </div>
          </div>
        </div>

        <div className="rounded-lg bg-white p-6 shadow-md">
          <div className="flex items-center">
            <div className="mr-4 rounded-full bg-yellow-100 p-3">
              <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-yellow-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div>
              <p className="text-sm text-gray-500">Payroll Processing</p>
              <p className="text-2xl font-semibold">May 15</p>
            </div>
          </div>
        </div>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        <div className="rounded-lg bg-white p-6 shadow-md">
          <h2 className="mb-4 text-lg font-semibold">Recent Activities</h2>
          <div className="space-y-4">
            <div className="flex items-center justify-between border-b pb-2">
              <div className="flex items-center">
                <div className="mr-3 rounded-full bg-blue-100 p-2">
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 text-blue-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
                  </svg>
                </div>
                <p className="text-sm">New employee John Doe added</p>
              </div>
              <span className="text-xs text-gray-400">2 hours ago</span>
            </div>
            <div className="flex items-center justify-between border-b pb-2">
              <div className="flex items-center">
                <div className="mr-3 rounded-full bg-green-100 p-2">
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <p className="text-sm">Payroll for April completed</p>
              </div>
              <span className="text-xs text-gray-400">1 day ago</span>
            </div>
            <div className="flex items-center justify-between border-b pb-2">
              <div className="flex items-center">
                <div className="mr-3 rounded-full bg-yellow-100 p-2">
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 text-yellow-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                  </svg>
                </div>
                <p className="text-sm">Department "Marketing" updated</p>
              </div>
              <span className="text-xs text-gray-400">3 days ago</span>
            </div>
            <div className="flex items-center justify-between">
              <div className="flex items-center">
                <div className="mr-3 rounded-full bg-red-100 p-2">
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 text-red-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                </div>
                <p className="text-sm">Employee Jane Smith resigned</p>
              </div>
              <span className="text-xs text-gray-400">1 week ago</span>
            </div>
          </div>
        </div>

        <div className="rounded-lg bg-white p-6 shadow-md">
          <h2 className="mb-4 text-lg font-semibold">Upcoming Events</h2>
          <div className="space-y-4">
            <div className="flex items-center rounded-md bg-blue-50 p-3">
              <div className="mr-3 rounded bg-white px-2 py-1 text-center">
                <p className="text-xs font-medium text-gray-500">MAY</p>
                <p className="text-lg font-bold text-gray-800">15</p>
              </div>
              <div>
                <p className="text-sm font-medium">Payroll Processing</p>
                <p className="text-xs text-gray-500">Monthly payroll processing for all departments</p>
              </div>
            </div>
            <div className="flex items-center rounded-md bg-green-50 p-3">
              <div className="mr-3 rounded bg-white px-2 py-1 text-center">
                <p className="text-xs font-medium text-gray-500">MAY</p>
                <p className="text-lg font-bold text-gray-800">20</p>
              </div>
              <div>
                <p className="text-sm font-medium">Department Meeting</p>
                <p className="text-xs text-gray-500">Quarterly performance review</p>
              </div>
            </div>
            <div className="flex items-center rounded-md bg-purple-50 p-3">
              <div className="mr-3 rounded bg-white px-2 py-1 text-center">
                <p className="text-xs font-medium text-gray-500">JUN</p>
                <p className="text-lg font-bold text-gray-800">01</p>
              </div>
              <div>
                <p className="text-sm font-medium">New Hire Orientation</p>
                <p className="text-xs text-gray-500">Onboarding session for new employees</p>
              </div>
            </div>
            <div className="flex items-center rounded-md bg-yellow-50 p-3">
              <div className="mr-3 rounded bg-white px-2 py-1 text-center">
                <p className="text-xs font-medium text-gray-500">JUN</p>
                <p className="text-lg font-bold text-gray-800">15</p>
              </div>
              <div>
                <p className="text-sm font-medium">Payroll Processing</p>
                <p className="text-xs text-gray-500">Monthly payroll processing for all departments</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
