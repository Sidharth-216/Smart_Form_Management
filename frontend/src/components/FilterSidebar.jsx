const filterMap = [
  ['country', 'Country'],
  ['state', 'State'],
  ['department', 'Department'],
  ['category', 'Category'],
]

export default function FilterSidebar({ filters, onChange, facets = {} }) {
  const update = (key, value) => onChange({ ...filters, [key]: value })
  const getOptions = (key) => {
    const value = facets[key]
    return Array.isArray(value) && value.length > 0 ? value : []
  }

  return (
    <aside className="panel space-y-5">
      <div>
        <p className="text-sm font-semibold uppercase tracking-[0.24em] text-cyan-700">Refine</p>
        <h3 className="mt-1 text-xl font-bold text-ink">Structured browsing</h3>
      </div>
      {filterMap.map(([key, label]) => (
        <label key={key} className="block space-y-2">
          <span className="text-sm font-medium capitalize text-slate-600">{label}</span>
          <select className="input" value={filters[key]} onChange={(event) => update(key, event.target.value)}>
            <option value="">All</option>
            {getOptions(key).map((value) => (
              <option key={value} value={value}>{value}</option>
            ))}
          </select>
        </label>
      ))}
      <button type="button" className="btn-secondary w-full" onClick={() => onChange({ country: '', state: '', department: '', category: '' })}>
        Clear filters
      </button>
    </aside>
  )
}
