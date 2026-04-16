import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import PdfViewer from '../components/PdfViewer'
import { formsApi } from '../services/api'

export default function FormDetailPage() {
  const { id } = useParams()
  const [form, setForm] = useState(null)
  const [error, setError] = useState('')

  useEffect(() => {
    let cancelled = false
    setError('')
    setForm(null)
    formsApi.detail(id)
      .then(({ data }) => {
        if (!cancelled) setForm(data)
      })
      .catch(() => {
        if (!cancelled) setError('Unable to load this form right now.')
      })

    return () => {
      cancelled = true
    }
  }, [id])

  if (error) {
    return <div className="page-shell py-12"><div className="panel border-rose-200 bg-rose-50 text-rose-700">{error}</div></div>
  }

  if (!form) {
    return <div className="page-shell py-12 text-slate-500">Loading form...</div>
  }

  return (
    <div className="page-shell space-y-6">
      <section className="panel grid gap-6 lg:grid-cols-[1.1fr_0.9fr]">
        <div>
          <p className="text-xs font-bold uppercase tracking-[0.28em] text-cyan-700">{form.department}</p>
          <h1 className="mt-2 text-4xl font-black text-ink">{form.title}</h1>
          <p className="mt-4 text-slate-600">{form.description}</p>
          <div className="mt-6 flex flex-wrap gap-3 text-sm text-slate-500">
            <span>{form.state}</span>
            <span>{form.category}</span>
            <span>v{form.version}</span>
          </div>
          <div className="mt-6 flex flex-wrap gap-3">
            <a className="btn-primary" href={form.fileUrl} target="_blank" rel="noreferrer">Download PDF</a>
            <button className="btn-secondary" onClick={() => window.print()}>Print</button>
          </div>
          <dl className="mt-8 grid gap-4 sm:grid-cols-2">
            <div>
              <dt className="text-xs uppercase tracking-[0.24em] text-slate-400">Country</dt>
              <dd className="mt-1 text-sm font-semibold text-ink">{form.country}</dd>
            </div>
            <div>
              <dt className="text-xs uppercase tracking-[0.24em] text-slate-400">Uploaded by</dt>
              <dd className="mt-1 text-sm font-semibold text-ink">{form.uploadedBy}</dd>
            </div>
          </dl>
        </div>
        <div className="rounded-3xl bg-slate-950 p-5 text-white">
          <h2 className="text-xl font-bold">Smart metadata</h2>
          <ul className="mt-4 space-y-2 text-sm text-slate-300">
            {form.keywords?.map((keyword) => (
              <li key={keyword}>• {keyword}</li>
            ))}
          </ul>
        </div>
      </section>
      <PdfViewer url={form.previewUrl || form.fileUrl} />
    </div>
  )
}
