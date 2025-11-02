import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import employeeService, { Employee } from '../../api/employeeService';
import SearchBar from '../../components/SearchBar';

const EmployeeList = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [filter, setFilter] = useState<{
    status: 'Active' | 'OnLeave' | 'Terminated' | undefined;
    page: number;
    pageSize: number;
    search?: string;
  }>({
    status: 'Active',
    page: 1,
    pageSize: 10,
  });
  const { data, isLoading, isError } = useQuery({
    queryKey: ['employees', filter, () => {}],
    queryFn: () => employeeService.getEmployees(filter),
  });
  // Search is now handled by the SearchBar component

  const handleStatusChange = (status: string) => {
    setFilter(prev => ({ ...prev, status: status as 'Active' | 'OnLeave' | 'Terminated', page: 1 }));
  };

  const handlePageChange = (newPage: number) => {
    setFilter(prev => ({ ...prev, page: newPage }));
  };



  return (
    <div className="space-y-6">
      <div className="flex justify-between">
        <h1 className="text-2xl font-semibold text-gray-900">Employees</h1>
        <Link
          to="/employees/new"
          className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
        >
          Add Employee
        </Link>
      </div>

      <div className="rounded-md bg-white p-6 shadow-md">
        <div className="mb-6 flex flex-wrap items-center justify-between gap-4">          <SearchBar
            onSearch={(query) => {
              setSearchTerm(query);
              setFilter(prev => ({ ...prev, search: query, page: 1 }));
            }}
            placeholder="Search employees..."
            initialValue={searchTerm}
          />

          <div className="flex space-x-2">
            <button
              className={`rounded-md px-4 py-2 text-sm font-medium ${
                filter.status === 'Active' ? 'bg-blue-100 text-blue-700' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
              onClick={() => handleStatusChange('Active')}
            >
              Active
            </button>
            <button
              className={`rounded-md px-4 py-2 text-sm font-medium ${
                filter.status === 'OnLeave' ? 'bg-yellow-100 text-yellow-700' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
              onClick={() => handleStatusChange('OnLeave')}
            >
              On Leave
            </button>
            <button
              className={`rounded-md px-4 py-2 text-sm font-medium ${
                filter.status === 'Terminated' ? 'bg-red-100 text-red-700' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
              onClick={() => handleStatusChange('Terminated')}
            >
              Terminated
            </button>
          </div>
        </div>

        {isLoading ? (
          <div className="flex h-40 items-center justify-center">
            <div className="h-8 w-8 animate-spin rounded-full border-b-2 border-t-2 border-blue-500"></div>
          </div>
        ) : isError ? (
          <div className="rounded-md bg-red-50 p-4">
            <div className="flex">
              <div className="flex-shrink-0">
                <svg className="h-5 w-5 text-red-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                </svg>
              </div>
              <div className="ml-3">
                <p className="text-sm font-medium text-red-800">Error loading employees. Please try again later.</p>
              </div>
            </div>
          </div>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-300">
                <thead className="bg-gray-50">
                  <tr>
                    <th scope="col" className="py-3.5 pl-4 pr-3 text-left text-sm font-semibold text-gray-900">
                      Name
                    </th>
                    <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                      Email
                    </th>
                    <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                      Phone
                    </th>
                    <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                      Hire Date
                    </th>
                    <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                      Status
                    </th>
                    <th scope="col" className="relative py-3.5 pl-3 pr-4">
                      <span className="sr-only">Actions</span>
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200 bg-white">
                  {data?.items && data.items.length > 0 ? (
                    data.items.map((employee: Employee) => (
                      <tr key={employee.employeeId}>
                        <td className="whitespace-nowrap py-4 pl-4 pr-3 text-sm">
                          <div className="font-medium text-gray-900">{`${employee.firstName} ${employee.lastName}`}</div>
                        </td>
                        <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{employee.email}</td>
                        <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{employee.phone || '-'}</td>
                        <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                          {employee.hireDate ? new Date(employee.hireDate).toLocaleDateString() : '-'}
                        </td>
                        <td className="whitespace-nowrap px-3 py-4 text-sm">
                          <span
                            className={`inline-flex rounded-full px-2 text-xs font-semibold leading-5 ${
                              employee.status === 'Active'
                                ? 'bg-green-100 text-green-800'
                                : employee.status === 'OnLeave'
                                ? 'bg-yellow-100 text-yellow-800'
                                : 'bg-red-100 text-red-800'
                            }`}
                          >
                            {employee.status}
                          </span>
                        </td>
                        <td className="whitespace-nowrap py-4 pl-3 pr-4 text-right text-sm font-medium">
                          <Link to={`/employees/${employee.employeeId}`} className="text-blue-600 hover:text-blue-900">
                            View
                          </Link>
                          <button className="ml-4 text-red-600 hover:text-red-900">Delete</button>
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td colSpan={6} className="py-6 text-center text-gray-500">
                        No employees found.
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>

            {data && (
              <div className="mt-5 flex items-center justify-between">
                <div className="text-sm text-gray-700">
                  Showing <span className="font-medium">{(data.pageNumber - 1) * data.pageSize + 1}</span>
                  {' '}to <span className="font-medium">{Math.min(data.pageNumber * data.pageSize, data.totalCount)}</span> of{' '}
                  <span className="font-medium">{data.totalCount}</span> results
                </div>
                <div className="flex space-x-2">
                  <button
                    onClick={() => handlePageChange(Math.max(1, data.pageNumber - 1))}
                    className="rounded-md border border-gray-300 bg-white px-3 py-1 text-sm font-medium text-gray-700 hover:bg-gray-50"
                    disabled={data.pageNumber === 1}
                  >
                    Previous
                  </button>
                  <button
                    onClick={() => handlePageChange(data.pageNumber + 1)}
                    className="rounded-md border border-gray-300 bg-white px-3 py-1 text-sm font-medium text-gray-700 hover:bg-gray-50"
                    disabled={data.pageNumber * data.pageSize >= data.totalCount}
                  >
                    Next
                  </button>
                </div>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
};

export default EmployeeList;
