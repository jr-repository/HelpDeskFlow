import type { DashboardSummary, Department, Ticket, User } from '../types/domain'

const API_BASE_URL = 'http://localhost:5183'

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
    ...options,
  })

  if (!response.ok) {
    const error = await response.text()
    throw new Error(error || 'Request failed')
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json()
}

export const api = {
  getDashboard: () => request<DashboardSummary>('/api/dashboard/summary'),

  getDepartments: () => request<Department[]>('/api/departments'),
  createDepartment: (payload: { name: string; description: string }) =>
    request<Department>('/api/departments', {
      method: 'POST',
      body: JSON.stringify(payload),
    }),

  getUsers: () => request<User[]>('/api/users'),
  createUser: (payload: {
    fullName: string
    email: string
    role: string
    departmentId: string
    isActive: boolean
  }) =>
    request<User>('/api/users', {
      method: 'POST',
      body: JSON.stringify(payload),
    }),

  getTickets: () => request<Ticket[]>('/api/tickets'),
  createTicket: (payload: {
    subject: string
    description: string
    priority: string
    departmentId: string
    requesterId: string
  }) =>
    request<Ticket>('/api/tickets', {
      method: 'POST',
      body: JSON.stringify(payload),
    }),
  assignTicket: (ticketId: string, agentId: string) =>
    request<Ticket>(`/api/tickets/${ticketId}/assign`, {
      method: 'PATCH',
      body: JSON.stringify({ agentId }),
    }),
  changeTicketStatus: (ticketId: string, status: string) =>
    request<Ticket>(`/api/tickets/${ticketId}/status`, {
      method: 'PATCH',
      body: JSON.stringify({ status }),
    }),
}
