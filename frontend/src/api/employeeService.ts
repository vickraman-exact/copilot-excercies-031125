import type { PaginatedResponse } from './PaginatedResponse';
import api from './api';

export interface Employee {
  employeeId: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  address?: string;
  dateOfBirth?: Date;
  hireDate: Date;
  terminationDate?: Date;
  status: 'Active' | 'OnLeave' | 'Terminated';
  departmentId: string;
  departmentName?: string;
  positionId: string;
  positionTitle?: string;
  managerId?: string;
  managerName?: string;
  emergencyContact?: string;
  bankDetails?: string;
  created: Date;
  modified: Date;
}

export interface EmployeeCreateRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  address?: string;
  dateOfBirth?: Date;
  hireDate: Date;
  status: 'Active' | 'OnLeave' | 'Terminated';
  departmentId: string;
  positionId: string;
  managerId?: string;
  emergencyContact?: string;
  bankDetails?: string;
}

export interface EmployeeUpdateRequest extends EmployeeCreateRequest {
  employeeId: string;
}

export interface EmployeeFilter {
  departmentId?: string;
  positionId?: string;
  status?: 'Active' | 'OnLeave' | 'Terminated';
  search?: string;
  page?: number;
  pageSize?: number;
}

const employeeService = {  // Get all employees with optional filtering and pagination
  getEmployees: async (filter?: EmployeeFilter): Promise<PaginatedResponse<Employee>> => {    // Map frontend filter keys to backend expected keys
    const params: any = {};
    if (filter) {
      if (filter.search) params.SearchTerm = filter.search;
      if (filter.page) params.PageNumber = filter.page;
      if (filter.pageSize) params.PageSize = filter.pageSize;
      if (filter.departmentId) params.DepartmentId = filter.departmentId;
      if (filter.positionId) params.PositionId = filter.positionId;
      if (filter.status) params.status = filter.status;
    }
    const response = await api.get<PaginatedResponse<Employee>>('/api/employees', { params });
    return response.data;
  },

  // Get employee by ID
  getEmployeeById: async (id: string): Promise<Employee> => {
    const response = await api.get<Employee>(`/api/employees/${id}`);
    return response.data;
  },

  // Create new employee
  createEmployee: async (employee: EmployeeCreateRequest): Promise<Employee> => {
    const response = await api.post<Employee>('/api/employees', employee);
    return response.data;
  },

  // Update employee
  updateEmployee: async (id: string, employee: EmployeeUpdateRequest): Promise<Employee> => {
    const response = await api.put<Employee>(`/api/employees/${id}`, employee);
    return response.data;
  },

  // Delete/deactivate employee
  deleteEmployee: async (id: string): Promise<void> => {
    await api.delete(`/api/employees/${id}`);
  },

  // Get employee documents
  getEmployeeDocuments: async (id: string) => {
    const response = await api.get(`/api/employees/${id}/documents`);
    return response.data;
  },

  // Upload employee document
  uploadEmployeeDocument: async (id: string, file: File) => {
    const formData = new FormData();
    formData.append('file', file);

    const response = await api.post(`/api/employees/${id}/documents`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    return response.data;
  },
};

export default employeeService;
