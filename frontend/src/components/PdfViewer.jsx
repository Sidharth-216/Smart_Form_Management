import { Document, Page, pdfjs } from 'react-pdf'
import workerSrc from 'pdfjs-dist/build/pdf.worker.min.mjs?url'

pdfjs.GlobalWorkerOptions.workerSrc = workerSrc

export default function PdfViewer({ url, onLoadSuccess }) {
  return (
    <div className="overflow-hidden rounded-3xl border border-slate-200 bg-white shadow-glow">
      <Document
        file={url}
        onLoadSuccess={onLoadSuccess}
        loading={<div className="p-8 text-sm text-slate-500">Loading PDF...</div>}
        error={<div className="p-8 text-sm text-rose-600">Preview unavailable. Use download or print instead.</div>}
      >
        <Page pageNumber={1} renderAnnotationLayer={false} renderTextLayer={false} width={800} />
      </Document>
    </div>
  )
}
