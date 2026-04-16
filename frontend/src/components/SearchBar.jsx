export default function SearchBar({ value, onChange, onSubmit, placeholder = 'Search by title, department, keyword, or scheme name' }) {
  return (
    <form
      className="flex flex-col gap-3 rounded-3xl bg-white/80 p-3 shadow-glow ring-1 ring-slate-200/70 sm:flex-row"
      onSubmit={(event) => {
        event.preventDefault()
        onSubmit?.()
      }}
    >
      <input
        value={value}
        onChange={(event) => onChange(event.target.value)}
        className="input flex-1 border-0 bg-transparent focus:ring-0"
        placeholder={placeholder}
      />
      <button type="submit" className="btn-primary">
        Search
      </button>
    </form>
  )
}
