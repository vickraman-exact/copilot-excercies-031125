import api from './api';
import type { PaginatedResponse } from './PaginatedResponse';

export interface Department {
  departmentId: string;
  name: string;
  description?: string;
  managerId?: string;
  managerName?: string;
  parentDepartmentId?: string;
  parentDepartmentName?: string;
  subDepartments?: Department[];
  employeeCount?: number;
  created: Date;
  modified: Date;
}

export interface DepartmentCreateRequest {
  name: string;
  description?: string;
  managerId?: string;
  parentDepartmentId?: string;
}

export interface DepartmentUpdateRequest extends DepartmentCreateRequest {
  departmentId: string;
}

export interface DepartmentFilter {
  search?: string;
  parentDepartmentId?: string;
  includeCounts?: boolean;
  page?: number;
  pageSize?: number;
}

const departmentService = {  // Get all departments with optional filtering and pagination

  getDepartments: async (filter?: DepartmentFilter): Promise<PaginatedResponse<Department>> => {
    // Map frontend filter keys to backend expected keys
    const params: any = {};
    if (filter) {
      if (filter.search) params.SearchTerm = filter.search;
      if (filter.page) params.Page = filter.page;
      if (filter.pageSize) params.PageSize = filter.pageSize;
      if (filter.parentDepartmentId) params.ParentDepartmentId = filter.parentDepartmentId;
      if (filter.includeCounts !== undefined) params.IncludeCounts = filter.includeCounts;
    }
    const response = await api.get<PaginatedResponse<Department>>('/api/departments', { params });
    return response.data;
  },

  // Get department by ID
  getDepartmentById: async (id: string): Promise<Department> => {
    const response = await api.get<Department>(`/api/departments/${id}`);
    return response.data;
  },

  // Create new department
  createDepartment: async (department: DepartmentCreateRequest): Promise<Department> => {
    const response = await api.post<Department>('/api/departments', department);
    return response.data;
  },

  // Update department
  updateDepartment: async (id: string, department: DepartmentUpdateRequest): Promise<Department> => {
    const response = await api.put<Department>(`/api/departments/${id}`, department);
    return response.data;
  },

  // Delete department
  deleteDepartment: async (id: string): Promise<void> => {
    await api.delete(`/api/departments/${id}`);
  },

  // Get department employees
  getDepartmentEmployees: async (id: string, filter?: { page?: number, pageSize?: number }): Promise<PaginatedResponse<any>> => {
    const response = await api.get<PaginatedResponse<any>>(`/api/departments/${id}/employees`, { params: filter });
    return response.data;
  },

  // Get department children
  getDepartmentChildren: async (id: string, filter?: { page?: number, pageSize?: number }): Promise<PaginatedResponse<Department>> => {
    const response = await api.get<PaginatedResponse<Department>>(`/api/departments/${id}/children`, { params: filter });
    return response.data;
  }
};

export default departmentService;
