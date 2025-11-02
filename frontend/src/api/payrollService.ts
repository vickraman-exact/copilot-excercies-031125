import api from './api';

export interface Payslip {
  payslipId: string;
  employeeId: string;
  employeeName?: string;
  payPeriodId: string;
  payPeriodName?: string;
  paymentDate?: Date;
  grossPay: number;
  netPay: number;
  deductions: number;
  taxes: number;
  generatedDate: Date;
  status: 'Draft' | 'Approved' | 'Paid';
  notes?: string;
}

export interface PayslipDetail extends Payslip {
  employeeEmail?: string;
  departmentName?: string;
  positionName?: string;
  earnings: PayslipItem[];
  deductionItems: PayslipItem[];
  taxItems: PayslipItem[];
  ytdEarnings: number;
  ytdDeductions: number;
  ytdTaxes: number;
}

export interface PayslipItem {
  id: string;
  type: string;
  description?: string;
  amount: number;
}

export interface PayPeriod {
  payPeriodId: string;
  startDate: Date;
  endDate: Date;
  paymentDate: Date;
  status: 'Draft' | 'Processing' | 'Completed';
  employeeCount?: number;
  totalAmount?: number;
}

export interface PayslipFilter {
  employeeId?: string;
  payPeriodId?: string;
  status?: 'Draft' | 'Approved' | 'Paid';
  fromDate?: Date;
  toDate?: Date;
  search?: string;
  page?: number;
  pageSize?: number;
}

const payrollService = {  // Get payslips with optional filtering
  getPayslips: async (filter?: PayslipFilter) => {
    const response = await api.get('/api/payslips', { params: filter });
    return response.data;
  },

  // Get payslip by ID
  getPayslipById: async (id: string) => {
    const response = await api.get(`/api/payslips/${id}`);
    return response.data;
  },

  // Generate payslips for a pay period
  generatePayslips: async (payPeriodId: string) => {
    const response = await api.post(`/api/payperiods/${payPeriodId}/process`, {});
    return response.data;
  },

  // Approve payslips
  approvePayslips: async (payslipIds: string[]) => {
    // Approve one by one as per backend API design
    const results = [];
    for (const id of payslipIds) {
      const response = await api.put(`/api/payslips/${id}`, { status: 'Approved' });
      results.push(response.data);
    }
    return results;
  },

  // Mark payslips as paid
  markAsPaid: async (payslipIds: string[]) => {
    // Mark as paid one by one as per backend API design
    const results = [];
    for (const id of payslipIds) {
      const response = await api.put(`/api/payslips/${id}/pay`, {});
      results.push(response.data);
    }
    return results;
  },

  // Get pay periods
  getPayPeriods: async () => {
    const response = await api.get('/api/payperiods');
    return response.data;
  },

  // Get pay period by ID
  getPayPeriodById: async (id: string) => {
    const response = await api.get(`/api/payperiods/${id}`);
    return response.data;
  },

  // Create pay period
  createPayPeriod: async (payPeriod: { startDate: Date; endDate: Date; paymentDate: Date }) => {
    const response = await api.post('/api/payperiods', payPeriod);
    return response.data;
  },

  // Get employee payslips
  getEmployeePayslips: async (employeeId: string, filter?: { page?: number; pageSize?: number }) => {
    const response = await api.get(`/api/employees/${employeeId}/payslips`, { params: filter });
    return response.data;
  },

  // Get employee YTD summary
  getEmployeeYTDSummary: async (employeeId: string, year?: number) => {
    // This might need to be implemented in the backend
    const response = await api.get(`/api/employees/${employeeId}/ytd-summary`, { 
      params: { year: year || new Date().getFullYear() } 
    });
    return response.data;
  }
};

export default payrollService;
