type StatCardProps = {
  label: string
  value: number
  tone?: 'blue' | 'green' | 'orange' | 'red'
}

export function StatCard({ label, value, tone = 'blue' }: StatCardProps) {
  return (
    <div className={`stat-card stat-card-${tone}`}>
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  )
}
