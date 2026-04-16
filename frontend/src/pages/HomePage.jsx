import { Link, useNavigate } from 'react-router-dom'
import { useState } from 'react'
import SearchBar from '../components/SearchBar'

const stats = [
  ['20K+', 'concurrent users target'],
  ['100ms', 'cached API response goal'],
  ['Odia', 'multi-language ready'],
]

const categories = ['Revenue forms', 'Transport licenses', 'Health certificates', 'Education applications', 'Land records', 'Business permits']

export default function HomePage() {
  const [query, setQuery] = useState('')
  const navigate = useNavigate()

  return (
    <div className="page-shell space-y-8 py-10">
      <section className="panel overflow-hidden relative">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_top_right,rgba(6,182,212,.16),transparent_30%),radial-gradient(circle_at_bottom_left,rgba(217,119,6,.14),transparent_24%)]" />
        <div className="relative grid gap-8 lg:grid-cols-[1.2fr_0.8fr]">
          <div className="space-y-6">
            <span className="inline-flex rounded-full bg-cyan-50 px-3 py-1 text-xs font-bold uppercase tracking-[0.28em] text-cyan-800">Government forms, organized</span>
            <h1 className="max-w-3xl text-4xl font-black tracking-tight text-ink sm:text-6xl">Search, preview, and print forms with structured intelligence.</h1>
            <p className="max-w-2xl text-lg leading-8 text-slate-600">A production-ready platform for Xerox and print shops to manage physical government forms across India with country, state, department, and category browsing plus AI-assisted tagging.</p>
            <SearchBar value={query} onChange={setQuery} onSubmit={() => navigate(`/forms?q=${encodeURIComponent(query)}`)} />
            <div className="flex flex-wrap gap-3">
              <Link to="/forms" className="btn-primary">Browse forms</Link>
              <Link to="/admin" className="btn-secondary">Admin dashboard</Link>
            </div>
          </div>
          <div className="grid gap-4 sm:grid-cols-3 lg:grid-cols-1">
            {stats.map(([value, label]) => (
              <div key={label} className="panel bg-white/70">
                <div className="text-3xl font-black text-ink">{value}</div>
                <div className="mt-1 text-sm text-slate-500">{label}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
        {categories.map((category) => (
          <Link key={category} to={`/forms?category=${encodeURIComponent(category)}`} className="panel group transition hover:-translate-y-1">
            <p className="text-xs font-bold uppercase tracking-[0.28em] text-cyan-700">Category</p>
            <h2 className="mt-3 text-2xl font-bold text-ink group-hover:text-cyan-800">{category}</h2>
            <p className="mt-2 text-sm leading-6 text-slate-500">Browse curated government forms with OCR metadata, semantic tags, and printable previews.</p>
          </Link>
        ))}
      </section>
    </div>
  )
}
