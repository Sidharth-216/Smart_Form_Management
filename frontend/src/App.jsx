import { Suspense, lazy } from 'react'
import { Navigate, Route, Routes } from 'react-router-dom'
import Layout from './components/Layout'
import ProtectedRoute from './components/ProtectedRoute'

const HomePage = lazy(() => import('./pages/HomePage'))
const FormsPage = lazy(() => import('./pages/FormsPage'))
const FormDetailPage = lazy(() => import('./pages/FormDetailPage'))
const AdminDashboard = lazy(() => import('./pages/AdminDashboard'))
const LoginPage = lazy(() => import('./pages/LoginPage'))

export default function App() {
  return (
    <Layout>
      <Suspense fallback={<div className="page-shell animate-rise">Loading platform...</div>}>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/forms" element={<FormsPage />} />
          <Route path="/forms/:id" element={<FormDetailPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route
            path="/admin"
            element={
              <ProtectedRoute role="admin">
                <AdminDashboard />
              </ProtectedRoute>
            }
          />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Suspense>
    </Layout>
  )
}
