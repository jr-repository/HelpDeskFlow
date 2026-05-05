type BadgeProps = {
  children: string
}

export function Badge({ children }: BadgeProps) {
  const key = children.toLowerCase().replace(/\s+/g, '-')

  return <span className={`badge badge-${key}`}>{children}</span>
}
