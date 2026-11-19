import { useState, useMemo } from 'react'
import { Link } from 'react-router-dom'
import { getStoredSession } from '../services/authService'
import './Home.css'

export default function Home() {
  // User session
  const session = getStoredSession()
  const userName = session?.user?.firstname || 'Alex'

  // Shipments state (starts empty without fake demo data)
  const [shipments, setShipments] = useState(() => {
    try {
      const stored = localStorage.getItem('docket_user_shipments')
      return stored ? JSON.parse(stored) : []
    } catch {
      return []
    }
  })

  // Selected pickup hub
  const [selectedLocation, setSelectedLocation] = useState(() => {
    return localStorage.getItem('docket_pickup_location') || 'Select location'
  })

  // Filter & Search states
  const [activeTab, setActiveTab] = useState('all') // 'all' | 'active' | 'exceptions' | 'delivered'
  const [searchQuery, setSearchQuery] = useState('')

  // Modal dialog states
  const [isLocationModalOpen, setIsLocationModalOpen] = useState(false)
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)

  // Form state for creating a shipment
  const [newTrkNumber, setNewTrkNumber] = useState('')
  const [newAddress, setNewAddress] = useState('')
  const [newRecipient, setNewRecipient] = useState('')
  const [newStatus, setNewStatus] = useState('Out for delivery')

  // Preset pickup hubs
  const presetLocations = [
    { name: 'Central Depot Hub', address: '402 Bay Street, Suite 100' },
    { name: 'Terminal 4 - Dock 12', address: 'Cargo Logistics Center, Bay 4' },
    { name: 'Downtown Drop Point', address: '14 Lake View Road, Floor 1' },
    { name: 'Eastside Delivery Lockers', address: '9 Pine Avenue, Bay B' },
  ]

  // Metric counts dynamically derived from actual data
  const metrics = useMemo(() => {
    const total = shipments.length
    const active = shipments.filter(
      (s) => s.statusType === 'out-for-delivery' || s.statusType === 'in-transit'
    ).length
    const exceptions = shipments.filter((s) => s.statusType === 'failed').length
    const delivered = shipments.filter((s) => s.statusType === 'delivered').length
    return { total, active, exceptions, delivered }
  }, [shipments])

  // Filtered shipments list
  const filteredShipments = useMemo(() => {
    let result = shipments

    if (activeTab === 'active') {
      result = result.filter(
        (s) => s.statusType === 'out-for-delivery' || s.statusType === 'in-transit'
      )
    } else if (activeTab === 'exceptions') {
      result = result.filter((s) => s.statusType === 'failed')
    } else if (activeTab === 'delivered') {
      result = result.filter((s) => s.statusType === 'delivered')
    }

    if (searchQuery.trim()) {
      const q = searchQuery.toLowerCase().trim()
      result = result.filter(
        (s) =>
          s.trkNumber.toLowerCase().includes(q) ||
          s.address.toLowerCase().includes(q) ||
          s.recipient.toLowerCase().includes(q)
      )
    }

    return result
  }, [shipments, activeTab, searchQuery])

  const handleSelectLocation = (locName) => {
    setSelectedLocation(locName)
    localStorage.setItem('docket_pickup_location', locName)
    setIsLocationModalOpen(false)
  }

  const handleCreateShipment = (e) => {
    e.preventDefault()
    if (!newAddress.trim()) return

    const trk =
      newTrkNumber.trim() ||
      `TRK-${Math.floor(10000000 + Math.random() * 90000000)}`

    const newShipment = {
      id: Date.now().toString(),
      trkNumber: trk,
      address: newAddress.trim(),
      recipient: newRecipient.trim() || 'Verified Recipient',
      status: newStatus,
      statusType:
        newStatus === 'Attempt failed'
          ? 'failed'
          : newStatus === 'Delivered'
            ? 'delivered'
            : 'out-for-delivery',
      createdAt: new Date().toLocaleDateString('en-GB', {
        day: 'numeric',
        month: 'short',
      }),
    }

    const updated = [newShipment, ...shipments]
    setShipments(updated)
    localStorage.setItem('docket_user_shipments', JSON.stringify(updated))

    setNewTrkNumber('')
    setNewAddress('')
    setNewRecipient('')
    setIsCreateModalOpen(false)
  }

  return (
    <div className="home-viewport">
      {/* ------------------------------------------------------------------ */}
      {/* Header                                                             */}
      {/* ------------------------------------------------------------------ */}
      <header className="home-header">
        <div className="home-header-inner">
          <Link to="/" className="brand-link" aria-label="Docket Home">
            <span className="brand-title">Docket</span>
          </Link>

          {/* Location Selector & Create Shipment */}
          <div className="header-actions-right">
            <button
              type="button"
              className="location-selector-btn"
              onClick={() => setIsLocationModalOpen(true)}
              aria-label="Select pickup location"
            >
              <div className="location-pin-icon">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0Z" />
                  <circle cx="12" cy="10" r="3" />
                </svg>
              </div>
              <div className="location-text-stack">
                <span className="location-sublabel">Pickup location</span>
                <span className="location-val">{selectedLocation}</span>
              </div>
              <svg className="location-chevron" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <polyline points="6 9 12 15 18 9" />
              </svg>
            </button>

            <button
              type="button"
              className="desktop-header-cta"
              onClick={() => setIsCreateModalOpen(true)}
            >
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.4" strokeLinecap="round" strokeLinejoin="round">
                <line x1="12" y1="5" x2="12" y2="19" />
                <line x1="5" y1="12" x2="19" y2="12" />
              </svg>
              <span>Create shipment</span>
            </button>
          </div>
        </div>
      </header>

      {/* ------------------------------------------------------------------ */}
      {/* Main Content                                                       */}
      {/* ------------------------------------------------------------------ */}
      <main className="home-main-content">
        {/* Overview Hero Bar with Quick Search */}
        <section className="overview-hero-bar">
          <div>
            <h1 className="greeting-headline">Hi, {userName}</h1>
            <p className="greeting-subheadline">
              Overview of your incoming packages and delivery handovers
            </p>
          </div>

          <div className="quick-track-container">
            <input
              type="text"
              placeholder="Search by tracking # or address..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="quick-track-input"
            />
            {searchQuery && (
              <button
                type="button"
                className="quick-track-btn"
                onClick={() => setSearchQuery('')}
              >
                Clear
              </button>
            )}
          </div>
        </section>

        {/* Metric Overview Tiles */}
        <div className="metrics-strip">
          <div className="stat-tile">
            <div className="stat-tile-top">
              <span className="stat-tile-label">Total Packages</span>
              <span className="stat-tile-dot"></span>
            </div>
            <span className="stat-tile-value">{metrics.total}</span>
          </div>

          <div className="stat-tile">
            <div className="stat-tile-top">
              <span className="stat-tile-label">In Transit</span>
              <span className="stat-tile-dot accent"></span>
            </div>
            <span className="stat-tile-value">{metrics.active}</span>
          </div>

          <div className="stat-tile">
            <div className="stat-tile-top">
              <span className="stat-tile-label">Exceptions</span>
              <span className="stat-tile-dot warning"></span>
            </div>
            <span className="stat-tile-value">{metrics.exceptions}</span>
          </div>

          <div className="stat-tile">
            <div className="stat-tile-top">
              <span className="stat-tile-label">Delivered</span>
              <span className="stat-tile-dot success"></span>
            </div>
            <span className="stat-tile-value">{metrics.delivered}</span>
          </div>
        </div>

        {/* Shipments Desk Card with Segmented Tabs */}
        <section className="shipments-desk-card">
          <div className="desk-card-toolbar">
            <div className="segmented-tab-switcher" role="tablist">
              <button
                type="button"
                role="tab"
                aria-selected={activeTab === 'all'}
                className={`segment-tab ${activeTab === 'all' ? 'active' : ''}`}
                onClick={() => setActiveTab('all')}
              >
                <span>All Shipments</span>
                <span className="tab-badge">{metrics.total}</span>
              </button>

              <button
                type="button"
                role="tab"
                aria-selected={activeTab === 'active'}
                className={`segment-tab ${activeTab === 'active' ? 'active' : ''}`}
                onClick={() => setActiveTab('active')}
              >
                <span>Active</span>
                <span className="tab-badge">{metrics.active}</span>
              </button>

              <button
                type="button"
                role="tab"
                aria-selected={activeTab === 'exceptions'}
                className={`segment-tab ${activeTab === 'exceptions' ? 'active' : ''}`}
                onClick={() => setActiveTab('exceptions')}
              >
                <span>Exceptions</span>
                <span className="tab-badge">{metrics.exceptions}</span>
              </button>

              <button
                type="button"
                role="tab"
                aria-selected={activeTab === 'delivered'}
                className={`segment-tab ${activeTab === 'delivered' ? 'active' : ''}`}
                onClick={() => setActiveTab('delivered')}
              >
                <span>Delivered</span>
                <span className="tab-badge">{metrics.delivered}</span>
              </button>
            </div>
          </div>

          {/* Desk Content */}
          <div className="desk-content-viewport">
            {filteredShipments.length > 0 ? (
              <div className="shipments-table-wrapper">
                {filteredShipments.map((shipment) => (
                  <article key={shipment.id} className="shipment-row-card">
                    <div className="shipment-main-col">
                      <div className="shipment-code-line">
                        <span className="shipment-tracking-id">
                          {shipment.trkNumber}
                        </span>
                        <span className={`shipment-status-badge ${shipment.statusType}`}>
                          {shipment.status}
                        </span>
                      </div>
                      <div className="shipment-address-line">
                        <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                          <path d="M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0Z" />
                          <circle cx="12" cy="10" r="3" />
                        </svg>
                        <span>{shipment.address}</span>
                        <span>•</span>
                        <span>{shipment.recipient}</span>
                      </div>
                    </div>
                  </article>
                ))}
              </div>
            ) : (
              <div className="empty-manifest-box">
                <div className="empty-manifest-icon">
                  <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
                    <path d="m7.5 4.27 9 5.15" />
                    <path d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z" />
                    <path d="m3.3 7 8.7 5 8.7-5" />
                    <path d="M12 22V12" />
                  </svg>
                </div>
                <h3 className="empty-manifest-title">
                  {searchQuery
                    ? 'No matching shipments found'
                    : activeTab === 'exceptions'
                      ? 'No delivery exceptions'
                      : activeTab === 'delivered'
                        ? 'No delivered packages yet'
                        : 'No active shipments'}
                </h3>
                <p className="empty-manifest-desc">
                  {searchQuery
                    ? `No package found matching "${searchQuery}". Please check the tracking number or address.`
                    : 'All packages are currently accounted for. When a delivery is dispatched or requires resolution, it will appear here.'}
                </p>
                <button
                  type="button"
                  className="empty-manifest-cta"
                  onClick={() => setIsCreateModalOpen(true)}
                >
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2" strokeLinecap="round" strokeLinejoin="round">
                    <line x1="12" y1="5" x2="12" y2="19" />
                    <line x1="5" y1="12" x2="19" y2="12" />
                  </svg>
                  <span>Create shipment</span>
                </button>
              </div>
            )}
          </div>
        </section>

      </main>

      {/* ------------------------------------------------------------------ */}
      {/* Mobile Bottom Bar                                                  */}
      {/* ------------------------------------------------------------------ */}
      <footer className="mobile-bottom-bar">
        <button
          type="button"
          className="mobile-create-btn"
          onClick={() => setIsCreateModalOpen(true)}
        >
          Create shipment
        </button>
      </footer>

      {/* ------------------------------------------------------------------ */}
      {/* Modal: Create Shipment                                             */}
      {/* ------------------------------------------------------------------ */}
      {isCreateModalOpen && (
        <div className="modal-overlay" role="dialog" aria-modal="true">
          <div className="modal-dialog">
            <div className="modal-header">
              <h2 className="modal-title">Create Shipment</h2>
              <button
                type="button"
                className="modal-close-btn"
                onClick={() => setIsCreateModalOpen(false)}
                aria-label="Close dialog"
              >
                ✕
              </button>
            </div>

            <form onSubmit={handleCreateShipment} className="modal-form">
              <div className="modal-form-group">
                <label className="modal-label">Tracking Number (Optional)</label>
                <input
                  type="text"
                  placeholder="e.g. TRK-48213907"
                  value={newTrkNumber}
                  onChange={(e) => setNewTrkNumber(e.target.value)}
                  className="modal-input"
                />
              </div>

              <div className="modal-form-group">
                <label className="modal-label">Delivery Address *</label>
                <input
                  type="text"
                  placeholder="e.g. 14 Lake View Road"
                  value={newAddress}
                  onChange={(e) => setNewAddress(e.target.value)}
                  className="modal-input"
                  required
                />
              </div>

              <div className="modal-form-group">
                <label className="modal-label">Recipient Name</label>
                <input
                  type="text"
                  placeholder="e.g. Alex Morgan"
                  value={newRecipient}
                  onChange={(e) => setNewRecipient(e.target.value)}
                  className="modal-input"
                />
              </div>

              <div className="modal-form-group">
                <label className="modal-label">Initial Status</label>
                <select
                  value={newStatus}
                  onChange={(e) => setNewStatus(e.target.value)}
                  className="modal-input"
                >
                  <option value="Out for delivery">Out for delivery</option>
                  <option value="Attempt failed">Attempt failed</option>
                  <option value="Delivered">Delivered</option>
                </select>
              </div>

              <div className="modal-actions-row">
                <button
                  type="button"
                  className="modal-cancel-btn"
                  onClick={() => setIsCreateModalOpen(false)}
                >
                  Cancel
                </button>
                <button type="submit" className="modal-submit-btn">
                  Create
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* ------------------------------------------------------------------ */}
      {/* Modal: Pickup Location Picker                                      */}
      {/* ------------------------------------------------------------------ */}
      {isLocationModalOpen && (
        <div className="modal-overlay" role="dialog" aria-modal="true">
          <div className="modal-dialog">
            <div className="modal-header">
              <h2 className="modal-title">Select Pickup Location</h2>
              <button
                type="button"
                className="modal-close-btn"
                onClick={() => setIsLocationModalOpen(false)}
                aria-label="Close dialog"
              >
                ✕
              </button>
            </div>

            <div className="location-options-list">
              {presetLocations.map((loc) => (
                <div
                  key={loc.name}
                  className={`location-option-item ${
                    selectedLocation === loc.name ? 'selected' : ''
                  }`}
                  onClick={() => handleSelectLocation(loc.name)}
                >
                  <div>
                    <div className="loc-opt-name">{loc.name}</div>
                    <div className="loc-opt-addr">{loc.address}</div>
                  </div>
                  {selectedLocation === loc.name && (
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#5741d8" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                      <polyline points="20 6 9 17 4 12" />
                    </svg>
                  )}
                </div>
              ))}
            </div>

            <div className="modal-actions-row">
              <button
                type="button"
                className="modal-cancel-btn"
                onClick={() => setIsLocationModalOpen(false)}
              >
                Done
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
