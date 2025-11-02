
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';

import payrollService, { PayPeriod } from '../../api/payrollService';
import employeeService from '../../api/employeeService';
import departmentService from '../../api/departmentService';
import positionService from '../../api/positionService';


const PayrollDashboard = () => {
  // Fetch pay periods (assume most recent is current)
  const { data: payPeriods, isLoading: isLoadingPayPeriods, isError: isErrorPayPeriods } = useQuery({
    queryKey: ['payPeriods'],
    queryFn: () => payrollService.getPayPeriods(),
  });

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

  // Get the most recent pay period as current
  const currentPeriod: PayPeriod | undefined = payPeriods?.Items?.[0];

  const formatCurrency = (amount?: number) => {
    if (typeof amount !== 'number') return '-';
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      maximumFractionDigits: 0
    }).format(amount);
  };

  const formatDate = (date?: Date | string) => {
    if (!date) return '-';
    const d = typeof date === 'string' ? new Date(date) : date;
    return new Intl.DateTimeFormat('en-US', { month: 'short', day: 'numeric' }).format(d);
  };

  if (isLoadingPayPeriods || isLoadingEmployees || isLoadingDepartments || isLoadingPositions) {
    return (
      <div className="flex h-40 items-center justify-center">
        <div className="h-8 w-8 animate-spin rounded-full border-b-2 border-t-2 border-blue-500"></div>
      </div>
    );
  }

  if (isErrorPayPeriods || !currentPeriod) {
    return (
      <div className="rounded-md bg-red-50 p-4">
        <div className="flex">
          <div className="flex-shrink-0">
            <svg className="h-5 w-5 text-red-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
              <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
            </svg>
          </div>
          <div className="ml-3">
            <p className="text-sm font-medium text-red-800">Error loading current pay period. Please try again later.</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-gray-900">Payroll Dashboard</h1>
        <div className="flex space-x-4">
          <Link
            to="/payroll/payslips"
            className="rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
          >
            View All Payslips
          </Link>
          <Link
            to="/payroll/periods"
            className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
          >
            Manage Pay Periods
          </Link>
        </div>
      </div>

      {/* Current Pay Period */}
      <div className="overflow-hidden rounded-lg bg-white shadow">
        <div className="px-4 py-5 sm:p-6">
          <h2 className="text-lg font-medium leading-6 text-gray-900">Current Pay Period</h2>
          <div className="mt-4 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
            <div>
              <p className="text-sm font-medium text-gray-500">Period</p>
              <p className="mt-1 text-lg font-semibold text-gray-900">
                {formatDate(currentPeriod.startDate)} - {formatDate(currentPeriod.endDate)}
              </p>
            </div>
            <div>
              <p className="text-sm font-medium text-gray-500">Payment Date</p>
              <p className="mt-1 text-lg font-semibold text-gray-900">
                {formatDate(currentPeriod.paymentDate)}
              </p>
            </div>
            <div>
              <p className="text-sm font-medium text-gray-500">Status</p>
              <p className="mt-1">
                <span
                  className={`inline-flex rounded-full px-2.5 py-0.5 text-sm font-semibold ${
                    currentPeriod.status === 'Draft'
                      ? 'bg-gray-100 text-gray-800'
                      : currentPeriod.status === 'Processing'
                        ? 'bg-yellow-100 text-yellow-800'
                        : currentPeriod.status === 'Completed'
                          ? 'bg-green-100 text-green-800'
                          : 'bg-red-100 text-red-800'
                  }`}
                >
                  {currentPeriod.status}
                </span>
              </p>
            </div>
            <div>
              <p className="text-sm font-medium text-gray-500">Total Amount</p>
              <p className="mt-1 text-lg font-semibold text-gray-900">
                {formatCurrency(currentPeriod.totalAmount)}
              </p>
            </div>
          </div>
          <div className="mt-6 flex justify-end">
            <button
              type="button"
              className="inline-flex items-center rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
            >
              Process Payroll
            </button>
          </div>
        </div>
      </div>


      {/* Dashboard Cards */}
      <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
        {/* Employees/Departments/Positions Card */}
        <div className="overflow-hidden rounded-lg bg-white shadow">
          <div className="px-4 py-5 sm:p-6">
            <h3 className="text-base font-medium text-gray-900">Organization Overview</h3>
            <dl className="mt-4 grid grid-cols-1 gap-x-4 gap-y-6 sm:grid-cols-2">
              <div className="sm:col-span-1">
                <dt className="text-sm font-medium text-gray-500">Total Employees</dt>
                <dd className="mt-1 text-3xl font-semibold text-gray-900">{employeesData?.totalCount ?? '-'}</dd>
              </div>
              <div className="sm:col-span-1">
                <dt className="text-sm font-medium text-gray-500">Departments</dt>
                <dd className="mt-1 text-3xl font-semibold text-gray-900">{departmentsData?.totalCount ?? '-'}</dd>
              </div>
              <div className="sm:col-span-1">
                <dt className="text-sm font-medium text-gray-500">Positions</dt>
                <dd className="mt-1 text-3xl font-semibold text-gray-900">{positionsData?.totalCount ?? '-'}</dd>
              </div>
            </dl>
          </div>
        </div>

        {/* Payroll Summary Card */}
        <div className="overflow-hidden rounded-lg bg-white shadow">
          <div className="px-4 py-5 sm:p-6">
            <h3 className="text-base font-medium text-gray-900">Payroll Summary</h3>
            <dl className="mt-4 grid grid-cols-1 gap-x-4 gap-y-6 sm:grid-cols-2">
              <div className="sm:col-span-1">
                <dt className="text-sm font-medium text-gray-500">Total Employees (This Period)</dt>
                <dd className="mt-1 text-3xl font-semibold text-gray-900">{currentPeriod.employeeCount ?? '-'}</dd>
              </div>
              <div className="sm:col-span-1">
                <dt className="text-sm font-medium text-gray-500">Active Employees (Est.)</dt>
                <dd className="mt-1 text-3xl font-semibold text-gray-900">{typeof currentPeriod.employeeCount === 'number' ? currentPeriod.employeeCount - 3 : '-'}</dd>
              </div>
              <div className="sm:col-span-1">
                <dt className="text-sm font-medium text-gray-500">On Leave (Est.)</dt>
                <dd className="mt-1 text-3xl font-semibold text-gray-900">3</dd>
              </div>
              <div className="sm:col-span-1">
                <dt className="text-sm font-medium text-gray-500">Average Salary</dt>
                <dd className="mt-1 text-3xl font-semibold text-gray-900">
                  {typeof currentPeriod.totalAmount === 'number' && typeof currentPeriod.employeeCount === 'number' && currentPeriod.employeeCount > 0
                    ? formatCurrency(currentPeriod.totalAmount / currentPeriod.employeeCount)
                    : '-'}
                </dd>
              </div>
            </dl>
          </div>
        </div>

        {/* Quick Actions Card */}
        <div className="overflow-hidden rounded-lg bg-white shadow">
          <div className="px-4 py-5 sm:p-6">
            <h3 className="text-base font-medium text-gray-900">Quick Actions</h3>
            <div className="mt-4 space-y-4">
              <button
                type="button"
                className="block w-full rounded-md bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
              >
                Generate Reports
              </button>
              <button
                type="button"
                className="block w-full rounded-md bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
              >
                Create New Pay Period
              </button>
              <button
                type="button"
                className="block w-full rounded-md bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
              >
                Manage Deductions
              </button>
              <button
                type="button"
                className="block w-full rounded-md bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
              >
                Tax Settings
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Recent Activity */}
      <div className="overflow-hidden rounded-lg bg-white shadow">
        <div className="px-4 py-5 sm:p-6">
          <h3 className="text-base font-medium text-gray-900">Recent Activity</h3>
          <div className="mt-4 text-gray-500 text-sm">No recent activity to display.</div>
          <div className="mt-6 text-center">
            <Link to="/payroll/activity" className="text-sm font-medium text-blue-600 hover:text-blue-500">
              View all activity
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PayrollDashboard;
