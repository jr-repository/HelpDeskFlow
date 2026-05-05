import { useEffect, useMemo, useState } from 'react'
import { api } from './api/client'
import { Badge } from './components/Badge'
import { SectionCard } from './components/SectionCard'
import { StatCard } from './components/StatCard'
import type { DashboardSummary, Department, Ticket, User } from './types/domain'
import { formatDate } from './utils/format'
import './index.css'

function App() {
  const [dashboard, setDashboard] = useState<DashboardSummary | null>(null)
  const [departments, setDepartments] = useState<Department[]>([])
  const [users, setUsers] = useState<User[]>([])
  const [tickets, setTickets] = useState<Ticket[]>([])
  const [loading, setLoading] = useState(true)
  const [message, setMessage] = useState('')

  const [departmentForm, setDepartmentForm] = useState({
    name: 'Technical Support',
    description: 'Handles product issues, bugs, and technical assistance.',
  })

  const [userForm, setUserForm] = useState({
    fullName: 'Alex Morgan',
    email: 'alex.morgan@helpdeskflow.test',
    role: 'Agent',
    departmentId: '',
    isActive: true,
  })

  const [ticketForm, setTicketForm] = useState({
    subject: 'Unable to access company dashboard',
    description: 'The user receives an access error when opening the dashboard.',
    priority: 'High',
    departmentId: '',
    requesterId: '',
  })

  async function loadData() {
    setLoading(true)

    try {
      const [summary, departmentList, userList, ticketList] = await Promise.all([
        api.getDashboard(),
        api.getDepartments(),
        api.getUsers(),
        api.getTickets(),
      ])

      setDashboard(summary)
      setDepartments(departmentList)
      setUsers(userList)
      setTickets(ticketList)

      setUserForm((current) => ({
        ...current,
        departmentId: current.departmentId || departmentList[0]?.id || '',
      }))

      setTicketForm((current) => ({
        ...current,
        departmentId: current.departmentId || departmentList[0]?.id || '',
        requesterId: current.requesterId || userList[0]?.id || '',
      }))
    } catch {
      setMessage('Backend is not reachable. Start the API at http://localhost:5183')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  const agents = useMemo(() => users.filter((user) => user.role !== 'Requester'), [users])

  async function createDepartment() {
    await api.createDepartment(departmentForm)
    setMessage('Department created successfully.')
    await loadData()
  }

  async function createUser() {
    if (!userForm.departmentId) {
      setMessage('Create a department first.')
      return
    }

    await api.createUser(userForm)
    setMessage('User created successfully.')
    await loadData()
  }

  async function createTicket() {
    if (!ticketForm.departmentId || !ticketForm.requesterId) {
      setMessage('Create a department and user first.')
      return
    }

    await api.createTicket(ticketForm)
    setMessage('Ticket created successfully.')
    await loadData()
  }

  async function assignTicket(ticketId: string, agentId: string) {
    if (!agentId) {
      return
    }

    await api.assignTicket(ticketId, agentId)
    setMessage('Ticket assigned successfully.')
    await loadData()
  }

  async function changeStatus(ticketId: string, status: string) {
    await api.changeTicketStatus(ticketId, status)
    setMessage('Ticket status updated.')
    await loadData()
  }

  function getDepartmentName(id: string) {
    return departments.find((department) => department.id === id)?.name || 'Unknown Department'
  }

  function getUserName(id?: string | null) {
    return users.find((user) => user.id === id)?.fullName || 'Unassigned'
  }

  return (
    <main className="app-shell">
      <header className="hero">
        <div>
          <span className="eyebrow">HelpDeskFlow</span>
          <h1>Helpdesk Management</h1>
          <p>
            Manage departments, users, tickets, assignments, priorities, and support workload
            with a lightweight fullstack system.
          </p>
        </div>
        <button onClick={loadData}>Refresh Data</button>
      </header>

      {message && <div className="notice">{message}</div>}

      {loading ? (
        <div className="loading">Loading HelpDeskFlow data...</div>
      ) : (
        <>
          <section className="stats-grid">
            <StatCard label="Tickets" value={dashboard?.totals.tickets || 0} />
            <StatCard label="Users" value={dashboard?.totals.users || 0} tone="green" />
            <StatCard label="Departments" value={dashboard?.totals.departments || 0} tone="orange" />
            <StatCard label="Urgent Tickets" value={dashboard?.tickets.urgent || 0} tone="red" />
          </section>

          <div className="content-grid">
            <SectionCard title="Create Department" description="Register a support department.">
              <div className="form-grid">
                <input
                  value={departmentForm.name}
                  onChange={(event) => setDepartmentForm({ ...departmentForm, name: event.target.value })}
                  placeholder="Department name"
                />
                <textarea
                  value={departmentForm.description}
                  onChange={(event) =>
                    setDepartmentForm({ ...departmentForm, description: event.target.value })
                  }
                  placeholder="Description"
                />
                <button onClick={createDepartment}>Create Department</button>
              </div>
            </SectionCard>

            <SectionCard title="Create User" description="Register an agent, manager, or requester.">
              <div className="form-grid">
                <input
                  value={userForm.fullName}
                  onChange={(event) => setUserForm({ ...userForm, fullName: event.target.value })}
                  placeholder="Full name"
                />
                <input
                  value={userForm.email}
                  onChange={(event) => setUserForm({ ...userForm, email: event.target.value })}
                  placeholder="Email"
                />
                <select
                  value={userForm.role}
                  onChange={(event) => setUserForm({ ...userForm, role: event.target.value })}
                >
                  <option>Admin</option>
                  <option>Manager</option>
                  <option>Agent</option>
                  <option>Requester</option>
                </select>
                <select
                  value={userForm.departmentId}
                  onChange={(event) => setUserForm({ ...userForm, departmentId: event.target.value })}
                >
                  <option value="">Select department</option>
                  {departments.map((department) => (
                    <option key={department.id} value={department.id}>
                      {department.name}
                    </option>
                  ))}
                </select>
                <button onClick={createUser}>Create User</button>
              </div>
            </SectionCard>

            <SectionCard title="Create Ticket" description="Open a new support ticket.">
              <div className="form-grid">
                <input
                  value={ticketForm.subject}
                  onChange={(event) => setTicketForm({ ...ticketForm, subject: event.target.value })}
                  placeholder="Subject"
                />
                <textarea
                  value={ticketForm.description}
                  onChange={(event) =>
                    setTicketForm({ ...ticketForm, description: event.target.value })
                  }
                  placeholder="Description"
                />
                <select
                  value={ticketForm.priority}
                  onChange={(event) => setTicketForm({ ...ticketForm, priority: event.target.value })}
                >
                  <option>Low</option>
                  <option>Medium</option>
                  <option>High</option>
                  <option>Urgent</option>
                </select>
                <select
                  value={ticketForm.departmentId}
                  onChange={(event) =>
                    setTicketForm({ ...ticketForm, departmentId: event.target.value })
                  }
                >
                  <option value="">Select department</option>
                  {departments.map((department) => (
                    <option key={department.id} value={department.id}>
                      {department.name}
                    </option>
                  ))}
                </select>
                <select
                  value={ticketForm.requesterId}
                  onChange={(event) => setTicketForm({ ...ticketForm, requesterId: event.target.value })}
                >
                  <option value="">Select requester</option>
                  {users.map((user) => (
                    <option key={user.id} value={user.id}>
                      {user.fullName}
                    </option>
                  ))}
                </select>
                <button onClick={createTicket}>Create Ticket</button>
              </div>
            </SectionCard>

            <SectionCard title="Ticket Overview" description="Track ticket status and assignment.">
              <div className="ticket-list">
                {tickets.length === 0 ? (
                  <div className="empty-state">No tickets available.</div>
                ) : (
                  tickets.map((ticket) => (
                    <article className="ticket-card" key={ticket.id}>
                      <div className="ticket-topline">
                        <span>{ticket.ticketNumber}</span>
                        <div>
                          <Badge>{ticket.priority}</Badge>
                          <Badge>{ticket.status}</Badge>
                        </div>
                      </div>
                      <h3>{ticket.subject}</h3>
                      <p>{ticket.description}</p>
                      <div className="ticket-meta">
                        <span>Department: {getDepartmentName(ticket.departmentId)}</span>
                        <span>Requester: {getUserName(ticket.requesterId)}</span>
                        <span>Agent: {getUserName(ticket.assignedAgentId)}</span>
                        <span>Created: {formatDate(ticket.createdAt)}</span>
                      </div>
                      <div className="ticket-actions">
                        <select
                          value={ticket.assignedAgentId || ''}
                          onChange={(event) => assignTicket(ticket.id, event.target.value)}
                        >
                          <option value="">Assign agent</option>
                          {agents.map((agent) => (
                            <option key={agent.id} value={agent.id}>
                              {agent.fullName}
                            </option>
                          ))}
                        </select>
                        <select
                          value={ticket.status}
                          onChange={(event) => changeStatus(ticket.id, event.target.value)}
                        >
                          <option>Open</option>
                          <option>In Progress</option>
                          <option>Resolved</option>
                          <option>Closed</option>
                        </select>
                      </div>
                    </article>
                  ))
                )}
              </div>
            </SectionCard>
          </div>
        </>
      )}
    </main>
  )
}

export default App
