import { useEffect, useState } from 'react'
import UploadForm from '../components/UploadForm'
import { adminApi, formsApi } from '../services/api'

export default function AdminDashboard() {
  const [pending, setPending] = useState([])
  const [forms, setForms] = useState([])
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')

  const refresh = async () => {
    setError('')
    try {
      const [pendingResponse, formsResponse] = await Promise.all([
        adminApi.pendingUploads(),
        formsApi.list({ page: 1, pageSize: 6 }),
      ])
      setPending(pendingResponse.data ?? [])
      setForms(formsResponse.data?.items ?? [])
    } catch {
      setPending([])
      setForms([])
      setError('Admin data could not be loaded.')
    }
  }

  useEffect(() => { refresh() }, [])

  const upload = async (formData) => {
    setBusy(true)
    try {
      await formsApi.upload(formData)
      await refresh()
    } finally {
      setBusy(false)
    }
  }

  const removeForm = async (id) => {
    await formsApi.remove(id)
    await refresh()
  }

  return (
    <div className="page-shell space-y-6">
      <section className="panel">
        <h1 className="text-4xl font-black text-ink">Admin dashboard</h1>
        <p className="mt-2 text-slate-500">Upload PDFs, manage moderation, and keep the latest version flagged.</p>
      </section>
      <UploadForm onSubmit={upload} busy={busy} />
      {error && <div className="panel border-rose-200 bg-rose-50 text-rose-700">{error}</div>}
      <section className="panel space-y-4">
        <h2 className="text-2xl font-bold text-ink">Recent forms</h2>
        <div className="grid gap-3">
          {forms.map((item) => (
            <div key={item.id} className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 md:flex-row md:items-center md:justify-between">
              <div>
                <div className="font-semibold text-ink">{item.title}</div>
                <div className="text-sm text-slate-500">{item.state} · {item.department} · v{item.version}</div>
              </div>
              <button className="btn-secondary" type="button" onClick={() => removeForm(item.id)}>
                Delete
              </button>
            </div>
          ))}
        </div>
      </section>
      <section className="panel space-y-4">
        <h2 className="text-2xl font-bold text-ink">Pending uploads</h2>
        <div className="space-y-3">
          {pending.map((item) => (
            <div key={item.id} className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 md:flex-row md:items-center md:justify-between">
              <div>
                <div className="font-semibold text-ink">{item.formId}</div>
                <div className="text-sm text-slate-500">Uploaded by {item.uploadedBy}</div>
              </div>
              <div className="flex gap-2">
                <button className="btn-primary" onClick={async () => { await adminApi.approve(item.id); await refresh() }}>Approve</button>
                <button className="btn-secondary" onClick={async () => { await adminApi.reject(item.id); await refresh() }}>Reject</button>
              </div>
            </div>
          ))}
        </div>
      </section>
    </div>
  )
}
