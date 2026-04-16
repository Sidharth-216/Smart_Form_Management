import { Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function ProtectedRoute({ children, role }) {
  const auth = useAuth()
  const adminPreview = import.meta.env.VITE_ADMIN_PREVIEW === 'true'

  if (adminPreview && role === 'admin') {
    return children
  }

  if (!auth?.ready) {
    return <div className="page-shell py-16 text-sm text-slate-500">Loading session...</div>
  }

  const allowed = auth?.user && (!role || auth.user.role === role)
  return allowed ? children : <Navigate to="/login" replace />
}
