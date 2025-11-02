import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import employeeService, { EmployeeCreateRequest } from '../../api/employeeService';
import departmentService from '../../api/departmentService';
import positionService from '../../api/positionService';

// Define schema with zod
const employeeSchema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  email: z.string().email('Invalid email format'),
  phone: z.string().optional(),
  address: z.string().optional(),
  dateOfBirth: z.string().optional(),
  hireDate: z.string().min(1, 'Hire date is required'),
  status: z.enum(['Active', 'OnLeave', 'Terminated']),
  departmentId: z.string().min(1, 'Department is required'),
  positionId: z.string().min(1, 'Position is required'),
  managerId: z.string().optional(),
  emergencyContact: z.string().optional(),
  bankDetails: z.string().optional(),
});

type EmployeeFormData = z.infer<typeof employeeSchema>;

const EmployeeCreate = () => {
  const navigate = useNavigate();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<EmployeeFormData>({
    resolver: zodResolver(employeeSchema),
    defaultValues: {
      status: 'Active',
      hireDate: new Date().toISOString().split('T')[0], // Today's date as default
    },
  });

  const onSubmit: SubmitHandler<EmployeeFormData> = async (data) => {
    try {
      setIsSubmitting(true);

      const employee: EmployeeCreateRequest = {
        ...data,
        dateOfBirth: data.dateOfBirth ? new Date(data.dateOfBirth) : undefined,
        hireDate: new Date(data.hireDate),
      };

      // In a real app, this would call the API
      // await employeeService.createEmployee(employee);

      // Simulating API call
      setTimeout(() => {
        navigate('/employees');
      }, 500);

    } catch (error) {
      console.error('Error creating employee:', error);
    } finally {
      setIsSubmitting(false);
    }
  };


  // Fetch departments
  const {
    data: departmentsData,
    isLoading: isLoadingDepartments,
    isError: isErrorDepartments,
  } = useQuery({
    queryKey: ['departments', { page: 1, pageSize: 1000 }],
    queryFn: () => departmentService.getDepartments({ page: 1, pageSize: 1000 }),
  });

  // Fetch positions
  const {
    data: positionsData,
    isLoading: isLoadingPositions,
    isError: isErrorPositions,
  } = useQuery({
    queryKey: ['positions', { page: 1, pageSize: 1000 }],
    queryFn: () => positionService.getPositions({ page: 1, pageSize: 1000 }),
  });

  // Fetch managers (active employees)
  const {
    data: managersData,
    isLoading: isLoadingManagers,
    isError: isErrorManagers,
  } = useQuery({
    queryKey: ['employees', { status: 'Active', page: 1, pageSize: 1000 }],
    queryFn: () => employeeService.getEmployees({ status: 'Active', page: 1, pageSize: 1000 }),
  });

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-gray-900">Add New Employee</h1>
      </div>

      <div className="rounded-lg bg-white p-8 shadow">
        <form onSubmit={(e) => {
          handleSubmit(onSubmit)(e);
        }} className="space-y-6">
          <div className="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-2">
            <div>
              <label htmlFor="firstName" className="block text-sm font-medium text-gray-700">
                First Name *
              </label>
              <input
                type="text"
                id="firstName"
                {...register('firstName')}
                className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm ${
                  errors.firstName ? 'border-red-300' : ''
                }`}
              />
              {errors.firstName && (
                <p className="mt-1 text-xs text-red-600">{errors.firstName.message}</p>
              )}
            </div>

            <div>
              <label htmlFor="lastName" className="block text-sm font-medium text-gray-700">
                Last Name *
              </label>
              <input
                type="text"
                id="lastName"
                {...register('lastName')}
                className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm ${
                  errors.lastName ? 'border-red-300' : ''
                }`}
              />
              {errors.lastName && (
                <p className="mt-1 text-xs text-red-600">{errors.lastName.message}</p>
              )}
            </div>

            <div>
              <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                Email *
              </label>
              <input
                type="email"
                id="email"
                {...register('email')}
                className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm ${
                  errors.email ? 'border-red-300' : ''
                }`}
              />
              {errors.email && (
                <p className="mt-1 text-xs text-red-600">{errors.email.message}</p>
              )}
            </div>

            <div>
              <label htmlFor="phone" className="block text-sm font-medium text-gray-700">
                Phone
              </label>
              <input
                type="text"
                id="phone"
                {...register('phone')}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
              />
            </div>

            <div>
              <label htmlFor="dateOfBirth" className="block text-sm font-medium text-gray-700">
                Date of Birth
              </label>
              <input
                type="date"
                id="dateOfBirth"
                {...register('dateOfBirth')}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
              />
            </div>

            <div>
              <label htmlFor="hireDate" className="block text-sm font-medium text-gray-700">
                Hire Date *
              </label>
              <input
                type="date"
                id="hireDate"
                {...register('hireDate')}
                className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm ${
                  errors.hireDate ? 'border-red-300' : ''
                }`}
              />
              {errors.hireDate && (
                <p className="mt-1 text-xs text-red-600">{errors.hireDate.message}</p>
              )}
            </div>

            <div>
              <label htmlFor="status" className="block text-sm font-medium text-gray-700">
                Status *
              </label>
              <select
                id="status"
                {...register('status')}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
              >
                <option value="Active">Active</option>
                <option value="OnLeave">On Leave</option>
                <option value="Terminated">Terminated</option>
              </select>
            </div>

            <div>
              <label htmlFor="departmentId" className="block text-sm font-medium text-gray-700">
                Department *
              </label>
              <select
                id="departmentId"
                {...register('departmentId')}
                className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm ${
                  errors.departmentId ? 'border-red-300' : ''
                }`}
              >
                <option value="">Select Department</option>
                {isLoadingDepartments ? (
                  <option disabled>Loading...</option>
                ) : isErrorDepartments ? (
                  <option disabled>Error loading departments</option>
                ) : (
                  departmentsData?.items?.map((dept: any) => (
                    <option key={dept.departmentId} value={dept.departmentId}>
                      {dept.name}
                    </option>
                  ))
                )}
              </select>
              {errors.departmentId && (
                <p className="mt-1 text-xs text-red-600">{errors.departmentId.message}</p>
              )}
            </div>

            <div>
              <label htmlFor="positionId" className="block text-sm font-medium text-gray-700">
                Position *
              </label>
              <select
                id="positionId"
                {...register('positionId')}
                className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm ${
                  errors.positionId ? 'border-red-300' : ''
                }`}
              >
                <option value="">Select Position</option>
                {isLoadingPositions ? (
                  <option disabled>Loading...</option>
                ) : isErrorPositions ? (
                  <option disabled>Error loading positions</option>
                ) : (
                  positionsData?.items?.map((pos: any) => (
                    <option key={pos.positionId} value={pos.positionId}>
                      {pos.title}
                    </option>
                  ))
                )}
              </select>
              {errors.positionId && (
                <p className="mt-1 text-xs text-red-600">{errors.positionId.message}</p>
              )}
            </div>

            <div>
              <label htmlFor="managerId" className="block text-sm font-medium text-gray-700">
                Manager
              </label>
              <select
                id="managerId"
                {...register('managerId')}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
              >
                <option value="">Select Manager</option>
                {isLoadingManagers ? (
                  <option disabled>Loading...</option>
                ) : isErrorManagers ? (
                  <option disabled>Error loading managers</option>
                ) : (
                  managersData?.items?.map((manager: any) => (
                    <option key={manager.employeeId} value={manager.employeeId}>
                      {manager.firstName} {manager.lastName}
                    </option>
                  ))
                )}
              </select>
            </div>

            <div className="sm:col-span-2">
              <label htmlFor="address" className="block text-sm font-medium text-gray-700">
                Address
              </label>
              <textarea
                id="address"
                rows={3}
                {...register('address')}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
              ></textarea>
            </div>

            <div className="sm:col-span-2">
              <label htmlFor="emergencyContact" className="block text-sm font-medium text-gray-700">
                Emergency Contact
              </label>
              <input
                type="text"
                id="emergencyContact"
                {...register('emergencyContact')}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                placeholder="Name, Phone Number"
              />
            </div>

            <div className="sm:col-span-2">
              <label htmlFor="bankDetails" className="block text-sm font-medium text-gray-700">
                Bank Details
              </label>
              <input
                type="text"
                id="bankDetails"
                {...register('bankDetails')}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                placeholder="Account Number, Bank Name"
              />
            </div>
          </div>

          <div className="flex justify-end space-x-3 pt-5">
            <button
              type="button"
              onClick={() => navigate('/employees')}
              className="rounded-md border border-gray-300 bg-white py-2 px-4 text-sm font-medium text-gray-700 shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="inline-flex justify-center rounded-md border border-transparent bg-blue-600 py-2 px-4 text-sm font-medium text-white shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-75"
            >
              {isSubmitting ? 'Saving...' : 'Save'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EmployeeCreate;
