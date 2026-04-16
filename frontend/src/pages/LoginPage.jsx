import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { authApi } from '../services/api'
import { useAuth } from '../context/AuthContext'

export default function LoginPage() {
  const [mode, setMode] = useState('login')
  const [form, setForm] = useState({ name: '', email: '', password: '' })
  const [error, setError] = useState('')
  const auth = useAuth()
  const navigate = useNavigate()

  const submit = async (event) => {
    event.preventDefault()
    setError('')

    try {
      const response = mode === 'login'
        ? await authApi.login({ email: form.email, password: form.password })
        : await authApi.register({ name: form.name, email: form.email, password: form.password })
      auth.signIn(response.data)
      navigate(response.data.role === 'admin' ? '/admin' : '/forms')
    } catch {
      setError('Authentication failed. Please check your credentials and try again.')
    }
  }

  return (
    <div className="page-shell grid place-items-center py-10">
      <form className="panel w-full max-w-xl space-y-4" onSubmit={submit}>
        <div className="flex gap-2">
          {['login', 'register'].map((item) => (
            <button key={item} type="button" className={mode === item ? 'btn-primary' : 'btn-secondary'} onClick={() => setMode(item)}>
              {item}
            </button>
          ))}
        </div>
        <h1 className="text-3xl font-black text-ink">{mode === 'login' ? 'Sign in' : 'Create account'}</h1>
        {error && <div className="rounded-2xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700">{error}</div>}
        {mode === 'register' && (
          <input className="input" placeholder="Name" value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} />
        )}
        <input className="input" placeholder="Email" type="email" value={form.email} onChange={(event) => setForm({ ...form, email: event.target.value })} />
        <input className="input" placeholder="Password" type="password" value={form.password} onChange={(event) => setForm({ ...form, password: event.target.value })} />
        {mode === 'register' && <p className="text-sm text-slate-500">New accounts are created with user access. Admin access is assigned separately.</p>}
        <button className="btn-primary" type="submit">Continue</button>
      </form>
    </div>
  )
}
