import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';

// Define schema with zod
const positionSchema = z.object({
  title: z.string().min(1, 'Position title is required'),
  description: z.string().optional(),
  departmentId: z.string().min(1, 'Department is required'),
  minSalary: z.string().refine(val => !val || !isNaN(Number(val)), {
    message: 'Min salary must be a number'
  }).transform(val => val ? Number(val) : undefined),
  maxSalary: z.string().refine(val => !val || !isNaN(Number(val)), {
    message: 'Max salary must be a number'
  }).transform(val => val ? Number(val) : undefined),
});

type PositionFormData = z.infer<typeof positionSchema>;

const PositionCreate = () => {
  const navigate = useNavigate();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<PositionFormData>({
    resolver: zodResolver(positionSchema),
  });

  const onSubmit: SubmitHandler<PositionFormData> = async (data) => {
    try {
      setIsSubmitting(true);
      
      // In a real app, this would call the API
      // await positionService.createPosition(data);
      
      // Simulating API call
      setTimeout(() => {
        navigate('/positions');
      }, 500);
      
    } catch (error) {
      console.error('Error creating position:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  // Mock data for departments dropdown
  const departments = [
    { id: '1', name: 'Engineering' },
    { id: '2', name: 'Marketing' },
    { id: '3', name: 'Human Resources' },
    { id: '4', name: 'Finance' },
  ];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-gray-900">Add New Position</h1>
      </div>

      <div className="rounded-lg bg-white p-8 shadow">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div className="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-2">
            <div className="sm:col-span-2">
              <label htmlFor="title" className="block text-sm font-medium text-gray-700">
                Position Title *
              </label>
              <input
                type="text"
                id="title"
                {...register('title')}
                className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm ${
                  errors.title ? 'border-red-300' : ''
                }`}
              />
              {errors.title && (
                <p className="mt-1 text-xs text-red-600">{errors.title.message}</p>
              )}
            </div>

            <div className="sm:col-span-2">
              <label htmlFor="description" className="block text-sm font-medium text-gray-700">
                Description
              </label>
              <textarea
                id="description"
                rows={3}
                {...register('description')}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
              ></textarea>
            </div>

            <div className="sm:col-span-2">
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
                {departments.map(dept => (
                  <option key={dept.id} value={dept.id}>
                    {dept.name}
                  </option>
                ))}
              </select>
              {errors.departmentId && (
                <p className="mt-1 text-xs text-red-600">{errors.departmentId.message}</p>
              )}
            </div>

            <div>
              <label htmlFor="minSalary" className="block text-sm font-medium text-gray-700">
                Minimum Salary
              </label>
              <div className="mt-1 relative rounded-md shadow-sm">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span className="text-gray-500 sm:text-sm">$</span>
                </div>
                <input
                  type="text"
                  id="minSalary"
                  {...register('minSalary')}
                  className={`pl-7 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm ${
                    errors.minSalary ? 'border-red-300' : ''
                  }`}
                  placeholder="0.00"
                />
              </div>
              {errors.minSalary && (
                <p className="mt-1 text-xs text-red-600">{errors.minSalary.message}</p>
              )}
            </div>

            <div>
              <label htmlFor="maxSalary" className="block text-sm font-medium text-gray-700">
                Maximum Salary
              </label>
              <div className="mt-1 relative rounded-md shadow-sm">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span className="text-gray-500 sm:text-sm">$</span>
                </div>
                <input
                  type="text"
                  id="maxSalary"
                  {...register('maxSalary')}
                  className={`pl-7 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm ${
                    errors.maxSalary ? 'border-red-300' : ''
                  }`}
                  placeholder="0.00"
                />
              </div>
              {errors.maxSalary && (
                <p className="mt-1 text-xs text-red-600">{errors.maxSalary.message}</p>
              )}
            </div>
          </div>

          <div className="flex justify-end space-x-3 pt-5">
            <button
              type="button"
              onClick={() => navigate('/positions')}
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

export default PositionCreate;
