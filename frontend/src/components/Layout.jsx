import { Link, NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function Layout({ children }) {
  const auth = useAuth()
  const navigate = useNavigate()

  return (
    <div className="noise min-h-screen text-slate-900">
      <header className="sticky top-0 z-30 border-b border-white/70 bg-white/70 backdrop-blur-xl">
        <div className="page-shell flex flex-wrap items-center justify-between gap-4 py-4">
          <Link to="/" className="brand-font text-lg font-bold tracking-tight text-ink sm:text-2xl">
            Smart Form Platform
          </Link>
          <nav className="flex flex-wrap items-center gap-3 text-sm font-medium text-slate-600">
            <NavLink to="/forms" className={({ isActive }) => isActive ? 'text-cyan-700' : 'hover:text-cyan-700'}>Browse</NavLink>
            <NavLink to="/admin" className={({ isActive }) => isActive ? 'text-cyan-700' : 'hover:text-cyan-700'}>Admin</NavLink>
            <NavLink to="/login" className={({ isActive }) => isActive ? 'text-cyan-700' : 'hover:text-cyan-700'}>{auth?.user ? auth.user.name : 'Login'}</NavLink>
            {auth?.user && (
              <button className="btn-secondary px-4 py-2" type="button" onClick={() => { auth.signOut(); navigate('/') }}>
                Sign out
              </button>
            )}
          </nav>
        </div>
      </header>
      <main>{children}</main>
    </div>
  )
}
