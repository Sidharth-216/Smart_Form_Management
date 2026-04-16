import { useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import FilterSidebar from '../components/FilterSidebar'
import useDebounce from '../hooks/useDebounce'
import { formsApi } from '../services/api'

export default function FormsPage() {
  const [searchParams] = useSearchParams()
  const [query, setQuery] = useState(searchParams.get('q') ?? '')
  const debouncedQuery = useDebounce(query)
  const [filters, setFilters] = useState({ country: '', state: '', department: '', category: searchParams.get('category') ?? '' })
  const [items, setItems] = useState([])
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [facets, setFacets] = useState({})
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    formsApi.facets()
      .then(({ data }) => setFacets(data ?? {}))
      .catch(() => setFacets({}))
  }, [])

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    setError('')
    const params = { q: debouncedQuery, page, pageSize: 12, ...filters }
    formsApi.search(params)
      .then(({ data }) => {
        if (cancelled) return
        setItems(data.items ?? [])
        setTotalPages(data.totalPages ?? 1)
      })
      .catch(() => {
        if (cancelled) return
        setItems([])
        setTotalPages(1)
        setError('Unable to load forms right now.')
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })

    return () => {
      cancelled = true
    }
  }, [debouncedQuery, filters, page])

  return (
    <div className="page-shell grid gap-6 lg:grid-cols-[300px_1fr]">
      <div className="space-y-4">
        <div className="panel space-y-3">
          <h1 className="text-3xl font-black text-ink">Browse forms</h1>
          <input className="input" value={query} onChange={(event) => { setQuery(event.target.value); setPage(1) }} placeholder="Search forms" />
        </div>
        <FilterSidebar filters={filters} onChange={(next) => { setFilters(next); setPage(1) }} facets={facets} />
      </div>
      <div className="space-y-4">
        <div className="flex flex-wrap items-center justify-between gap-3 text-sm text-slate-500">
          <p>{loading ? 'Loading forms...' : `${items.length} results found`}</p>
          <p>Page {page} of {totalPages}</p>
        </div>
        {error && <div className="panel border-rose-200 bg-rose-50 text-rose-700">{error}</div>}
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {loading
            ? Array.from({ length: 6 }).map((_, index) => (
              <div key={index} className="panel animate-pulse space-y-3">
                <div className="h-4 w-24 rounded-full bg-slate-200" />
                <div className="h-6 w-4/5 rounded-full bg-slate-200" />
                <div className="h-4 w-full rounded-full bg-slate-200" />
                <div className="h-4 w-2/3 rounded-full bg-slate-200" />
              </div>
            ))
            : items.map((item) => (
              <Link key={item.id} to={`/forms/${item.id}`} className="panel group hover:-translate-y-1 transition">
                <div className="flex items-center justify-between gap-3">
                  <span className="rounded-full bg-cyan-50 px-3 py-1 text-xs font-bold uppercase tracking-[0.24em] text-cyan-800">{item.state}</span>
                  <span className="text-xs font-semibold text-slate-400">v{item.version}</span>
                </div>
                <h2 className="mt-4 text-xl font-bold text-ink group-hover:text-cyan-800">{item.title}</h2>
                <p className="mt-2 line-clamp-3 text-sm text-slate-500">{item.description}</p>
                <div className="mt-4 flex flex-wrap gap-2 text-xs text-slate-500">
                  <span>{item.department}</span>
                  <span>{item.category}</span>
                  {item.isLatest && <span className="rounded-full bg-emerald-50 px-2 py-1 text-emerald-700">Latest</span>}
                </div>
              </Link>
            ))}
        </div>
        <div className="flex items-center justify-between">
          <button className="btn-secondary disabled:opacity-60" disabled={page <= 1 || loading} onClick={() => setPage((current) => current - 1)}>Previous</button>
          <button className="btn-secondary disabled:opacity-60" disabled={page >= totalPages || loading} onClick={() => setPage((current) => current + 1)}>Next</button>
        </div>
      </div>
    </div>
  )
}
