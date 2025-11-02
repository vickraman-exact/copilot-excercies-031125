import type { PaginatedResponse } from './PaginatedResponse';
import api from './api';

export interface Position {
  positionId: string;
  title: string;
  description?: string;
  departmentId: string;
  departmentName?: string;
  minSalary?: number;
  maxSalary?: number;
  employeeCount?: number;
  created: Date;
  modified: Date;
}

export interface PositionCreateRequest {
  title: string;
  description?: string;
  departmentId: string;
  minSalary?: number;
  maxSalary?: number;
}

export interface PositionUpdateRequest extends PositionCreateRequest {
  positionId: string;
}

export interface PositionFilter {
  search?: string;
  departmentId?: string;
  page?: number;
  pageSize?: number;
}

const positionService = {  // Get all positions with optional filtering and pagination
  getPositions: async (filter?: PositionFilter): Promise<PaginatedResponse<Position>> => {
    // Map frontend filter keys to backend expected keys
    const params: any = {};
    if (filter) {
      if (filter.search) params.SearchTerm = filter.search;
      if (filter.page) params.Page = filter.page;
      if (filter.pageSize) params.PageSize = filter.pageSize;
      if (filter.departmentId) params.DepartmentId = filter.departmentId;
    }
    const response = await api.get<PaginatedResponse<Position>>('/api/positions', { params });
    return response.data;
  },

  // Get position by ID
  getPositionById: async (id: string): Promise<Position> => {
    const response = await api.get<Position>(`/api/positions/${id}`);
    return response.data;
  },

  // Create new position
  createPosition: async (position: PositionCreateRequest): Promise<Position> => {
    const response = await api.post<Position>('/api/positions', position);
    return response.data;
  },

  // Update position
  updatePosition: async (id: string, position: PositionUpdateRequest): Promise<Position> => {
    const response = await api.put<Position>(`/api/positions/${id}`, position);
    return response.data;
  },

  // Delete position
  deletePosition: async (id: string): Promise<void> => {
    await api.delete(`/api/positions/${id}`);
  },

  // Get employees by position
  getPositionEmployees: async (id: string, filter?: { page?: number, pageSize?: number }) => {
    const response = await api.get(`/api/positions/${id}/employees`, { params: filter });
    return response.data;
  }
};

export default positionService;
