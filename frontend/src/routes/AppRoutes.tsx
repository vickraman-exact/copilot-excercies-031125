import { Routes, Route, Navigate } from 'react-router-dom';
import { JSX, useContext } from 'react';
import { AuthContext } from '../App';

import Dashboard from '../pages/dashboard/Dashboard';
import Layout from '../components/layout/Layout';
import Login from '../pages/auth/Login';
import DepartmentCreate from '../pages/departments/DepartmentCreate';
import DepartmentDetail from '../pages/departments/DepartmentDetail';
import DepartmentList from '../pages/departments/DepartmentList';
import EmployeeCreate from '../pages/employees/EmployeeCreate';
import EmployeeDetail from '../pages/employees/EmployeeDetail';
import EmployeeList from '../pages/employees/EmployeeList';
import NotFound from '../pages/NotFound';
import PayrollDashboard from '../pages/payroll/PayrollDashboard';
import PayslipDetail from '../pages/payroll/PayslipDetail';
import PayslipList from '../pages/payroll/PayslipList';
import PositionCreate from '../pages/positions/PositionCreate';
import PositionDetail from '../pages/positions/PositionDetail';
import PositionList from '../pages/positions/PositionList';

// Protected Route Component
const ProtectedRoute = ({ children }: { children: JSX.Element }) => {
  const { isAuthenticated } = useContext(AuthContext);
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  return children;
};

const AppRoutes = () => {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      
      {/* Protected Routes */}
      <Route path="/" element={
        <ProtectedRoute>
          <Layout />
        </ProtectedRoute>
      }>
        <Route index element={<Dashboard />} />
        
        {/* Employee Routes */}
        <Route path="employees">
          <Route index element={<EmployeeList />} />
          <Route path=":id" element={<EmployeeDetail />} />
          <Route path="new" element={<EmployeeCreate />} />
        </Route>
          {/* Department Routes */}
        <Route path="departments">
          <Route index element={<DepartmentList />} />
          <Route path=":id" element={<DepartmentDetail />} />
          <Route path="new" element={<DepartmentCreate />} />
        </Route>
        
        {/* Position Routes */}
        <Route path="positions">
          <Route index element={<PositionList />} />
          <Route path=":id" element={<PositionDetail />} />
          <Route path="new" element={<PositionCreate />} />
        </Route>
        
        {/* Payroll Routes */}
        <Route path="payroll">
          <Route index element={<PayrollDashboard />} />
          <Route path="payslips" element={<PayslipList />} />
          <Route path="payslips/:id" element={<PayslipDetail />} />
        </Route>
      </Route>
      
      {/* Not Found Route */}
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
};

export default AppRoutes;
