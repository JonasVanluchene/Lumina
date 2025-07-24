export default function RegisterStepThree({ data, onChange, onSubmit, onBack }) {
  return (
    <div>
      <div className="mb-3">
        <label>Password</label>
        <input
          type="password"
          className="form-control"
          value={data.password}
          onChange={(e) => onChange('password', e.target.value)}
          required
        />
      </div>
      <div className="form-check mb-3">
        <input
          type="checkbox"
          className="form-check-input"
          checked={data.newsletter}
          onChange={(e) => onChange('newsletter', e.target.checked)}
        />
        <label className="form-check-label">Subscribe to newsletter</label>
      </div>
      <button className="btn btn-secondary me-2" onClick={onBack}>Back</button>
      <button className="btn btn-success" onClick={onSubmit}>Register</button>
    </div>
  );
}