import { useState } from 'react'

const initialState = {
  title: '',
  description: '',
  country: 'India',
  state: 'Odisha',
  department: 'Revenue',
  category: 'Application',
  version: '1.0.0',
  languageCsv: 'English, Odia',
  keywordsCsv: '',
  file: null,
}

export default function UploadForm({ onSubmit, busy }) {
  const [form, setForm] = useState(initialState)
  const [error, setError] = useState('')

  const update = (key, value) => setForm((current) => ({ ...current, [key]: value }))

  const submit = async (event) => {
    event.preventDefault()
    setError('')

    if (!form.file) {
      setError('Please choose a PDF file.')
      return
    }

    if (form.file.type !== 'application/pdf' && !form.file.name.toLowerCase().endsWith('.pdf')) {
      setError('Only PDF files are allowed.')
      return
    }

    if (form.file.size > 25 * 1024 * 1024) {
      setError('PDF file size must be 25 MB or smaller.')
      return
    }

    const payload = new FormData()
    Object.entries(form).forEach(([key, value]) => {
      if (key === 'file') {
        if (value) payload.append('File', value)
        return
      }
      payload.append(key.charAt(0).toUpperCase() + key.slice(1), value)
    })
    await onSubmit(payload)
    setForm(initialState)
  }

  return (
    <form className="panel grid gap-4 md:grid-cols-2" onSubmit={submit}>
      {error && <div className="md:col-span-2 rounded-2xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700">{error}</div>}
      {['title', 'country', 'state', 'department', 'category', 'version', 'languageCsv', 'keywordsCsv'].map((key) => (
        <label key={key} className="space-y-2 md:col-span-1">
          <span className="text-sm font-medium capitalize text-slate-600">{key}</span>
          <input className="input" value={form[key]} onChange={(event) => update(key, event.target.value)} required={key !== 'keywordsCsv'} />
        </label>
      ))}
      <label className="space-y-2 md:col-span-2">
        <span className="text-sm font-medium text-slate-600">Description</span>
        <textarea className="input min-h-32 resize-y" value={form.description} onChange={(event) => update('description', event.target.value)} required />
      </label>
      <label className="space-y-2 md:col-span-2">
        <span className="text-sm font-medium text-slate-600">PDF file</span>
        <input className="input py-2" type="file" accept="application/pdf" onChange={(event) => update('file', event.target.files?.[0] ?? null)} required />
      </label>
      <div className="md:col-span-2">
        <button className="btn-primary disabled:opacity-60" disabled={busy} type="submit">
          {busy ? 'Uploading...' : 'Upload PDF'}
        </button>
      </div>
    </form>
  )
}
