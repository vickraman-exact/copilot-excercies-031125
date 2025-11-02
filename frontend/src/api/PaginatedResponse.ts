// Common generic paginated response type for API responses
export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  success?: boolean;
  message?: string;
}
