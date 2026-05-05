export type Department = {
  id: string
  name: string
  description: string
  createdAt: string
  updatedAt: string
}

export type User = {
  id: string
  fullName: string
  email: string
  role: string
  departmentId: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export type Ticket = {
  id: string
  ticketNumber: string
  subject: string
  description: string
  priority: string
  status: string
  departmentId: string
  requesterId: string
  assignedAgentId?: string | null
  resolvedAt?: string | null
  createdAt: string
  updatedAt: string
}

export type TicketComment = {
  id: string
  ticketId: string
  userId: string
  message: string
  isInternalNote: boolean
  createdAt: string
  updatedAt: string
}

export type DashboardSummary = {
  totals: {
    tickets: number
    users: number
    departments: number
    comments: number
    activityLogs: number
  }
  tickets: {
    open: number
    inProgress: number
    resolved: number
    closed: number
    urgent: number
    high: number
    medium: number
    low: number
  }
  workload: {
    id: string
    fullName: string
    assignedTickets: number
  }[]
}
